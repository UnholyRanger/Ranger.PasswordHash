# Ranger.PasswordHash

A Library to support swapping of password algorithms with ease. Protecting our systems by considering security is always a moving target and one part of this moving piece is ensuring our user's passwords are hashed securely. Over time we find hashes to be weak, or systems get stronger calling for larger iterations. However, most applications are written to hash once and implement fall back logic when it needs to change..... over... and over... and over sometimes, again.
One company I worked at, we implemented a version of an agile based password hashing library. The original code had 3 levels of fallback testing. So in the event of an invalid password, it would go through 3 different hash checks. Now, they know which version is used, and an update to the library will update the hash algorithm.
This is version 3.0 of that. More versatile, and open for additions. It's mainly built around PBKDFv2, but you can build your own extension library to use other hash algorithms such as bcrypt or scrypt.

## Tech/Framework used

Built with:

- [.NET 10](https://dotnet.microsoft.com/en-us/download/dotnet)

## Installation

```
dotnet add package Ranger.PasswordHash
```

## Code Examples

The main idea of the project is that applications will store the hashed password, salt, and algorithm ID in the database. This is acceptable from a security standpoint. When checking a password, you retrieve the hash, salt and algorithm ID and pass them to `Verify`. When setting a new password, you hash it and store the result.

### Built-in Algorithms

| Algorithm ID          | Hash function | Iterations | Notes                     |
| --------------------- | ------------- | ---------- | ------------------------- |
| `PBKDFv2-SHA256-600k` | HMAC-SHA256   | 600,000    | Default preferred         |
| `PBKDFv2-610k`        | HMAC-SHA1     | 610,000    | Kept for existing hashes  |
| `PBKDFv2-10k`         | HMAC-SHA1     | 10,000     | Kept for existing hashes  |

### Set a Password

``` C#
var hashService = serviceProvider.GetRequiredService<IPasswordHash>();

// No salt or algorithm ID: a random salt is generated and the preferred algorithm is used
var result = hashService.Hash(new PasswordHashRequest { Password = "MySecurePassword" });

await database.SaveCredentialsAsync(
    passwordHash: result.Value,
    salt: result.Salt.ToBase64(),
    algorithmId: result.AlgorithmId);
```

Always store the salt with `ToBase64()`. Salts are random bytes and can't be stored safely as plain text.

### Validate a Password

``` C#
var user = await database.GetCredentialsAsync(username);

var verification = hashService.Verify(
    password: passwordText,
    hash: user.PasswordHash,
    salt: Salt.FromBase64(user.Salt),
    algorithmId: user.AlgorithmId);

switch (verification)
{
    case PasswordVerifyResult.Failed:
        throw new InvalidCredentialException("Invalid username and password combination");

    case PasswordVerifyResult.SuccessRehashNeeded:
        // The stored hash uses an older algorithm. We have the plain text password right now,
        // so upgrade it without the user noticing.
        var upgraded = hashService.Hash(new PasswordHashRequest { Password = passwordText });
        await database.SaveCredentialsAsync(upgraded.Value, upgraded.Salt.ToBase64(), upgraded.AlgorithmId);
        break;
}

CompleteLogin();
```

`Verify` compares hashes in fixed time. Don't compare hash strings with `==`, which can leak timing information.

### Setting Up The Use Of This Library

The library is built for `IServiceCollection`, aka dependency injection. To use all of the default settings and internal algorithms, just do the following:

``` C#
serviceCollection.AddDynamicPasswordHashing();
```

Now you can start dynamically hashing your passwords with whichever algorithms come prepackaged.

### Build Your Own Algorithm

This library is extensible so you can build your own algorithms to use. In some cases, you may have a home grown algorithm, or are currently using one not supported by this library. Wrap it with our interface and register it, then update the database to have the ID you gave your algorithm and start using it!

``` C#
public class MyInternalHashAlgorithm : IPasswordHashAlgorithm
{
    public string Id => "MyVeryOwnAlgorithm";

    public PasswordHashResult Hash(PasswordHashRequest request)
    {
        // hash request.Password with request.Salt.Value
    }
}

serviceCollection.AddDynamicPasswordHashing(o =>
{
    o.RegisterAlgorithm<MyInternalHashAlgorithm>();
});
```

If you want to ONLY support your own algorithms, you can call `UseOnlyExternalAlgorithms()` on the `IPasswordHashConfiguration`. This will prevent loading in prepackaged algorithms. Lastly, if your custom algorithm is preferred and you want all others to be considered obsolete, set it as such:

``` C#
serviceCollection.AddDynamicPasswordHashing(o =>
{
    o.RegisterAlgorithm<MyInternalHashAlgorithm>()
     .SetPreferredAlgorithm<MyInternalHashAlgorithm>();
});
```

The preferred algorithm must be registered. Resolving `IPasswordHash` throws a `PasswordHashException` if it isn't.

## Upgrading from 2.x

- **Namespaces:** remove `using Ranger.PasswordHash.Core;`. Those types are now in `Ranger.PasswordHash`.
- **Salts:** the implicit `string` conversions are gone. Load salts you stored as plain text with `Salt.FromText(storedSalt)`, and store new salts with `ToBase64()`. When a user with an old salt signs in, rehash and save their password with a new salt.
- **Default algorithm:** `PBKDFv2-SHA256-600k` is now preferred. If you rely on the default, existing `PBKDFv2-10k` and `PBKDFv2-610k` hashes still verify, but report `SuccessRehashNeeded` so they are upgraded at the next sign-in.
- **Renamed:** `SetPrefferedAlgorithm` is now `SetPreferredAlgorithm`. The old name still works but is obsolete.
- **Verify:** replace `Hash(...).Value == storedHash` with `Verify(...)`.

See the [changelog](https://github.com/UnholyRanger/Ranger.PasswordHash/blob/master/CHANGELOG.md) for the full list of changes.

## Contribute

I hope this library is helpful and I'm open to other ideas. Feel free to fork and extend this library even more.

## License

MIT No Attribution (MIT-0). See [LICENSE](https://github.com/UnholyRanger/Ranger.PasswordHash/blob/master/LICENSE).

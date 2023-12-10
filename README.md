# Ranger.PasswordHash

A Library to support swapping of password algorithms with ease. Protecting our systems by considering security is always a moving target and one part of this moving piece is ensuring our user's passwords are hashed securely. Over time we find hashes to be weak, or systems get stronger calling for larger iterations. However, most applications are written to hash once and implement fall back logic when it needs to change..... over... and over... and over sometimes, again.
One company I worked at, we implemented a version of an agile based password hashing library. The original code had 3 levels of fallback testing. So in the event of an invalid password, it would go through 3 different hash checks. Now, they know which version is used, and an update to the library will update the hash algorithm.
This, is version 2.0 of that. More versatile, and open for additions. It's mainly built around PBKDFv2, but you can build your own extension library to use other hash algorithms such as bcrypt or scrypt.

## Tech/Framework used

Built with:

- [Dotnet 6/8](https://dotnet.microsoft.com/en-us/download/dotnet)

## Code Examples

The main idea of the project is that applications will store the hashed password, salt, and algorithm ID in the database. This is acceptable from a security standpoint. When comparing a password, you will retrieve the salt/algorithm ID and pass that into the Agile algorithm. When setting a new password, you will pass the unhashed password with salt and then store the result.

### Validate Password

``` C#
string passwordtext = "MySecurePassword";
var hashService = service.GetService<IPasswordHash>();

// pull these details from the database
var userDetails = new {
    PasswordHash = "YnVtRc",
    Salt = "secretSalt",
    AlgorithmId = "PBKDFv2-10k"
};

// make the request to hash
var passwordRequest = new PasswordHashRequest {
    AlgorithmId = userDetails.AlgorithmId,
    Password = passwordtext,
    Salt = userDetails.Salt
};

var hashResponse = hashService.Hash(passwordRequest);

if (hashResponse.Value == userDetails.PasswordHash)
{
    if (hashResponse.IsObSolete)
        UpdatePasswordAsync(passwordtext, passwordRequest);
    CompleteLogin();
}
else
    throw new PasswordException("Invalid username and password combination");
```

### Update Password

Since we have the plaintext password at the same time we know the algorithm is obsolete and it needs to be updated, we can do this behind the scenes from the user. They won't know we just made their password more secure.

``` C#
public static async void UpdatePasswordAsync(string plaintext, PasswordHashRequest originalRequest) 
{
    originalRequest.AlgorithmId = null;
    originalRequest.Salt = null; // This is optional, if you'd like to generate a new salt
    var response = hashService.Hash(originalRequest);
    
    // update the user details
    Database.UpdateUserCredentialsAsync(Password: response.Value, Salt: originalRequest.Salt, AlgorithmId: response.AlgorithmId);
}
```

### Setting Up The Use Of This Library

In Version 2.0, the library was moved to be move IServiceCollection, aka dependency injection, friendly. To use all of the default settings and internal algorithms, just do the following:

``` C#
serviceCollection.AddDynamicPasswordHashing();
```

Now you can start dynamically hashing your algorithms with whichever ones come prepackaged.

### Build Your Own Algorithm

This library is extensible that you can build your own algorithms to use. Some cases, you may have a home grown algorithm, or are currently using one not supported by this library. Wrap it with our interface and register it, then update the database to have the ID you gave your algorithm and start using it!

``` C#
public MyInternalHashAlgorithm : IPasswordHashAlgorithm
{
    public string AlgorithmId => "MyVeryOwnAlgorithm";
    
    // implement methods...
}

public void Main()
{
    serviceCollection.AddDynamicPasswordHashing(o => {
        o.RegisterAlgorithm<MyInternalHashAlgorithm>();
    });
}
```

If you want to ONLY support your own algorithms, you can call `UseOnlyExternalAlgorithms()` on the IPasswordHashConfiguration. This will prevent loading in prepackaged algorithms. Lastly, if your custom algorithm is prefered and want all others to be considered obsolete, set it as such:

``` C#
public void Main()
{
    serviceCollection.AddDynamicPasswordHashing(o => {
        o.RegisterAlgorithm<MyInternalHashAlgorithm>()
        .SetPrefferedAlgorithm<MyInternalHashAlgorithm>();
    });
}
```

## Contribute

I hope this library is helpful and I'm open to other ideas. Feel free to fork and extend this library even more.

## License

MIT

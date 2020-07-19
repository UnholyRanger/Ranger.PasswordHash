# Ranger.PasswordHash
A Library to support swapping of password algorithms with ease. Protecting our systems by considering security is always a moving target and one part of this moving piece is ensuring our user's passwords are hashed securely. Overtime we find hashes to be weak, or systems get stronger calling for larger iterations. However, most applications are written to hash once and implement fall back logic when it needs to change..... over... and over... and over sometimes, again.
One company I worked at, we implemented a version of an agile based password hashing library. The original code had 3 levels of fallback testing. So in the event of an invalid password, it would go through 3 different hash checks. Now, they know which version is used, and an update to the library will update the hash algorithm.
This, is version 2.0. More versatile, and open for additions. It's mainly built around PBKDFv2, but you can build your own extension library to use other hash algorithms such as bcrypt or scrypt.

# Tech/Framework used
<b>Built with:</b>
- [Dotnet Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)

# Code Example
The main idea of the project is that applications will store the hashed password, salt, and algorithm ID in the database. This is acceptable from a security standpoint. When comparing a password, you will retrieve the salt/algorithm ID and pass that into the Agile algorithm. When setting a new password, you will pass the unhashed password with salt and then store the result.

### Validate Password
```
string passwordtext = "MySecurePassword";

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

var hashResponse = new AgilePasswordHash().Hash(passwordRequest);

if (hashResponse.Value == userDetails.PasswordHash)
{
	PasswordUpdate.UpdatePasswordAsync(passwordtext, passwordRequest);
	CompleteLogin();
}
else
	throw new PasswordException("Invalid username and password combination");
```

### Update Password
Since we have the plaintext password at the same time we know the algorithm is obsolete and it needs to be updated, we can do this behind the scenes from the user. They won't know we just made their password more secure.

```
public static async void UpdatePasswordAsync(string plaintext, PasswordHashRequest originalRequest) 
{
	originalRequest.AlgorithmId = null;
	var response = new AgilePasswordHash().Hash(originalRequest);
	
	// update the user details
	Database.UpdateUserCredentialsAsync(Password: response.Value, Salt: originalRequest.Salt, AlgorithmId: response.AlgorithmId);
}
```

### Build Your Own Algorithm
This library is extensible that you can build your own algorithms to use. Some cases, you may have a home grown algorithm, or are currently using one not supported by this library. Wrap it with our interface and register it, then update the database to have the ID you gave your algorithm and start using it!

```
public MyInternalHashAlgorithm: IPasswordHashAlgorithm
{
	public string AlgorithmId => "MyVeryOwnAlgorithm";
	public bool IsObsolete => false; // Do true if you want to auto-update
	
	// implement methods...
}

public void Main()
{
	PasswordHashRepo.Register(new MyInternalHashAlgorithm());
}
```

# Contribute
I hope this library is helpful and I'm open to other ideas. Feel free to fork and extend this library even more.

# License
MIT

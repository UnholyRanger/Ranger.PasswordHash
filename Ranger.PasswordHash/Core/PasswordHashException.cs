using System;

namespace Ranger.PasswordHash
{
	/// <summary>
	/// Thrown when a password cannot be hashed, such as when an algorithm is not
	/// registered or the hashing function fails.
	/// </summary>
	public class PasswordHashException : Exception
	{
		internal PasswordHashException(string message) : base(message) { }

		internal PasswordHashException(string message, Exception e) : base(message, e) { }
	}
}

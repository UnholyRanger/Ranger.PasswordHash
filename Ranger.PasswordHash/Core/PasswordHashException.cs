using System;

namespace Ranger.PasswordHash
{
	public class PasswordHashException : Exception
	{
		internal PasswordHashException(string message) : base(message) { }

		internal PasswordHashException(string message, Exception e) : base(message, e) { }
	}
}

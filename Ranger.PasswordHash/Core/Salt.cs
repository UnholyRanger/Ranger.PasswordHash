using System;
using System.Linq;

namespace Ranger.PasswordHash
{
	/// <summary>
	/// A model for working with Hash salts simple.
	/// </summary>
	/// <remarks>
	/// Store salts with <see cref="ToBase64"/> and load them with <see cref="FromBase64(string)"/>.
	/// Salts are random bytes, so they cannot safely round trip through a plain text encoding.
	/// </remarks>
	public class Salt
	{
		/// <summary>
		/// Gets the salt value as a <see cref="byte"/> array.
		/// </summary>
		public byte[] Value { get; }

		/// <summary>
		/// Constructs a <see cref="Salt"/> from a <see cref="byte"/> array value.
		/// </summary>
		/// <param name="salt">The salt bytes.</param>
		public Salt(byte[] salt)
			=> Value = salt.ToArray();

		/// <summary>
		/// Constructs a <see cref="Salt"/> from a base64 string, such as one produced by <see cref="ToBase64"/>.
		/// </summary>
		/// <param name="value">The base64 encoded salt.</param>
		/// <returns>The decoded salt.</returns>
		/// <exception cref="FormatException">The value is not valid base64.</exception>
		public static Salt FromBase64(string value)
			=> new(Convert.FromBase64String(value));

		/// <summary>
		/// Constructs a <see cref="Salt"/> from the UTF-8 bytes of a text value. Use this for
		/// salts that were stored as plain text, such as those used with version 2 of this library.
		/// </summary>
		/// <param name="value">The salt text.</param>
		/// <returns>The salt.</returns>
		public static Salt FromText(string value)
			=> new(value.ConvertToBytes());

		/// <summary>
		/// Gets the salt as a base64 string, suitable for storing.
		/// </summary>
		/// <returns>The base64 encoded salt.</returns>
		public string ToBase64()
			=> Value.ConvertTobase64();

		/// <summary>
		/// Gets the salt as a base64 string. Equivalent to <see cref="ToBase64"/>.
		/// </summary>
		/// <returns>The base64 encoded salt.</returns>
		public override string ToString()
			=> ToBase64();

		/// <summary>
		/// Implicitly converts a <see cref="byte"/> array to a <see cref="Salt"/>.
		/// </summary>
		/// <param name="value">The salt bytes.</param>
		public static implicit operator Salt(byte[] value)
			=> new(value);
	}
}

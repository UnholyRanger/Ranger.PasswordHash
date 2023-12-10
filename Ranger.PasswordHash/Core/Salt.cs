using System.Linq;

namespace Ranger.PasswordHash
{
    /// <summary>
    /// A model for working with Hash salts simple.
    /// </summary>
    public class Salt
	{
		/// <summary>
		/// Gets the salt value as a <see cref="byte[]"/>.
		/// </summary>
		public byte[] Value { get; private set; }

		/// <summary>
		/// Consturcts a <see cref="Salt"/> from a <see cref="string"/> value.
		/// </summary>
		/// <param name="salt"></param>
		public Salt(string salt)
			=> Value = salt.ConvertToBytes();

		/// <summary>
		/// Constructs a <see cref="Salt"/> from a <see cref="byte[]"/> value.
		/// </summary>
		/// <param name="salt"></param>
		public Salt(byte[] salt)
			=> Value = salt.ToArray();

		/// <inheritdoc/>
		public override string ToString()
			=> Value.ConvertToString();

		/// <summary>
		/// Implicitly converts a string to a <see cref="Salt"/>.
		/// </summary>
		/// <param name="value"></param>
		public static implicit operator Salt(string value)
            => new(value);

        /// <summary>
        /// Implicitly converts a <see cref="byte[]"/> to a <see cref="Salt"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Salt(byte[] value)
			=> new(value);

        /// <summary>
        /// Implicitly converts a <see cref="Salt"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator string(Salt salt)
			=> salt.ToString();
	}
}

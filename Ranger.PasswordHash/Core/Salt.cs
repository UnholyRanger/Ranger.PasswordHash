using System.Linq;

namespace Ranger.PasswordHash
{
	public class Salt
	{
		public byte[] Value { get; private set; }

		public Salt(string salt)
			=> Value = salt.ConvertToBytes();

		public Salt(byte[] salt)
			=> Value = salt.ToArray();

		public override string ToString()
			=> Value.ConvertToString();

		public static implicit operator Salt(string value)
			=> new Salt(value);

		public static implicit operator Salt(byte[] value)
			=> new Salt(value);

		public static implicit operator string(Salt salt)
			=> salt.Value.ConvertToString();
	}
}

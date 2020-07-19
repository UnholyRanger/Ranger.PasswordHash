namespace Ranger.PasswordHash
{
	public interface IPasswordHashAlgorithm
	{
		string AlgorithmId { get; }

		bool IsObsolete { get; set; }

		PasswordHashResponse Hash(string value, string salt);

		PasswordHashResponse Hash(byte[] value, byte[] salt);
	}
}

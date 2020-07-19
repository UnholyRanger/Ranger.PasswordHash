namespace Ranger.PasswordHash
{
	public class PasswordHashResponse
	{
		public string AlgorithmId { get; internal set; }

		public string Value { get; internal set; }

		public bool IsObsolete { get; internal set; }

		internal PasswordHashResponse() { }
	}
}

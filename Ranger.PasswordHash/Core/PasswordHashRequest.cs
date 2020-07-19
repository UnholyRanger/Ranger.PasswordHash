namespace Ranger.PasswordHash
{
	public class PasswordHashRequest
	{
		public string AlgorithmId { get; set; }

		public string Password { get; set; }

		public Salt Salt { get; set; }
	}
}

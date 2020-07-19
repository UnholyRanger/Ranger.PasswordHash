namespace Ranger.PasswordHash
{
	public interface IPasswordHash
	{
		PasswordHashResponse Hash(PasswordHashRequest request);
	}
}

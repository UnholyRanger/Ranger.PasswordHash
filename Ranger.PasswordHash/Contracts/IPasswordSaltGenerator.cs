namespace Ranger.PasswordHash
{
    /// <summary>
    /// An interface for generating a salt when none is
    /// provided for a password hash request.
    /// </summary>
    public interface IPasswordSaltGenerator
    {
        /// <summary>
        /// Generates a <see cref="Salt"/>.
        /// </summary>
        /// <returns></returns>
        Salt Generate();
    }
}

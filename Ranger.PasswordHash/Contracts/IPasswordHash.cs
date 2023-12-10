namespace Ranger.PasswordHash
{
    /// <summary>
    /// Interface for requesting a dynamic password hash.
    /// </summary>
    public interface IPasswordHash
    {
        /// <summary>
        /// Generates a password hash based on the <see cref="PasswordHashRequest"/>
        /// provided. If a <see cref="PasswordHashRequest.AlgorithmId"/> is provided, that
        /// hashing function will be used, else the preferred algorithm will be used.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IPasswordHashResult Hash(PasswordHashRequest request);
    }
}

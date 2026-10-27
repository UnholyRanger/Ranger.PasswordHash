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
        /// <param name="request">The password, and optionally the salt and algorithm, to hash.</param>
        /// <returns>The hash, the salt used, and whether the algorithm is obsolete.</returns>
        IPasswordHashResult Hash(PasswordHashRequest request);

        /// <summary>
        /// Verifies a password against a stored hash, using a fixed time comparison.
        /// </summary>
        /// <param name="password">The plain text password to check.</param>
        /// <param name="hash">The stored <see cref="IPasswordHashResult.Value"/>.</param>
        /// <param name="salt">The stored <see cref="IPasswordHashResult.Salt"/>.</param>
        /// <param name="algorithmId">The stored <see cref="IPasswordHashResult.AlgorithmId"/>.</param>
        /// <returns>Whether the password matches, and whether it should be rehashed.</returns>
        PasswordVerifyResult Verify(string password, string hash, Salt salt, string algorithmId);
    }
}

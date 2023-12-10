namespace Ranger.PasswordHash
{
    /// <summary>
    /// An interface for password hashing algoritms that
    /// can be used by the <see cref="IPasswordHash"/>.
    /// </summary>
    public interface IPasswordHashAlgorithm
    {
        /// <summary>
        /// Gets a value indicating the ID of the underlying algorithm.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Performs a hash based on the requested values.
        /// </summary>
        /// <param name="request">Requested data to be hashed.</param>
        /// <returns>Hash of the password.</returns>
        PasswordHashResult Hash(PasswordHashRequest request);
    }
}

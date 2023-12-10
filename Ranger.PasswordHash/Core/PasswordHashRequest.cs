namespace Ranger.PasswordHash
{
    /// <summary>
    /// A model used for making a hashing request.
    /// </summary>
    public record PasswordHashRequest
    {
        /// <summary>
        /// Gets the ID of the algorithm to use for the hashing. If no
        /// value is supplied, the Preferred algorithm will be used.
        /// </summary>
        public string? AlgorithmId { get; init; }

        /// <summary>
        /// Gets the password that will be hashed.
        /// </summary>
        public string Password { get; init; } = string.Empty;

        /// <summary>
        /// Gets the salt that will be used to hash the password. If no salt is supplied in the request,
        /// the system will generate one through the <see cref="IPasswordSaltGenerator"/>.
        /// </summary>
        public Salt? Salt { get; init; }
    }
}

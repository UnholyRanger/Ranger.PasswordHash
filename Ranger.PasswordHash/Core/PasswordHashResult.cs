namespace Ranger.PasswordHash
{
    /// <summary>
    /// A model for representing the response from an
    /// <see cref="IPasswordHash"/> request.
    /// </summary>
    public class PasswordHashResult : IPasswordHashResult
    {
        /// <inheritdoc/>
        public string AlgorithmId { get; init; } = string.Empty;

        /// <inheritdoc/>
        public string Value { get; init; } = string.Empty;

        /// <inheritdoc/>
        public Salt Salt { get; internal set; } = default!;

        /// <inheritdoc/>
        public bool IsObsolete { get; internal set; }
    }
}

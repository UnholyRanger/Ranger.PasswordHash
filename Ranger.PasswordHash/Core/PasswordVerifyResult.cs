namespace Ranger.PasswordHash
{
    /// <summary>
    /// The outcome of verifying a password with <see cref="IPasswordHash.Verify(string, string, Salt, string)"/>.
    /// </summary>
    public enum PasswordVerifyResult
    {
        /// <summary>
        /// The password does not match the stored hash.
        /// </summary>
        Failed,

        /// <summary>
        /// The password matches the stored hash.
        /// </summary>
        Success,

        /// <summary>
        /// The password matches, but the stored hash uses an algorithm other than the
        /// preferred one. Hash the password again and store the new result.
        /// </summary>
        SuccessRehashNeeded,
    }
}

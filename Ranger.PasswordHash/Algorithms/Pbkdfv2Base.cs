using System;
using System.Security.Cryptography;

namespace Ranger.PasswordHash.Algorithms
{
    /// <summary>
    /// A base implementation of the PBKDFv2 hashing algorithm.
    /// </summary>
    public abstract class Pbkdfv2Base : IPasswordHashAlgorithm
    {
        /// <inheritdoc/>
        public abstract string Id { get; }

        /// <summary>
        /// Gets a value indicating how many times to iterate through the
        /// PBKDF function.
        /// </summary>
        internal abstract int IterationCount { get; }

        /// <summary>
        /// Gets the HMAC hash function used by the PBKDF function. Defaults to SHA1,
        /// which matches the hashes produced by earlier versions of this library.
        /// </summary>
        internal virtual HashAlgorithmName HashAlgorithm => HashAlgorithmName.SHA1;

        /// <summary>
        /// Gets the number of bytes in the derived hash.
        /// </summary>
        internal virtual int HashLength => 20;

        /// <inheritdoc/>
        public PasswordHashResult Hash(PasswordHashRequest request)
            => PerformHash(request);

        /// <summary>
        /// Performs the RFC 2898 (PBKDFv2) hashing function.
        /// </summary>
        /// <param name="request">The password and salt to hash.</param>
        /// <returns>The hash result.</returns>
        /// <exception cref="PasswordHashException">The hash could not be computed.</exception>
        protected PasswordHashResult PerformHash(PasswordHashRequest request)
        {
            try
            {
                var hash = Rfc2898DeriveBytes.Pbkdf2(request.Password.ConvertToBytes(), request.Salt!.Value, IterationCount, HashAlgorithm, HashLength);
                return new PasswordHashResult
                {
                    AlgorithmId = Id,
                    Value = Convert.ToBase64String(hash),
                };
            }
            catch (Exception e)
            {
                throw new PasswordHashException("Error while performing a hash using PBKDFv2.", e);
            }
        }
    }
}

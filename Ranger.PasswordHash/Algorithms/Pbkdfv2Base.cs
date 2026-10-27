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

        /// <inheritdoc/>
        public PasswordHashResult Hash(PasswordHashRequest request)
            => PerformHash(request);

        /// <summary>
        /// Performs the RFC 2898 (PBKDFv2) hashing function.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="PasswordHashException"></exception>
        protected PasswordHashResult PerformHash(PasswordHashRequest request)
        {
            try
            {
                // SHA1 and a 20 byte output match the legacy Rfc2898DeriveBytes defaults,
                // so hashes produced by earlier versions still verify.
                var hash = Rfc2898DeriveBytes.Pbkdf2(request.Password.ConvertToBytes(), request.Salt!.Value, IterationCount, HashAlgorithmName.SHA1, 20);
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

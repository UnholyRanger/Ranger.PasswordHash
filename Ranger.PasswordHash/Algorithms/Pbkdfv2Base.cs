using System;
using System.Security.Cryptography;

namespace Ranger.PasswordHash
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
                using (var hasher = new Rfc2898DeriveBytes(request.Password.ConvertToBytes(), request.Salt.Value, IterationCount))
                {
                    return new PasswordHashResult
                    {
                        AlgorithmId = Id,
                        Value = Convert.ToBase64String(hasher.GetBytes(20)),
                    };
                }
            }
            catch (Exception e)
            {
                throw new PasswordHashException("Error while performing a hash using PBKDFv2.", e);
            }
        }
    }
}

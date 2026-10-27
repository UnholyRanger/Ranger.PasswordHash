using System.Security.Cryptography;

namespace Ranger.PasswordHash.Algorithms
{
    /// <summary>
    /// A hashing function for the PBKDF V2 using HMAC-SHA256 with 600,000 iterations
    /// and a 32 byte hash, following the OWASP password storage recommendation.
    /// </summary>
    public class PbkdfV2Sha256600k : Pbkdfv2Base
    {
        /// <inheritdoc/>
        public override string Id => @"PBKDFv2-SHA256-600k";

        /// <inheritdoc/>
        internal override int IterationCount => 600000;

        /// <inheritdoc/>
        internal override HashAlgorithmName HashAlgorithm => HashAlgorithmName.SHA256;

        /// <inheritdoc/>
        internal override int HashLength => 32;
    }
}

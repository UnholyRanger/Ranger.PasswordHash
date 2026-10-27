using System.Security.Cryptography;

namespace Ranger.PasswordHash
{
    internal class PasswordSaltGenerator : IPasswordSaltGenerator
    {
        /// <inheritdoc/>
        public Salt Generate()
            => new(RandomNumberGenerator.GetBytes(32));
    }
}

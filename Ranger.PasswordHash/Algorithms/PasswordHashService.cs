using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Ranger.PasswordHash
{
    internal class PasswordHashService : IPasswordHash
    {
        private readonly PasswordHashConfiguration _configuration;
        private readonly IPasswordSaltGenerator _saltGenerator;
        private readonly IReadOnlyList<IPasswordHashAlgorithm> _algorithms;

        private IPasswordHashAlgorithm? _preferredAlgorithm;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordHashService"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="saltGenerator"></param>
        /// <param name="algorithms"></param>
        public PasswordHashService(PasswordHashConfiguration configuration, IPasswordSaltGenerator saltGenerator, IEnumerable<IPasswordHashAlgorithm> algorithms)
        {
            _configuration = configuration;
            _saltGenerator = saltGenerator;
            _algorithms = algorithms.ToArray();

            if (_configuration.PreferedAlgorithm is not null && GetPrefferedAlgorithm() is null)
                throw new PasswordHashException($"The preferred algorithm ({_configuration.PreferedAlgorithm.Name}) is not registered. Register it with {nameof(IPasswordHashConfiguration.RegisterAlgorithm)}.");
        }

        /// <inheritdoc/>
        public IPasswordHashResult Hash(PasswordHashRequest request)
        {
            var algorithm = RetrieveAlgorithm(request.AlgorithmId)
                ?? throw new PasswordHashException($"Provided algorithm ID ({request.AlgorithmId}) did not result in a hash algorithm being found in the repo.");

            PasswordHashResult hashResult;
            if (request.Salt is null)
            {
                var salt = _saltGenerator.Generate();
                hashResult = algorithm.Hash(new PasswordHashRequest { AlgorithmId = request.AlgorithmId, Password = request.Password, Salt = salt });
                hashResult.Salt = salt;
            }
            else
            {
                hashResult = algorithm.Hash(request);
                hashResult.Salt = request.Salt;
            }

            hashResult.IsObsolete = GetPrefferedAlgorithm() is not null && GetPrefferedAlgorithm() != algorithm;

            return hashResult;
        }

        /// <inheritdoc/>
        public PasswordVerifyResult Verify(string password, string hash, Salt salt, string algorithmId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(algorithmId);

            var result = Hash(new PasswordHashRequest { AlgorithmId = algorithmId, Password = password, Salt = salt });

            if (!CryptographicOperations.FixedTimeEquals(result.Value.ConvertToBytes(), hash.ConvertToBytes()))
                return PasswordVerifyResult.Failed;

            return result.IsObsolete ? PasswordVerifyResult.SuccessRehashNeeded : PasswordVerifyResult.Success;
        }

        private IPasswordHashAlgorithm? RetrieveAlgorithm(string? algorithmId)
        {
            if (!string.IsNullOrWhiteSpace(algorithmId))
                return _algorithms.FirstOrDefault(a => a.Id == algorithmId);

            return GetPrefferedAlgorithm()
                ?? throw new PasswordHashException($"You need to register a preferred algorithm to be used when no algorithms are specifically requested.");
        }

        private IPasswordHashAlgorithm? GetPrefferedAlgorithm()
            => _preferredAlgorithm ??= _algorithms.FirstOrDefault(a => a.GetType() == _configuration.PreferedAlgorithm);
    }
}

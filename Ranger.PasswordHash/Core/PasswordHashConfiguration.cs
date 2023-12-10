using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ranger.PasswordHash.Core
{
    /// <summary>
    /// Hash configuration builder.
    /// </summary>
    internal class PasswordHashConfiguration : IPasswordHashConfiguration
    {
        private readonly IServiceCollection _services;

        internal PasswordHashConfiguration(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// Gets the type of the preferred hashing algorithm.
        /// </summary>
        internal Type? PreferedAlgorithm { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to load hash algorithms within this library.
        /// </summary>
        internal bool ExcludeInternalAlgorithms { get; private set; } = false;

        /// <inheritdoc/>
        public IPasswordHashConfiguration SetPrefferedAlgorithm<TIHashAlgorithm>()
            where TIHashAlgorithm : class, IPasswordHashAlgorithm
        {
            PreferedAlgorithm = typeof(TIHashAlgorithm);
            return this;
        }

        /// <inheritdoc/>
        public IPasswordHashConfiguration UseOnlyExternalAlgorithms()
        {
            ExcludeInternalAlgorithms = true;
            return this;
        }

        /// <inheritdoc/>
        public IPasswordHashConfiguration RegisterAlgorithm<TIHashAlgorithm>()
            where TIHashAlgorithm : class, IPasswordHashAlgorithm
        {
            _services.AddSingleton<IPasswordHashAlgorithm, TIHashAlgorithm>();
            return this;
        }
    }
}

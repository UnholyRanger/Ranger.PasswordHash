using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ranger.PasswordHash
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
        internal Type? PreferredAlgorithm { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to load hash algorithms within this library.
        /// </summary>
        internal bool ExcludeInternalAlgorithms { get; private set; } = false;

        /// <inheritdoc/>
        public IPasswordHashConfiguration SetPreferredAlgorithm<TIHashAlgorithm>()
            where TIHashAlgorithm : class, IPasswordHashAlgorithm
        {
            PreferredAlgorithm = typeof(TIHashAlgorithm);
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

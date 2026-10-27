using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ranger.PasswordHash.Algorithms;

namespace Ranger.PasswordHash
{
    /// <summary>
    /// Extensions for registering password hashing with an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class PasswordHashServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the required services for using the <see cref="IPasswordHash"/> that aids
        /// in dynamically hashing passwords. Use the options action to register any custom algorithms
        /// and override the default preferred algorithm.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="options">Optional configuration for registering algorithms and choosing the preferred one.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddDynamicPasswordHashing(this IServiceCollection services, Action<IPasswordHashConfiguration>? options = null)
        {
            var configuration = new PasswordHashConfiguration(services);

            options?.Invoke(configuration);

            services
                .AddSingleton<IPasswordHash>(sp => new PasswordHashService(configuration, sp.GetRequiredService<IPasswordSaltGenerator>(), sp.GetServices<IPasswordHashAlgorithm>()));

            services.TryAddSingleton<IPasswordSaltGenerator, PasswordSaltGenerator>();

            if (configuration.ExcludeInternalAlgorithms)
                return services;

            if (configuration.PreferredAlgorithm is null)
                configuration.SetPreferredAlgorithm<PbkdfV2Sha256600k>();

            return services
                .AddSingleton<IPasswordHashAlgorithm, Pbkdfv210k>()
                .AddSingleton<IPasswordHashAlgorithm, PbkdfV2610k>()
                .AddSingleton<IPasswordHashAlgorithm, PbkdfV2Sha256600k>()
            ;
        }
    }
}

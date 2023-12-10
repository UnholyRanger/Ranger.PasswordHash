using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ranger.PasswordHash.Algorithms;

namespace Ranger.PasswordHash.Core
{
    public static class PasswordHashServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the required services for using the <see cref="IPasswordHash"/> that will aids
        /// in dynamically hashing passwords. Use the options action to register any custom algorithms
        /// and override the default preferred algorithm.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddDynamicPasswordHashing(this IServiceCollection services, Action<IPasswordHashConfiguration>? options = null)
        {
            var configuration = new PasswordHashConfiguration(services);

            options?.Invoke(configuration);

            services
                .AddSingleton<IPasswordHash>(sp => new PasswordHashService(configuration, sp.GetRequiredService<IPasswordSaltGenerator>(), sp.GetServices<IPasswordHashAlgorithm>()));

            if (configuration.ExcludeInternalAlgorithms)
                return services;

            if (configuration.PreferedAlgorithm is null)
                configuration.SetPrefferedAlgorithm<PbkdfV2610k>();

            services.TryAddSingleton<IPasswordSaltGenerator, PasswordSaltGenerator>();

            return services
                .AddSingleton<IPasswordHashAlgorithm, Pbkdfv210k>()
                .AddSingleton<IPasswordHashAlgorithm, PbkdfV2610k>()
            ;
        }
    }
}

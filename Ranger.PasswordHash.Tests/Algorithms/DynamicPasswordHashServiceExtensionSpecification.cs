using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Ranger.PasswordHash.Algorithms;
using Ranger.PasswordHash.Core;
using Xunit;

namespace Ranger.PasswordHash.Tests.Algorithms
{
    public class DynamicPasswordHashServiceExtensionSpecification
    {
        private IServiceCollection _services;

        public DynamicPasswordHashServiceExtensionSpecification()
        {
            _services = new ServiceCollection();
        }

        [Fact(DisplayName = "WHEN registering dynamic password hashing service THEN services are registered.")]
        public void Validate_Algorithm_Registration()
        {
            // GIVEN registering services with the default configuration
            _services.AddDynamicPasswordHashing();

            // WHEN requesting algorithms
            var sp = _services.BuildServiceProvider();
            var algorithms = sp.GetServices<IPasswordHashAlgorithm>();

            // THEN the services are registered
            algorithms.Should()
                .ContainItemsAssignableTo<Pbkdfv210k>()
                .And.ContainItemsAssignableTo<PbkdfV2610k>();

            // AND the hashing service is registered
            sp.GetRequiredService<IPasswordHash>().Should().NotBeNull();
        }

        [Fact(DisplayName = "GIVEN dynamic password hashing WHEN requesting password hash service with option to exlcude internal hashes THEN services are not registered.")]
        public void Validate_Service_Registration_With_Action_NoInternalHashesLoaded()
        {
            // GIVEN registering services with the default configuration
            _services.AddDynamicPasswordHashing(o =>
            {
                o.UseOnlyExternalAlgorithms();
            });

            // WHEN requesting algorithms
            var sp = _services.BuildServiceProvider();
            var algorithms = sp.GetServices<IPasswordHashAlgorithm>();

            // THEN the services are registered
            algorithms.Should().BeEmpty();
        }

        [Fact(DisplayName = "GIVEN dynamic password hashing WHEN requesting password hash service with custom hash THEN services are registered.")]
        public void Validate_Service_Registration_With_Action_CustomAlgorithms()
        {
            // GIVEN registering services with the default configuration
            _services.AddDynamicPasswordHashing(o =>
            {
                o.UseOnlyExternalAlgorithms();
                o.RegisterAlgorithm<Pbkdfv210k>();
            });

            // WHEN requesting algorithms
            var sp = _services.BuildServiceProvider();
            var algorithms = sp.GetServices<IPasswordHashAlgorithm>();

            // THEN the services are registered
            algorithms.Should().HaveCount(1)
                .And.OnlyContain(a => a is Pbkdfv210k);
        }
    }
}

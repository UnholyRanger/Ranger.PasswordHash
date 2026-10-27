using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Ranger.PasswordHash.Algorithms;
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
                .And.ContainItemsAssignableTo<PbkdfV2610k>()
                .And.ContainItemsAssignableTo<PbkdfV2Sha256600k>();

            // AND the hashing service is registered
            sp.GetRequiredService<IPasswordHash>().Should().NotBeNull();
        }

        [Fact(DisplayName = "GIVEN the default configuration WHEN hashing without an algorithm ID THEN expect PBKDFv2-SHA256-600k to be used.")]
        public void Validate_Default_Preferred_Algorithm()
        {
            // GIVEN registering services with the default configuration
            _services.AddDynamicPasswordHashing();
            var hashService = _services.BuildServiceProvider().GetRequiredService<IPasswordHash>();

            // WHEN hashing without an algorithm ID
            var result = hashService.Hash(new PasswordHashRequest { Password = "MySecurePassword" });

            // THEN the SHA256 algorithm is used
            result.AlgorithmId.Should().Be("PBKDFv2-SHA256-600k");
            result.IsObsolete.Should().BeFalse();

            // AND the result verifies after storing the salt as base64
            hashService.Verify("MySecurePassword", result.Value, Salt.FromBase64(result.Salt.ToBase64()), result.AlgorithmId)
                .Should().Be(PasswordVerifyResult.Success);
        }

        [Fact(DisplayName = "GIVEN the obsolete misspelled preferred algorithm method WHEN hashing THEN expect it to still set the preferred algorithm.")]
        public void Validate_Obsolete_SetPrefferedAlgorithm_Alias()
        {
            // GIVEN the old method name
            _services.AddDynamicPasswordHashing(o =>
            {
#pragma warning disable CS0618 // Type or member is obsolete
                o.SetPrefferedAlgorithm<Pbkdfv210k>();
#pragma warning restore CS0618
            });
            var hashService = _services.BuildServiceProvider().GetRequiredService<IPasswordHash>();

            // WHEN hashing without an algorithm ID
            var result = hashService.Hash(new PasswordHashRequest { Password = "MySecurePassword" });

            // THEN the configured algorithm is used
            result.AlgorithmId.Should().Be("PBKDFv2-10k");
        }

        [Fact(DisplayName = "GIVEN a preferred algorithm that is not registered WHEN resolving the hashing service THEN expect an exception.")]
        public void Validate_Unregistered_Preferred_Algorithm_Throws()
        {
            // GIVEN a preferred algorithm without registering it
            _services.AddDynamicPasswordHashing(o =>
            {
                o.UseOnlyExternalAlgorithms();
                o.SetPreferredAlgorithm<Pbkdfv210k>();
            });

            // WHEN resolving the hashing service
            var action = () => _services.BuildServiceProvider().GetRequiredService<IPasswordHash>();

            // THEN an exception is thrown
            action.Should().Throw<PasswordHashException>();
        }

        [Fact(DisplayName = "GIVEN dynamic password hashing WHEN requesting password hash service with option to exclude internal hashes THEN services are not registered.")]
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

            // AND the hashing service can be resolved with the default salt generator
            sp.GetRequiredService<IPasswordHash>().Should().NotBeNull();
        }
    }
}

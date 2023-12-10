using FluentAssertions;
using Moq;
using Ranger.PasswordHash.Algorithms;
using Ranger.PasswordHash.Core;
using Ranger.PasswordHash.Tests.TestAlgorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Xunit;

namespace Ranger.PasswordHash.Tests.Algorithms
{
    public class PasswordHashServiceSpecification
    {
        private readonly Mock<IPasswordSaltGenerator> _mockSaltGenerator = new();

        public PasswordHashServiceSpecification()
        {
            _mockSaltGenerator.Setup(g => g.Generate()).Returns(new Salt(Array.Empty<byte>()));
        }

        [Fact(DisplayName = "GIVEN algorithms are configured WHEN requesting a hash with an ID THEN expect that hash used.")]
        public void Given_RegisteredAlgorithms_WHEN_RequestingExisting_Expect_ThatAlgorithm()
        {
            // GIVEN a default configuration
            var configuration = new PasswordHashConfiguration(null);

            // AND registered algorithms
            var mocks = Enumerable.Range(0, 5).Select(v => {
                var id = $"Hash_{v}";
                var m = new Mock<IPasswordHashAlgorithm>();
                m.Setup(a => a.Id).Returns(id);
                m.Setup(a => a.Hash(It.IsAny<PasswordHashRequest>())).Returns(new PasswordHashResult { AlgorithmId = id, Value = string.Empty });
                return m;
            }).ToArray();

            var serviceUnderTest = GetTestService(configuration, mocks.Select(m => m.Object));

            // WHEN requesting a hash with a specific ID
            const string id = $"Hash_3";
            var request = new PasswordHashRequest() { AlgorithmId = id, Salt = new Salt(string.Empty) };

            var result = serviceUnderTest.Hash(request);

            // THEN expect that hash to be used
            for(int i = 0; i < 5; i++)
            {
                if (i == 3)
                    mocks[i].Verify(m => m.Hash(request), Times.Once);
                else
                {
                    mocks[i].Verify(m => m.Id, Times.AtMost(1));
                    mocks[i].VerifyNoOtherCalls();
                }
            }

            // AND not to be obsolete
            result.IsObsolete.Should().BeFalse();
        }

        [Fact(DisplayName = "GIVEN algorithms are configured AND a preffered alrogithm is set WHEN requesting a hash without an ID THEN expect preferred algorithm used.")]
        public void Given_RegisteredAlgorithms_WHEN_RequestingNonExistingSpecific_Expect_PreferredAlgorithm()
        {
            // GIVEN a default configuration
            var configuration = new PasswordHashConfiguration(null);

            // AND registered algorithms
            var mockOtherAlgorithm = new Mock<IPasswordHashAlgorithm>();
            var algorithms = new IPasswordHashAlgorithm[] { new PreferredAlgorithm(), mockOtherAlgorithm.Object };

            // AND a preferred algorithm
            configuration.SetPrefferedAlgorithm<PreferredAlgorithm>();

            var serviceUnderTest = GetTestService(configuration, algorithms);

            // WHEN requesting a hash
            var result = serviceUnderTest.Hash(new PasswordHashRequest());

            // THEN the preferred algorithm should be used
            result.Value.Should().Be(nameof(PreferredAlgorithm));

            // AND the other algorithms should not be called
            mockOtherAlgorithm.VerifyNoOtherCalls();

        }

        [Fact(DisplayName = "GIVEN algorithms are configured AND a preffered alrogithm is set WHEN requesting a hash with an ID THEN expect algorithm used AND result is marked obsolete.")]
        public void Given_RegisteredAlgorithms_WHEN_RequestingExistingSpecific_Expect_ResultObsolete()
        {
            const string requestedAlgorithmId = "Mock_Algorithm";

            // GIVEN a default configuration
            var configuration = new PasswordHashConfiguration(null);

            // AND registered algorithms
            var mockOtherAlgorithm = new Mock<IPasswordHashAlgorithm>();
            mockOtherAlgorithm.Setup(a => a.Id).Returns(requestedAlgorithmId);
            mockOtherAlgorithm.Setup(a => a.Hash(It.Is<PasswordHashRequest>(r => r.AlgorithmId == requestedAlgorithmId)))
                .Returns(new PasswordHashResult { AlgorithmId = requestedAlgorithmId, Value = requestedAlgorithmId});

            var algorithms = new IPasswordHashAlgorithm[] { new PreferredAlgorithm(), mockOtherAlgorithm.Object };

            // AND a preferred algorithm
            configuration.SetPrefferedAlgorithm<PreferredAlgorithm>();

            var serviceUnderTest = GetTestService(configuration, algorithms);

            // WHEN requesting a hash
            var result = serviceUnderTest.Hash(new PasswordHashRequest() { AlgorithmId = requestedAlgorithmId });

            // THEN the preferred algorithm should be used
            result.AlgorithmId.Should().Be(requestedAlgorithmId);
            result.Value.Should().Be(requestedAlgorithmId);

            // AND the result is marked obsolete
            result.IsObsolete.Should().BeTrue();
        }

        [Fact(DisplayName = "GIVEN no salt is defined in the request WHEN requesting a has THEN epect one to be generated and returned.")]
        public void Given_NoSaltProvided_Expect_GenerateOne()
        {
            // GIVEN a default configuration
            var configuration = new PasswordHashConfiguration(null);

            // AND registered algorithms
            var algorithms = new IPasswordHashAlgorithm[] { new PreferredAlgorithm() };

            // AND no salt defined
            var request = new PasswordHashRequest { AlgorithmId = "Preferred-Algorithm", Password = "Empty", Salt = null };

            // AND the generator will be called
            var generatedBytes = RandomNumberGenerator.GetBytes(20);
            _mockSaltGenerator.Setup(g => g.Generate()).Returns(generatedBytes);

            var serviceUnderTest = GetTestService(configuration, algorithms);

            // WHEN requesting a hash
            var result = serviceUnderTest.Hash(request);

            // THEN the generator was called
            _mockSaltGenerator.Verify(g => g.Generate(), Times.Once());
            _mockSaltGenerator.VerifyNoOtherCalls();

            // AND the salt is returned
            result.Salt.Value.Should().BeEquivalentTo(generatedBytes);
        }

        [Fact(DisplayName = "GIVEN no algorithms are configured WHEN requesting a hash THEN expect an exception to be thrown.")]
        public void Given_NoAlgorithms_Expect_Exception()
        {
            // GIVEN a default configuration
            var configuration = new PasswordHashConfiguration(null);

            // AND no registered algorithms
            var algorithms = Array.Empty<IPasswordHashAlgorithm>();

            var serviceUnderTest = GetTestService(configuration, algorithms);

            // WHEN requesting a hash
            var action = () => serviceUnderTest.Hash(new PasswordHashRequest());
            
            // THEN an exception should be thrown
            var exception = action.Should().Throw<PasswordHashException>()
                .WithMessage("You need to register a preferred algorithm to be used when no algorithms are specifically requested.");
            
        }

        [Fact(DisplayName = "GIVEN no algorithms are configured AND a preffered alrogithm is set WHEN requesting a hash THEN expect an exception to be thrown.")]
        public void Given_NoAlgorithms_HasPreferred_Expect_Exception()
        {
            // GIVEN a default configuration
            var configuration = new PasswordHashConfiguration(null);

            // AND a preferred algorithm
            configuration.SetPrefferedAlgorithm<Pbkdfv210k>();

            // AND no registered algorithms
            var algorithms = Array.Empty<IPasswordHashAlgorithm>();

            var serviceUnderTest = GetTestService(configuration, algorithms);

            // WHEN requesting a hash
            var action = () => serviceUnderTest.Hash(new PasswordHashRequest());

            // THEN an exception should be thrown
            var exception = action.Should().Throw<PasswordHashException>()
                .WithMessage("You need to register a preferred algorithm to be used when no algorithms are specifically requested.");

        }

        private PasswordHashService GetTestService(PasswordHashConfiguration configuration, IEnumerable<IPasswordHashAlgorithm> algorithms)
            => new(configuration, _mockSaltGenerator.Object, algorithms);
    }
}

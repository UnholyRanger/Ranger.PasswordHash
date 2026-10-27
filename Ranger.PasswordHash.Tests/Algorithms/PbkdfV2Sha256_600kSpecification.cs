using FluentAssertions;
using Ranger.PasswordHash.Algorithms;
using System.Security.Cryptography;
using Xunit;

namespace Ranger.PasswordHash.Tests.Algorithms
{
    public class PbkdfV2Sha256_600kSpecification
	{
		private const string expectId = "PBKDFv2-SHA256-600k";

		// GIVEN the PBKDFv2 using SHA256 with 600k iterations
		private static readonly PbkdfV2Sha256600k algorithmUndertest = new();

		[Fact(DisplayName = "GIVEN a test algorithm WHEN checking the ID EXPECT proper value.")]
		public void When_GivenAlgorithm_Expect_ValidProperties()
		{
			algorithmUndertest.Id.Should().Be(expectId);
			algorithmUndertest.IterationCount.Should().Be(600000);
			algorithmUndertest.HashAlgorithm.Should().Be(HashAlgorithmName.SHA256);
			algorithmUndertest.HashLength.Should().Be(32);
		}

		[Theory(DisplayName = "GIVEN values to be hash WHEN hashing EXPECT all to be as expected.")]
		// Python: base64(hashlib.pbkdf2_hmac('sha256', password, salt, 600000, 32))
		[InlineData("ThisIs$up3rSecure", "SecureSaltValue", "yyJ8+7vqiYysFAQ6zlqoHwNkXGSWQ4bcdy/rO5oWsXs=")]
		[InlineData("SmallValue", "SuperDuper12345Salt", "qfN18XVdGo7BRH3FUViT2+uO9ca02N1qFMxDnAUzohk=")]
		public void When_GivenValueAndSalt_Expect_ValidHashValue(string value, string salt, string result)
		{
			// GIVEN a hashing request
			var request = new PasswordHashRequest
			{
				Password = value,
				Salt = Salt.FromText(salt),
			};

			// WHEN hashing
			var response = algorithmUndertest.Hash(request);

			// THEN the values are as expected
			response.AlgorithmId.Should().Be(expectId);
			response.Value.Should().Be(result);
		}
	}
}

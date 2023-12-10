using FluentAssertions;
using Ranger.PasswordHash.Algorithms;
using Xunit;

namespace Ranger.PasswordHash.Tests.Algorithms
{
    public class PbkdfV2_10kSpecification
	{
		private const string expectId = "PBKDFv2-10k";

		// GIVEN the PBKDFv2 with 10k iterations
		private static readonly Pbkdfv210k algorithmUndertest = new();

		[Fact(DisplayName = "GIVEN a test algorithm WHEN checking the ID EXPECT proper value.")]
		public void When_GivenAlgorithm_Expect_ValidProperties()
		{
			algorithmUndertest.Id.Should().Be(expectId);
			algorithmUndertest.IterationCount.Should().Be(10000);
		}

		[Fact(DisplayName = "GIVEN a hashing request WHEN hashing EXPECT result.")]
		public void When_GivenHashRequest_Expect_ResultHasValidProperties()
		{
			// GIVEN a hashing request
			var request = new PasswordHashRequest
			{
				Password = string.Empty,
				Salt = new string('s', 8)
			};

			// WHEN hashing
			var response = algorithmUndertest.Hash(request);

			// THEN values are as expected
			response.AlgorithmId.Should().Be(expectId);
			response.Value.Should().NotBeNull();
		}

		[Theory (DisplayName = "GIVEN values to be hash WHEN hashing EXPECT all to be as expected.")]
		// http://anandam.name/pbkdf2/
		[InlineData("ThisIs$up3rSecure", "SecureSaltValue", "LmEnvDWVxI+1i9RTWAlU+S2NucY=")]
		[InlineData("SmallValue", "12345SuperSaltedPassword6789", "VM8VURiwYvx1cj7UWGZhH0xXZMw=")]
		// https://asecuritysite.com/encryption/PBKDF2z
		[InlineData("Pa$$wordM@nag3r", "SecureSaltValue", "erVypei7sTqO/HzdoqAT8QiyB2M=")]
		[InlineData("SmallValue", "SuperDuper12345Salt", "BDPaAsT+axAYSTw5J9cKLpUH1vo=")]
		public void When_GivenValueAndSalt_Expect_ValidHashValue(string value, string salt, string result)
		{
			// GIVEN a hashing request
			var request = new PasswordHashRequest
			{
				Password = value,
				Salt = salt,
			};

			// WHEN hashing
			var response = algorithmUndertest.Hash(request);

			// THEN the values are as expected
			response.Value.Should().Be(result);
		}
	}
}

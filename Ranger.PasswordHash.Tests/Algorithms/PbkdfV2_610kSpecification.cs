using FluentAssertions;
using Ranger.PasswordHash.Algorithms;
using Xunit;

namespace Ranger.PasswordHash.Tests.Algorithms
{
    public class PbkdfV2_610kSpecification
	{
		private const string expectId = "PBKDFv2-610k";

		// GIVEN the PBKDFv2 with 10k iterations
		private static readonly PbkdfV2610k algorithmUndertest = new();

		[Fact(DisplayName = "GIVEN a test algorithm WHEN checking the ID EXPECT proper value.")]
		public void When_GivenAlgorithm_Expect_ValidProperties()
		{
			algorithmUndertest.Id.Should().Be(expectId);
			algorithmUndertest.IterationCount.Should().Be(610000);
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
	}
}

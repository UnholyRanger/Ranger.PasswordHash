using NUnit.Framework;

namespace Ranger.PasswordHash.Tests.Algorithms
{
	[TestFixture]
	public class PbkdfV2_1000Tests
	{
		private const string expectId = "PBKDFv2-10k";
		private const bool expectedObsolete = false;

		private static readonly IPasswordHashAlgorithm algorithmUndertest = new Pbkdfv210k();

		[Test]
		public void When_GivenAlgorithm_Expect_ValidProperties()
		{
			Assert.AreEqual(algorithmUndertest.AlgorithmId, expectId);
			Assert.AreEqual(algorithmUndertest.IsObsolete, expectedObsolete);
		}

		[Test]
		public void When_GivenHashRequest_Expect_ResultHasValidProperties()
		{
			var request = new PasswordHashRequest
			{
				Password = string.Empty,
				Salt = new string('s', 8)
			};

			var response = ((IPasswordHash)algorithmUndertest).Hash(request);

			Assert.AreEqual(response.AlgorithmId, expectId);
			Assert.AreEqual(response.IsObsolete, expectedObsolete);
			Assert.IsNotNull(response.Value);
		}

		[Test]
		// http://anandam.name/pbkdf2/
		[TestCase("ThisIs$up3rSecure", "SecureSaltValue", "LmEnvDWVxI+1i9RTWAlU+S2NucY=")]
		[TestCase("SmallValue", "12345SuperSaltedPassword6789", "VM8VURiwYvx1cj7UWGZhH0xXZMw=")]
		// https://asecuritysite.com/encryption/PBKDF2z
		[TestCase("Pa$$wordM@nag3r", "SecureSaltValue", "erVypei7sTqO/HzdoqAT8QiyB2M=")]
		[TestCase("SmallValue", "SuperDuper12345Salt", "BDPaAsT+axAYSTw5J9cKLpUH1vo=")]
		public void When_GivenValueAndSalt_Expect_ValidHashValue(string value, string salt, string result)
		{
			var response = algorithmUndertest.Hash(value, salt);

			Assert.AreEqual(response.Value, result);
		}
	}
}

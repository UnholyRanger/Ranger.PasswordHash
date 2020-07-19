using NUnit.Framework;
using System.Text;

namespace Ranger.PasswordHash.Tests.Core
{
	[TestFixture]
	public class SaltTest
	{
		private const string testSaltString = "SaltedStringValue";

		[Test]
		public void When_SaltProvided_Expect_CanConvertStringAndByteArray()
		{
			Salt saltString = new Salt(testSaltString);
			Salt saltBytes = new Salt(Encoding.UTF8.GetBytes(saltString));

			Assert.AreNotSame(saltString, saltBytes);
			Assert.AreEqual(saltString.ToString(), testSaltString);
			Assert.AreEqual(saltBytes.ToString(), testSaltString);
			Assert.AreEqual(saltBytes.Value, saltString.Value);
		}

		[Test]
		public void When_SaltProvidedAsString_Expect_CanCast()
		{
			Salt salt = testSaltString;

			Assert.NotNull(salt);
			Assert.AreEqual(testSaltString, salt.ToString());
		}

		[Test]
		public void When_SaltProvidedAsByteArray_Expect_CanCast()
		{
			var mySalt = Encoding.UTF8.GetBytes(testSaltString);

			Salt salt = mySalt;

			Assert.NotNull(salt);
			Assert.AreEqual(testSaltString, salt.ToString());
			Assert.AreEqual(mySalt, salt.Value);
			Assert.AreNotSame(mySalt, salt.Value);
		}	
	}
}

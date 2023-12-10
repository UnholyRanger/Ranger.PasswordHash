using FluentAssertions;
using System.Text;
using Xunit;

namespace Ranger.PasswordHash.Tests.Core
{
    public class SaltSpecification
    {
        // GIVEN a test string for salting
        private const string testSaltString = "SaltedStringValue";

        [Fact(DisplayName = "GIVEN a salt string WHEN converting to a salt THEN expect the string any byte array to be equivalent.")]
        public void When_SaltProvided_Expect_CanConvertStringAndByteArray()
        {
            // GIVEN a salt from a string
            var saltString = new Salt(testSaltString);

            // AND a salt from the bytes of that string
            var saltBytes = new Salt(Encoding.UTF8.GetBytes(saltString));

            // THEN they should be equivalent
            saltString.Should().BeEquivalentTo(saltBytes);

            // AND their values should be the same
            saltString.ToString().Should().Be(testSaltString);
            saltBytes.ToString().Should().Be(testSaltString);
            saltBytes.Value.Should().BeEquivalentTo(saltString.Value);
        }

        [Fact(DisplayName = "GIVEN a salt string WHEN implicitly converting THEN still matches string.")]
        public void When_SaltProvidedAsString_Expect_CanCast()
        {
            // WHEN implicitly converting to a salt
            Salt salt = testSaltString;

            // THEN the conversion works
            salt.Should().NotBeNull();
            
            // AND the values are equivalent
            salt.ToString().Should().Be(testSaltString);
        }

        [Fact(DisplayName = "GIVEN a byte array WHEN implicitly converting to a salt THEN expect the string and byte array to be equivalent.")]
        public void When_SaltProvidedAsByteArray_Expect_CanCast()
        {
            // GIVEN a byte array for a salt
            var mySalt = Encoding.UTF8.GetBytes(testSaltString);

            // WHEN implicitly converting to a salt
            Salt salt = mySalt;

            // THEN the conversion works
            salt.Should().NotBeNull();

            // AND the values are equivalent
            salt.ToString().Should().Be(testSaltString);
            salt.Value.Should().BeEquivalentTo(mySalt);
        }	
    }
}

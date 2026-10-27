using FluentAssertions;
using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Ranger.PasswordHash.Tests.Core
{
    public class SaltSpecification
    {
        // GIVEN a test string for salting
        private const string testSaltString = "SaltedStringValue";

        [Fact(DisplayName = "GIVEN a salt text WHEN converting to a salt THEN expect the text and byte array to be equivalent.")]
        public void When_SaltProvided_Expect_CanConvertTextAndByteArray()
        {
            // GIVEN a salt from a text value
            var saltText = Salt.FromText(testSaltString);

            // AND a salt from the bytes of that text
            var saltBytes = new Salt(Encoding.UTF8.GetBytes(testSaltString));

            // THEN their values should be the same
            saltText.Value.Should().BeEquivalentTo(saltBytes.Value);
            saltText.ToBase64().Should().Be(saltBytes.ToBase64());
        }

        [Fact(DisplayName = "GIVEN random salt bytes WHEN round tripping through base64 THEN expect the same bytes.")]
        public void When_RandomSaltRoundTripped_Expect_SameBytes()
        {
            // GIVEN random bytes, like the salt generator produces
            var bytes = RandomNumberGenerator.GetBytes(32);
            var salt = new Salt(bytes);

            // WHEN storing and loading the salt
            var stored = salt.ToBase64();
            var loaded = Salt.FromBase64(stored);

            // THEN the bytes are unchanged
            loaded.Value.Should().Equal(bytes);

            // AND ToString matches the stored value
            salt.ToString().Should().Be(stored);
        }

        [Fact(DisplayName = "GIVEN an invalid base64 value WHEN loading a salt THEN expect a format exception.")]
        public void When_InvalidBase64_Expect_FormatException()
        {
            var action = () => Salt.FromBase64("not base64!");

            action.Should().Throw<FormatException>();
        }

        [Fact(DisplayName = "GIVEN a byte array WHEN implicitly converting to a salt THEN expect the byte array to be equivalent.")]
        public void When_SaltProvidedAsByteArray_Expect_CanCast()
        {
            // GIVEN a byte array for a salt
            var mySalt = Encoding.UTF8.GetBytes(testSaltString);

            // WHEN implicitly converting to a salt
            Salt salt = mySalt;

            // THEN the conversion works
            salt.Should().NotBeNull();

            // AND the values are equivalent
            salt.Value.Should().Equal(mySalt);
        }
    }
}

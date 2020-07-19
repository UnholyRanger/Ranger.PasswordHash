using Moq;
using NUnit.Framework;
using System.Linq;

namespace Ranger.PasswordHash.Tests.Core
{
	[TestFixture]
	public class PasswordAlgorithmRepoTest
	{
		private IPasswordHashAlgorithm algorithm;
		private const string MockAlgorithmId = "Mockv1-100";

		[OneTimeSetUp]
		public void Setup()
		{
			var mockAlgorithm = new Mock<IPasswordHashAlgorithm>();
			mockAlgorithm.SetupGet(a => a.AlgorithmId).Returns(MockAlgorithmId);
			mockAlgorithm.SetupGet(a => a.IsObsolete).Returns(true);

			algorithm = mockAlgorithm.Object;
		}

		[Test]
		public void When_DeregisteringNonExistingId_Expect_NoError()
		{
			Assert.DoesNotThrow(() => PasswordAlgorithmRepo.DeregisterAlgorithm("RandomId"));
		}

		[Test]
		public void When_SearchingEmptyValue_Expect_NoError()
		{
			Assert.IsTrue(PasswordAlgorithmRepo.Search("*").Any());
			Assert.IsFalse(PasswordAlgorithmRepo.Search(null).Any());
			Assert.IsFalse(PasswordAlgorithmRepo.Search(string.Empty).Any());
		}

		/* Due to a Static repo, tests are ordered when modifying the repo */

		[Test]
		[Order(1)]
		public void When_EmptyRepo_Expect_AlgorithmsRegistered()
		{
			Assert.IsTrue(PasswordAlgorithmRepo.Search("*").Any());
		}

		[Test]
		[Order(1)]
		public void When_SearchPbkdf_Expect_AlgorithmFound()
		{
			var collection = PasswordAlgorithmRepo.Search("pbkdfv2-*");

			Assert.IsTrue(collection.Any());
			Assert.IsTrue(collection.All(a => a.AlgorithmId.StartsWith("PBKDFv2")));
		}

		[Test]
		[Order(2)]
		public void When_AddingAlgorithm_Expect_ShowsInSearch()
		{
			PasswordAlgorithmRepo.RegisterAlgorithm(algorithm);

			var collection = PasswordAlgorithmRepo.Search("*", true);

			var foundAlgorithm = collection.FirstOrDefault(a => a.AlgorithmId == algorithm.AlgorithmId);

			Assert.AreSame(algorithm, foundAlgorithm);
		}

		[Test]
		[Order(2)]
		public void When_SearchingForNonObsoleteAlgorithm_Expect_ExcludedObsoleteOnes()
		{
			PasswordAlgorithmRepo.RegisterAlgorithm(algorithm);

			var collection = PasswordAlgorithmRepo.Search("*", false);

			Assert.IsTrue(collection.Any());
			Assert.IsFalse(collection.Any(a => a.AlgorithmId == algorithm.AlgorithmId));
		}

		[Test]
		[Order(2)]
		public void When_DeregisteringAlgorithm_Expect_NoLongerSearchable()
		{
			PasswordAlgorithmRepo.RegisterAlgorithm(algorithm);

			var myAlgorithm = PasswordAlgorithmRepo.Get(algorithm.AlgorithmId);

			Assert.IsNotNull(myAlgorithm);
			Assert.AreSame(algorithm, myAlgorithm);

			PasswordAlgorithmRepo.DeregisterAlgorithm(algorithm.AlgorithmId);

			Assert.Throws<PasswordHashException>(() => PasswordAlgorithmRepo.Get(algorithm.AlgorithmId), $"The given algorithm ID '{algorithm.AlgorithmId}' was not found in the repo.");
			Assert.IsFalse(PasswordAlgorithmRepo.Search(algorithm.AlgorithmId, true).Any());
		}
	}
}

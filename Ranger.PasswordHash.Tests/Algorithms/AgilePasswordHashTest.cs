using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Ranger.PasswordHash.Tests.Algorithms
{
	[TestFixture]
	public class AgilePasswordHashTest
	{
		IReadOnlyCollection<IPasswordHashAlgorithm> originalAlgorithms;

		private IPasswordHashAlgorithm currentAlgorithm;
		private IPasswordHashAlgorithm newAlgorithm;

		private const string CurrentHashValue = "CurrentHashValue";
		private const string NewHashValue = "NewHashValue";

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			originalAlgorithms = PasswordAlgorithmRepo.Search("*", true);
			foreach (var algorithm in originalAlgorithms)
				PasswordAlgorithmRepo.DeregisterAlgorithm(algorithm.AlgorithmId);

			// current algorithm Values
			const string currentAlgorithmId = "MockCurrent";
			const bool currentIsObsolete = true; 

			var currentMockAlgorithm = new Mock<IPasswordHashAlgorithm>();
			currentMockAlgorithm.SetupGet(m => m.AlgorithmId).Returns(currentAlgorithmId);
			currentMockAlgorithm.SetupGet(m => m.IsObsolete).Returns(currentIsObsolete);
			currentMockAlgorithm.Setup(m => m.Hash(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(new PasswordHashResponse
				{
					AlgorithmId = currentAlgorithmId,
					IsObsolete = currentIsObsolete,
					Value = CurrentHashValue
				});

			currentAlgorithm = currentMockAlgorithm.Object;
			PasswordAlgorithmRepo.RegisterAlgorithm(currentAlgorithm);

			// new algorithm values
			const string newAlgorithmId = "MockNew";
			const bool newAlgorithmObsolete = false;

			var newMockAlgorithm = new Mock<IPasswordHashAlgorithm>();
			newMockAlgorithm.SetupGet(m => m.AlgorithmId).Returns(newAlgorithmId);
			newMockAlgorithm.SetupGet(m => m.IsObsolete).Returns(newAlgorithmObsolete);
			newMockAlgorithm.Setup(m => m.Hash(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(new PasswordHashResponse
				{
					AlgorithmId = newAlgorithmId,
					IsObsolete = newAlgorithmObsolete,
					Value = NewHashValue
				});

			newAlgorithm = newMockAlgorithm.Object;
			PasswordAlgorithmRepo.RegisterAlgorithm(newAlgorithm);
		}

		[OneTimeTearDown]
		public void OneTimeCleanup()
		{
			foreach (var algorithm in PasswordAlgorithmRepo.Search("*", true))
				PasswordAlgorithmRepo.DeregisterAlgorithm(algorithm.AlgorithmId);

			foreach (var algorithm in originalAlgorithms)
				PasswordAlgorithmRepo.RegisterAlgorithm(algorithm);
		}

		[Test]
		public void When_ProvidingCurrentAlgorithm_Expect_CurrentAlgorithmReturned()
		{
			var request = new PasswordHashRequest
			{
				AlgorithmId = currentAlgorithm.AlgorithmId,
				Password = "MyPassword",
				Salt = "MySalt"
			};

			var response = new AgilePasswordHash().Hash(request);

			Assert.AreEqual(response.AlgorithmId, currentAlgorithm.AlgorithmId);
			Assert.AreEqual(response.IsObsolete, currentAlgorithm.IsObsolete);
			Assert.AreEqual(response.Value, CurrentHashValue);
		}

		[Test]
		public void When_ProvidingNoAlgorithmId_Expect_NewAlgorithmUsed()
		{
			var request = new PasswordHashRequest
			{
				Password = "MyPassword",
				Salt = "MySalt"
			};

			var response = new AgilePasswordHash().Hash(request);

			Assert.AreEqual(response.AlgorithmId, newAlgorithm.AlgorithmId);
			Assert.AreEqual(response.IsObsolete, newAlgorithm.IsObsolete);
			Assert.AreEqual(response.Value, NewHashValue);
		}

		[Test]
		public void When_CurrentIsObsolete_Expect_SecondRequestUseNewAlgorithm()
		{
			var request = new PasswordHashRequest
			{
				AlgorithmId = currentAlgorithm.AlgorithmId,
				Password = "MyPassword",
				Salt = "MySalt"
			};

			var agileHash = new AgilePasswordHash();

			var response = agileHash.Hash(request);

			Assert.AreEqual(response.AlgorithmId, currentAlgorithm.AlgorithmId);
			Assert.IsTrue(response.IsObsolete);

			request.AlgorithmId = null;

			response = agileHash.Hash(request);

			Assert.AreEqual(response.AlgorithmId, newAlgorithm.AlgorithmId);
			Assert.IsFalse(response.IsObsolete);
		}

		[Test]
		public void When_AdditionalObsoleteAlgorithmIsGiven_Expect_NotUsed()
		{
			const string tmpAlgorithmId = "TmpAlgorithm";
			const bool tmpAlgorithmObsolete = true;

			var tmpAlgorithm = new Mock<IPasswordHashAlgorithm>();
			tmpAlgorithm.SetupGet(m => m.AlgorithmId).Returns(tmpAlgorithmId);
			tmpAlgorithm.SetupGet(m => m.IsObsolete).Returns(tmpAlgorithmObsolete);
			tmpAlgorithm.Setup(m => m.Hash(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(new PasswordHashResponse
				{
					AlgorithmId = tmpAlgorithmId,
					IsObsolete = tmpAlgorithmObsolete,
					Value = CurrentHashValue
				});

			PasswordAlgorithmRepo.RegisterAlgorithm(tmpAlgorithm.Object);

			var passwordRequest = new PasswordHashRequest
			{
				AlgorithmId = currentAlgorithm.AlgorithmId,
				Password = "Tmp",
				Salt = "Tmp"
			};

			var agileHash = new AgilePasswordHash();

			var response = agileHash.Hash(passwordRequest);

			Assert.AreEqual(response.AlgorithmId, currentAlgorithm.AlgorithmId);

			passwordRequest.AlgorithmId = null;

			response = agileHash.Hash(passwordRequest);

			Assert.AreEqual(response.AlgorithmId, newAlgorithm.AlgorithmId);

			PasswordAlgorithmRepo.DeregisterAlgorithm(tmpAlgorithmId);
		}
	}
}

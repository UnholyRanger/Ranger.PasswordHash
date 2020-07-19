using System;
using System.Linq;

namespace Ranger.PasswordHash
{
	public class AgilePasswordHash : IPasswordHash
	{
		public PasswordHashResponse Hash(PasswordHashRequest request)
		{
			var algorithm = RetrieveAlgorithm(request.AlgorithmId);
			if (algorithm == null)
				throw new PasswordHashException($"Provided algorithm ID ({request.AlgorithmId}) did not result in a hash algorithm being found in the repo.");

			return algorithm.Hash(request.Password, request.Salt);
		}

		private IPasswordHashAlgorithm RetrieveAlgorithm(string algorithmId)
		{
			if (String.IsNullOrWhiteSpace(algorithmId))
				algorithmId = "*";

			var collection = PasswordAlgorithmRepo.Search(algorithmId, true);
			if (collection.Count == 1)
				return collection.First();

			return collection.Where(a => !a.IsObsolete).First();
		}
	}
}

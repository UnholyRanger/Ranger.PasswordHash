using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ranger.PasswordHash
{
	public static class PasswordAlgorithmRepo
	{
		private static readonly IDictionary<string, IPasswordHashAlgorithm> algorithms;

		static PasswordAlgorithmRepo()
		{
			var availableAlgorithms = new List<IPasswordHashAlgorithm>
			{
				new Pbkdfv210k()
			};

			algorithms = new Dictionary<string, IPasswordHashAlgorithm>();

			foreach (var hashAlgorithm in availableAlgorithms)
				algorithms[hashAlgorithm.AlgorithmId] = hashAlgorithm;
		}

		public static void RegisterAlgorithm(IPasswordHashAlgorithm algorithm)
			=> algorithms[algorithm.AlgorithmId] = algorithm;

		public static void DeregisterAlgorithm(string algorithmId)
			=> algorithms.Remove(algorithmId);

		public static IPasswordHashAlgorithm Get(string algorithmId)
		{
			if (!algorithms.ContainsKey(algorithmId))
				throw new PasswordHashException($"The given algorithm ID '{algorithmId}' was not present in the repo.");

			return algorithms[algorithmId];
		}

		public static IReadOnlyCollection<IPasswordHashAlgorithm> Search(string idSearch, bool includeObsolete = false)
		{
			if (string.IsNullOrWhiteSpace(idSearch))
				return new List<IPasswordHashAlgorithm>();

			var searchRegex = new Lazy<string>(() => AlterRegex(idSearch));

			return algorithms.Where(kvp => !kvp.Value.IsObsolete || includeObsolete)
				.Where(kvp => Regex.IsMatch(kvp.Key, searchRegex.Value, RegexOptions.IgnoreCase))
				.Select(kvp => kvp.Value).ToList();
		}

		private static string AlterRegex(string value)
			=> Regex.Escape(value).Replace(@"\*", ".+").Replace(@"\?", ".");
	}
}

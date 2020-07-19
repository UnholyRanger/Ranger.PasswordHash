using System;
using System.Security.Cryptography;

namespace Ranger.PasswordHash
{
	public abstract class Pbkdfv2Base : IPasswordHashAlgorithm, IPasswordHash
	{
		internal Pbkdfv2Base() { }

		public abstract string AlgorithmId { get; }

		protected abstract int IterationCount { get; }

		public abstract bool IsObsolete { get; set; }

		public PasswordHashResponse Hash(string value, string salt)
			=> PerformHash(value.ConvertToBytes(), salt.ConvertToBytes());

		public PasswordHashResponse Hash(byte[] value, byte[] salt)
			=> PerformHash(value, salt);

		public PasswordHashResponse Hash(PasswordHashRequest request)
			=> Hash(request.Password, request.Salt);

		private PasswordHashResponse PerformHash(byte[] value, byte[] salt)
		{
			try
			{
				using (var hasher = new Rfc2898DeriveBytes(value, salt, 10000))
				{
					return new PasswordHashResponse
					{
						AlgorithmId = AlgorithmId,
						Value = Convert.ToBase64String(hasher.GetBytes(20)),
						IsObsolete = IsObsolete
					};
				}
			}
			catch(Exception e)
			{
				throw new PasswordHashException("Error while performing a hash using PBKDFv2.", e);
			}
		}
	}
}

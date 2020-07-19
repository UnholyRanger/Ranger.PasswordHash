using System;
using System.Text;

namespace Ranger.PasswordHash
{
	internal static class HashHelper
	{
		internal static byte[] ConvertToBytes(this string value)
			=> Encoding.UTF8.GetBytes(value);

		internal static string ConvertToString(this byte[] value)
			=> Encoding.UTF8.GetString(value);

		internal static string ConvertTobase64(this byte[] value)
			=> Convert.ToBase64String(value);
	}
}

using JetBrains.Annotations;
using Oracle.ManagedDataAccess.Client;
using System;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// ˲̬�쳣����� 
	/// </summary>
	public static class OracleTransientExceptionDetector
	{
		/// <summary>
		/// Ӧ������
		/// </summary>
		/// <param name="ex">�쳣</param>
		/// <returns></returns>
		public static bool ShouldRetryOn([NotNull] Exception ex)
		{
			if (ex is OracleException)
			{
				return false;
			}
			return ex is TimeoutException;
		}
	}
}

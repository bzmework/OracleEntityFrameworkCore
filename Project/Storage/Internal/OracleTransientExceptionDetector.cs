using JetBrains.Annotations;
using Oracle.ManagedDataAccess.Client;
using System;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// 瞬态异常检测器 
	/// </summary>
	public static class OracleTransientExceptionDetector
	{
		/// <summary>
		/// 应再重试
		/// </summary>
		/// <param name="ex">异常</param>
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

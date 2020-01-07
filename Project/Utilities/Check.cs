using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace Oracle.EntityFrameworkCore.Utilities
{
	/// <summary>
	/// 检查
	/// </summary>
	[DebuggerStepThrough]
	internal static class Check
	{
		/// <summary>
		/// 是否非空值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">值</param>
		/// <param name="parameterName">参数名</param>
		/// <returns></returns>
		[ContractAnnotation("value:null => halt")]
		public static T NotNull<T>(
			[NoEnumeration] T value, 
			[InvokerParameterName] [NotNull] string parameterName)
		{
			if (value == null)
			{
				NotEmpty(parameterName, "parameterName");
				throw new ArgumentNullException(parameterName);
			}
			return value;
		}

		/// <summary>
		/// 是否非空字符串
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">值</param>
		/// <param name="parameterName">参数名</param>
		/// <returns></returns>
		[ContractAnnotation("value:null => halt")]
		public static IReadOnlyList<T> NotEmpty<T>(
			IReadOnlyList<T> value, 
			[InvokerParameterName] [NotNull] string parameterName)
		{
			NotNull(value, parameterName);
			if (value.Count == 0)
			{
				NotEmpty(parameterName, "parameterName");
				throw new ArgumentException(AbstractionsStrings.CollectionArgumentIsEmpty(parameterName));
			}
			return value;
		}

		/// <summary>
		/// 是否非空字符串
		/// </summary>
		/// <param name="value">值</param>
		/// <param name="parameterName">参数名</param>
		/// <returns></returns>
		[ContractAnnotation("value:null => halt")]
		public static string NotEmpty(
			string value, 
			[InvokerParameterName] [NotNull] string parameterName)
		{
			Exception ex = null;
			if (value == null)
			{
				ex = new ArgumentNullException(parameterName);
			}
			else if (value.Trim().Length == 0)
			{
				ex = new ArgumentException(AbstractionsStrings.ArgumentIsEmpty(parameterName));
			}
			if (ex != null)
			{
				NotEmpty(parameterName, "parameterName");
				throw ex;
			}
			return value;
		}

		/// <summary>
		/// 空值但是是非空字符串
		/// </summary>
		/// <param name="value">值</param>
		/// <param name="parameterName">参数名</param>
		/// <returns></returns>
		public static string NullButNotEmpty(
			string value, 
			[InvokerParameterName] [NotNull] string parameterName)
		{
			if (value != null && value.Length == 0)
			{
				NotEmpty(parameterName, "parameterName");
				throw new ArgumentException(AbstractionsStrings.ArgumentIsEmpty(parameterName));
			}
			return value;
		}

		/// <summary>
		/// 是否有空值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">值</param>
		/// <param name="parameterName">参数名</param>
		/// <returns></returns>
		public static IReadOnlyList<T> HasNoNulls<T>(
			IReadOnlyList<T> value, 
			[InvokerParameterName] [NotNull] string parameterName) where T : class
		{
			NotNull(value, parameterName);
			if (value.Any((T e) => e == null))
			{
				NotEmpty(parameterName, "parameterName");
				throw new ArgumentException(parameterName);
			}
			return value;
		}

		/// <summary>
		/// 是否开启了Debug
		/// </summary>
		/// <param name="logger"></param>
		internal static bool IsDebugEnabled(ILogger logger)
		{
			return (logger != null && logger.IsEnabled(LogLevel.Debug));
		}

		/// <summary>
		/// 是否开启了Trace
		/// </summary>
		/// <param name="logger"></param>
		/// <returns></returns>
		internal static bool IsTraceEnabled(ILogger logger)
		{
			return (logger != null && logger.IsEnabled(LogLevel.Trace));
		}

		/// <summary>
		/// 是否开启了Error
		/// </summary>
		/// <param name="logger"></param>
		/// <returns></returns>
		internal static bool IsErrorEnabled(ILogger logger)
		{
			return (logger != null && logger.IsEnabled(LogLevel.Error));
		}

		/// <summary>
		/// 是否开启了Warning
		/// </summary>
		/// <param name="logger"></param>
		/// <returns></returns>
		internal static bool IsWarningEnabled(ILogger logger)
		{
			return (logger != null && logger.IsEnabled(LogLevel.Warning));
		}
		internal static bool IsInformationEnabled(ILogger logger)
		{
			return (logger != null && logger.IsEnabled(LogLevel.Information));
		}

	}
}

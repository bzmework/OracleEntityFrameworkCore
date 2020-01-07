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
	/// ���
	/// </summary>
	[DebuggerStepThrough]
	internal static class Check
	{
		/// <summary>
		/// �Ƿ�ǿ�ֵ
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">ֵ</param>
		/// <param name="parameterName">������</param>
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
		/// �Ƿ�ǿ��ַ���
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">ֵ</param>
		/// <param name="parameterName">������</param>
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
		/// �Ƿ�ǿ��ַ���
		/// </summary>
		/// <param name="value">ֵ</param>
		/// <param name="parameterName">������</param>
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
		/// ��ֵ�����Ƿǿ��ַ���
		/// </summary>
		/// <param name="value">ֵ</param>
		/// <param name="parameterName">������</param>
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
		/// �Ƿ��п�ֵ
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">ֵ</param>
		/// <param name="parameterName">������</param>
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
		/// �Ƿ�����Debug
		/// </summary>
		/// <param name="logger"></param>
		internal static bool IsDebugEnabled(ILogger logger)
		{
			return (logger != null && logger.IsEnabled(LogLevel.Debug));
		}

		/// <summary>
		/// �Ƿ�����Trace
		/// </summary>
		/// <param name="logger"></param>
		/// <returns></returns>
		internal static bool IsTraceEnabled(ILogger logger)
		{
			return (logger != null && logger.IsEnabled(LogLevel.Trace));
		}

		/// <summary>
		/// �Ƿ�����Error
		/// </summary>
		/// <param name="logger"></param>
		/// <returns></returns>
		internal static bool IsErrorEnabled(ILogger logger)
		{
			return (logger != null && logger.IsEnabled(LogLevel.Error));
		}

		/// <summary>
		/// �Ƿ�����Warning
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

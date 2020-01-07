using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;
using System;
using System.Text;
using System.Threading;

namespace Oracle.EntityFrameworkCore
{
	/// <summary>
	/// ׷����־
	/// </summary>
	/// <typeparam name="TLoggerCategory"></typeparam>
	internal static class Trace<TLoggerCategory> where TLoggerCategory : LoggerCategory<TLoggerCategory>, new()
	{
		internal static string s_dateTimeFormat; // ����ʱ���ʽ
		internal static string s_dateFormat; // ���ڸ�ʽ
		internal static string s_timeFormat; // ʱ���ʽ

		/// <summary>
		/// ʵ����
		/// </summary>
		static Trace()
		{
			s_dateTimeFormat = "yyyy-MM-dd HH:mm:ss.ffffff";
			s_dateFormat = "yyyy_MM_dd";
			s_timeFormat = "HH_mm_ss";
		}

		private static StringBuilder WriteHelper(OracleTraceTag traceTag, OracleTraceClassName className, OracleTraceFuncName funcName, params string[] message)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string str = string.Empty;
			string str2 = string.Empty;
			bool flag = false;
			if (OracleTraceTag.Error == (traceTag & OracleTraceTag.Error))
			{
				traceTag &= (OracleTraceTag)(-268435457);
				str2 = " (ERROR)";
				flag = true;
			}
			else if (OracleTraceTag.Entry == (traceTag & OracleTraceTag.Entry))
			{
				traceTag &= (OracleTraceTag)(-257);
				str2 = " (ENTRY)";
				flag = true;
			}
			else if (OracleTraceTag.Exit == (traceTag & OracleTraceTag.Exit))
			{
				traceTag &= (OracleTraceTag)(-513);
				str2 = " (EXIT)";
				flag = true;
			}
			switch (traceTag)
			{
			case OracleTraceTag.SQL:
				str = " (SQL)";
				break;
			case OracleTraceTag.Environment:
				str = " (ENV)";
				break;
			case OracleTraceTag.Sqlnet:
				str = " (SQLNET)";
				break;
			case OracleTraceTag.Tnsnames:
				str = " (TNSNAMES)";
				break;
			case OracleTraceTag.Version:
				str = " (VER)";
				break;
			case OracleTraceTag.Map:
				str = " (MAP)";
				break;
			case OracleTraceTag.Connection:
				str = " (CONN)";
				break;
			}
			stringBuilder.AppendFormat(GetTimeInfo());
			stringBuilder.AppendFormat("{0,-10} ", str + str2);
			stringBuilder.AppendFormat(className.ToString() + "." + funcName.ToString() + "() ");
			if (message != null && message.Length != 0)
			{
				stringBuilder.AppendFormat(": ");
				if (flag)
				{
					stringBuilder.Append(" ");
				}
				if (OracleTraceTag.SQL == traceTag)
				{
					message[0] = message[0].Replace(Environment.NewLine, " ");
				}
				string[] array = null;
				if (message.Length > 1)
				{
					array = new string[message.Length];
					Array.Copy(message, 1, array, 0, message.Length - 1);
				}
				if (array != null)
				{
					string format = message[0];
					object[] args = array;
					stringBuilder.AppendFormat(format, args);
				}
				else
				{
					stringBuilder.Append(message[0]);
				}
			}
			return stringBuilder;
		}

		/// <summary>
		/// д��־
		/// </summary>
		/// <param name="logger">��־����</param>
		/// <param name="logLevel">��־����</param>
		/// <param name="traceTag">׷�ٱ�ǩ</param>
		/// <param name="className">׷�ٵ�����</param>
		/// <param name="funcName">׷�ٺ�����</param>
		/// <param name="message">׷�ٵ���Ϣ</param>
		internal static void Write(IDiagnosticsLogger<TLoggerCategory> logger, LogLevel logLevel, OracleTraceTag traceTag, OracleTraceClassName className, OracleTraceFuncName funcName, params string[] message)
		{
			if (logger != null)
			{
				if (!ConfigHelper.configItemsTraced)
				{
					lock (ConfigHelper.locker)
					{
						if (!ConfigHelper.configItemsTraced)
						{
							foreach (string configItem in ConfigHelper.configItems)
							{
								StringBuilder stringBuilder = new StringBuilder();
								stringBuilder.Append(GetTimeInfo());
								stringBuilder.Append(" (CONFIG) ");
								stringBuilder.Append(configItem);

								if (Check.IsTraceEnabled(logger?.Logger))
								{
									logger.Logger.LogTrace(stringBuilder.ToString());
								}
							}
							ConfigHelper.configItemsTraced = true;
						}
					}
				}
				StringBuilder stringBuilder2 = WriteHelper(traceTag, className, funcName, message);
				lock (ConfigHelper.locker)
				{
					switch (logLevel)
					{
					case LogLevel.Information:
						logger.Logger.LogInformation(stringBuilder2.ToString());
						break;
					case LogLevel.Warning:
						logger.Logger.LogWarning(stringBuilder2.ToString());
						break;
					case LogLevel.Critical:
						logger.Logger.LogCritical(stringBuilder2.ToString());
						break;
					case LogLevel.Debug:
						logger.Logger.LogDebug(stringBuilder2.ToString());
						break;
					case LogLevel.Error:
						logger.Logger.LogError(stringBuilder2.ToString());
						break;
					case LogLevel.Trace:
						logger.Logger.LogTrace(stringBuilder2.ToString());
						break;
					}
				}
			}
		}

		/// <summary>
		/// ���ʱ����Ϣ
		/// </summary>
		/// <returns></returns>
		private static string GetTimeInfo()
		{
			return string.Format("{0} ThreadID:{1,-3}", DateTime.Now.ToString(s_dateTimeFormat), Thread.CurrentThread.ManagedThreadId);
		}
	}
}

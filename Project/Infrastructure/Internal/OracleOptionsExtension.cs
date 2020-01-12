using System.Text;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Infrastructure.Internal
{
	/// <summary>
	/// Oracle选项扩展
	/// </summary>
	public class OracleOptionsExtension : RelationalOptionsExtension
	{
		private long? _serviceProviderHash; // 服务提供器Hash

		private string _oracleSQLCompatibility; // 兼容OracleSQL。"11"表示11.2G及以前的数据库, "12"表示12c及以后的数据库

		private string _logFragment; // 选项扩展日志片段

		private IDiagnosticsLogger<DbLoggerCategory.Infrastructure> m_oracleLogger;

		/// <summary>
		/// 返回兼容OracleSQL设置
		/// </summary>
		public virtual string OracleSQLCompatibility
		{
			get { return _oracleSQLCompatibility; }
		}

		/// <summary>
		/// 返回选项扩展日志片段，展示提供了哪些选项配置，供用户查看。
		/// </summary>
		public override string LogFragment
		{
			get
			{
				if (_logFragment == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(base.LogFragment);
					if (_oracleSQLCompatibility != null)
					{
						stringBuilder.Append($"OracleSQLCompatibility={_oracleSQLCompatibility}");
					}
					_logFragment = stringBuilder.ToString();
				}
				return _logFragment;
			}
		}

		/// <summary>
		/// 实例化Oracle选项扩展
		/// </summary>
		public OracleOptionsExtension()
		{
			//
		}

		/// <summary>
		/// 实例化Oracle选项扩展
		/// </summary>
		/// <param name="copyFrom"></param>
		/// <param name="logger"></param>
		protected OracleOptionsExtension(
			[NotNull] OracleOptionsExtension copyFrom, 
			IDiagnosticsLogger<DbLoggerCategory.Infrastructure> logger = null)
			: base(copyFrom)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Infrastructure>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleOptionsExtensions, OracleTraceFuncName.ctor);
			}

			Check.NotNull(copyFrom, nameof(copyFrom));
			m_oracleLogger = logger;
			_oracleSQLCompatibility = copyFrom._oracleSQLCompatibility;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Infrastructure>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleOptionsExtensions, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// 克隆。将OracleOptionsExtension提供给内核基础架构
		/// </summary>
		/// <returns></returns>
		protected override RelationalOptionsExtension Clone()
		{
			return new OracleOptionsExtension(this, m_oracleLogger);
		}

		/// <summary>
		/// 使用兼容OracleSQL
		/// </summary>
		/// <param name="oracleSQLCompatibility">使用兼容OracleSQL。"11"表示11.2G及以前的数据库, "12"表示12c及以后的数据库</param>
		/// <returns></returns>
		public virtual OracleOptionsExtension WithOracleSQLCompatibility(string oracleSQLCompatibility)
		{
			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Infrastructure>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleOptionsExtensions, OracleTraceFuncName.WithOracleSQLCompatibility);
			}

			OracleOptionsExtension oracleOptionsExtension = (OracleOptionsExtension)Clone();
			oracleOptionsExtension._oracleSQLCompatibility = oracleSQLCompatibility;

			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Infrastructure>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Map, OracleTraceClassName.OracleOptionsExtensions, OracleTraceFuncName.WithOracleSQLCompatibility, "OracleSQLCompatibility: " + oracleSQLCompatibility);
			}
			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{ 
				Trace<DbLoggerCategory.Infrastructure>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleOptionsExtensions, OracleTraceFuncName.WithOracleSQLCompatibility);
			}

			return oracleOptionsExtension;
		}

		/// <summary>
		/// 获取服务提供程序哈希代码
		/// </summary>
		/// <returns></returns>
		public override long GetServiceProviderHashCode()
		{
			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Infrastructure>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleOptionsExtensions, OracleTraceFuncName.GetServiceProviderHashCode);
			}

			if (_serviceProviderHash == null)
			{
				_serviceProviderHash = ((base.GetServiceProviderHashCode() * 397) ^ (_oracleSQLCompatibility?.GetHashCode() ?? 0));
			}

			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Infrastructure>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleOptionsExtensions, OracleTraceFuncName.GetServiceProviderHashCode);
			}

			return _serviceProviderHash.Value;
		}

		/// <summary>
		/// 应用服务
		/// </summary>
		/// <param name="services">服务集合</param>
		/// <returns></returns>
		public override bool ApplyServices(IServiceCollection services)
		{
			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Infrastructure>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleOptionsExtensions, OracleTraceFuncName.ApplyServices);
			}

			Check.NotNull(services, nameof(services));
			services.AddEntityFrameworkOracle();

			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Infrastructure>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleOptionsExtensions, OracleTraceFuncName.ApplyServices);
			}

			return true;
		}
	}
}

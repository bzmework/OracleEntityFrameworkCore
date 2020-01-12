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
	/// Oracleѡ����չ
	/// </summary>
	public class OracleOptionsExtension : RelationalOptionsExtension
	{
		private long? _serviceProviderHash; // �����ṩ��Hash

		private string _oracleSQLCompatibility; // ����OracleSQL��"11"��ʾ11.2G����ǰ�����ݿ�, "12"��ʾ12c���Ժ�����ݿ�

		private string _logFragment; // ѡ����չ��־Ƭ��

		private IDiagnosticsLogger<DbLoggerCategory.Infrastructure> m_oracleLogger;

		/// <summary>
		/// ���ؼ���OracleSQL����
		/// </summary>
		public virtual string OracleSQLCompatibility
		{
			get { return _oracleSQLCompatibility; }
		}

		/// <summary>
		/// ����ѡ����չ��־Ƭ�Σ�չʾ�ṩ����Щѡ�����ã����û��鿴��
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
		/// ʵ����Oracleѡ����չ
		/// </summary>
		public OracleOptionsExtension()
		{
			//
		}

		/// <summary>
		/// ʵ����Oracleѡ����չ
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
		/// ��¡����OracleOptionsExtension�ṩ���ں˻����ܹ�
		/// </summary>
		/// <returns></returns>
		protected override RelationalOptionsExtension Clone()
		{
			return new OracleOptionsExtension(this, m_oracleLogger);
		}

		/// <summary>
		/// ʹ�ü���OracleSQL
		/// </summary>
		/// <param name="oracleSQLCompatibility">ʹ�ü���OracleSQL��"11"��ʾ11.2G����ǰ�����ݿ�, "12"��ʾ12c���Ժ�����ݿ�</param>
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
		/// ��ȡ�����ṩ�����ϣ����
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
		/// Ӧ�÷���
		/// </summary>
		/// <param name="services">���񼯺�</param>
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

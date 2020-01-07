using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Infrastructure;
using Oracle.EntityFrameworkCore.Utilities;
using Oracle.ManagedDataAccess.Client;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// 关系连接
	/// </summary>
	public class OracleRelationalConnection : RelationalConnection, IOracleConnection, IRelationalConnection, IRelationalTransactionManager, IDbContextTransactionManager, IResettableService, IDisposable
	{
		/// <summary>
		/// 实体PDB管理用户
		/// </summary>
		public const string EFPDBAdminUser = "ef_pdb_admin";

		internal const int DefaultMasterConnectionCommandTimeout = 60;

		private IDiagnosticsLogger<DbLoggerCategory.Database.Connection> m_oracleLogger;

		/// <summary>
		/// 支持环境事务
		/// </summary>
		protected override bool SupportsAmbientTransactions
		{
			get { return true; }
		}

		/// <summary>
		/// 是否启用了多个活动结果集
		/// </summary>
		public override bool IsMultipleActiveResultSetsEnabled
		{
			get { return true; }
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">关系连接依赖</param>
		/// <param name="logger">日志</param>
		public OracleRelationalConnection(
			[NotNull] RelationalConnectionDependencies dependencies,
			IDiagnosticsLogger<DbLoggerCategory.Database.Connection> logger = null)
			: base(dependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Database.Connection>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleRelationalConnection, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Database.Connection>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleRelationalConnection, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// 创建数据库连接
		/// </summary>
		/// <returns></returns>
		protected override DbConnection CreateDbConnection()
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Connection>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleRelationalConnection, OracleTraceFuncName.CreateDbConnection);
				}

				return (DbConnection)new OracleConnection(ConnectionString);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Connection>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleCompositeMemberTranslator, OracleTraceFuncName.CreateDbConnection, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Connection>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleRelationalConnection, OracleTraceFuncName.CreateDbConnection);
				}
			}
		}

		/// <summary>
		/// 创建主连接
		/// </summary>
		/// <returns></returns>
		public virtual IOracleConnection CreateMasterConnection()
		{
			DbContextOptions dbContextOptions = null;

			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Connection>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleRelationalConnection, OracleTraceFuncName.CreateMasterConnection);
				}

				OracleConnectionStringBuilder connObj = new OracleConnectionStringBuilder(ConnectionString);
				connObj.UserID = "ef_pdb_admin";
				connObj.Password = "ef_pdb_admin";
				OracleConnectionStringBuilder connObjRef = (OracleConnectionStringBuilder)(object)connObj;
				dbContextOptions = new DbContextOptionsBuilder().UseOracle(((DbConnectionStringBuilder)(object)connObjRef).ConnectionString, delegate(OracleDbContextOptionsBuilder b)
				{
					b.CommandTimeout(CommandTimeout ?? 60);
				}).Options;
				return new OracleRelationalConnection(Dependencies.With(dbContextOptions), m_oracleLogger);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Connection>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleCompositeMemberTranslator, OracleTraceFuncName.CreateMasterConnection, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Connection>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleRelationalConnection, OracleTraceFuncName.CreateMasterConnection);
				}
			}
		}
	}
}

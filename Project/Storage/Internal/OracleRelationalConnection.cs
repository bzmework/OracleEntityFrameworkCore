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
	/// ��ϵ����
	/// </summary>
	public class OracleRelationalConnection : RelationalConnection, IOracleConnection, IRelationalConnection, IRelationalTransactionManager, IDbContextTransactionManager, IResettableService, IDisposable
	{
		/// <summary>
		/// ʵ��PDB�����û�
		/// </summary>
		public const string EFPDBAdminUser = "ef_pdb_admin";

		internal const int DefaultMasterConnectionCommandTimeout = 60;

		private IDiagnosticsLogger<DbLoggerCategory.Database.Connection> m_oracleLogger;

		/// <summary>
		/// ֧�ֻ�������
		/// </summary>
		protected override bool SupportsAmbientTransactions
		{
			get { return true; }
		}

		/// <summary>
		/// �Ƿ������˶��������
		/// </summary>
		public override bool IsMultipleActiveResultSetsEnabled
		{
			get { return true; }
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="dependencies">��ϵ��������</param>
		/// <param name="logger">��־</param>
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
		/// �������ݿ�����
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
		/// ����������
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

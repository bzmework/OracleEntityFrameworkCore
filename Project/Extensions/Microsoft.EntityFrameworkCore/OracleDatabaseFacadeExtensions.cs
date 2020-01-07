using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Storage.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
	/// <summary>
	/// 数据库外观扩展
	/// </summary>
	public static class OracleDatabaseFacadeExtensions
	{
		/// <summary>
		/// 是否是Oracle数据库
		/// </summary>
		/// <param name="database">数据库</param>
		/// <returns></returns>
		public static bool IsOracle([NotNull] this DatabaseFacade database)
		{
			Check.NotNull(database, nameof(database));
			return database.ProviderName.Equals(typeof(OracleOptionsExtension).GetTypeInfo().Assembly.GetName().Name, StringComparison.Ordinal);
		}

		/// <summary>
		/// 确保数据库和表已创建
		/// </summary>
		/// <param name="database">数据库</param>
		/// <param name="schemas">方案</param>
		/// <returns></returns>
		public static bool EnsureCreated([NotNull] this DatabaseFacade database, string[] schemas)
		{
			Check.NotNull(database, nameof(database));
			if (schemas == null || schemas.Length == 0)
			{
				return database.EnsureCreated();
			}
			OracleDatabaseCreator oracleDatabaseCreator = (OracleDatabaseCreator)database.GetService<IRelationalDatabaseCreator>();
			using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
			{
				if (!oracleDatabaseCreator.Exists())
				{
					oracleDatabaseCreator.Create();
					oracleDatabaseCreator.CreateTables();
					return true;
				}
				if (!oracleDatabaseCreator.HasTablesWithSchemaOption(schemas))
				{
					oracleDatabaseCreator.CreateTables();
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 异步检查确保数据库和表已创建
		/// </summary>
		/// <param name="database">数据库</param>
		/// <param name="schemas">方案</param>
		/// <param name="cancellationToken">取消令牌</param>
		/// <returns></returns>
		public static async Task<bool> EnsureCreatedAsync([NotNull] this DatabaseFacade database, string[] schemas, CancellationToken cancellationToken = default(CancellationToken))
		{
			Check.NotNull(database, nameof(database));

			if (schemas == null || schemas.Length == 0)
			{
				return await database.EnsureCreatedAsync();
			}
			OracleDatabaseCreator db = (OracleDatabaseCreator)database.GetService<IRelationalDatabaseCreator>();
			using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
			{
				if (!(await db.ExistsAsync(cancellationToken)))
				{
					await db.CreateAsync(cancellationToken);
					await db.CreateTablesAsync(cancellationToken);
					return true;
				}
				if (!(await db.HasTablesAsyncWithSchemaOption(schemas, cancellationToken)))
				{
					await db.CreateTablesAsync(cancellationToken);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 确保被删除
		/// </summary>
		/// <param name="database">数据库</param>
		/// <param name="schemas">方案</param>
		/// <returns></returns>
		public static bool EnsureDeleted([NotNull] this DatabaseFacade database, string[] schemas)
		{
			Check.NotNull(database, nameof(database));

			if (schemas == null || schemas.Length == 0)
			{
				return database.EnsureDeleted();
			}
			OracleDatabaseCreator oracleDatabaseCreator = (OracleDatabaseCreator)database.GetService<IRelationalDatabaseCreator>();
			if (oracleDatabaseCreator.Exists())
			{
				oracleDatabaseCreator.DeleteWithSchemaOption(schemas);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 异步确保被删除
		/// </summary>
		/// <param name="database">数据库</param>
		/// <param name="schemas">方案</param>
		/// <param name="cancellationToken">取消令牌</param>
		/// <returns></returns>
		public static async Task<bool> EnsureDeletedAsync([NotNull] this DatabaseFacade database, string[] schemas, CancellationToken cancellationToken = default(CancellationToken))
		{
			Check.NotNull(database, nameof(database));

			if (schemas == null || schemas.Length == 0)
			{
				return await database.EnsureDeletedAsync();
			}
			OracleDatabaseCreator oracleDBCreator = (OracleDatabaseCreator)database.GetService<IRelationalDatabaseCreator>();
			if (await oracleDBCreator.ExistsAsync(cancellationToken))
			{
				await oracleDBCreator.DeleteAsyncWithSchemaOption(schemas, cancellationToken);
				return true;
			}
			return false;
		}
	}
}

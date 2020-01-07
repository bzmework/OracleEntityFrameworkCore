using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Oracle.EntityFrameworkCore.Infrastructure;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
	/// <summary>
	/// 数据库上下文选项扩展
	/// </summary>
	public static class OracleDbContextOptionsExtensions
	{
		/// <summary>
        /// 配置上下文(Context)以连接到Oracle数据库
		/// </summary>
		/// <param name="optionsBuilder">数据库上下文选项创建器</param>
		/// <param name="connectionString">连接串</param>
		/// <param name="oracleOptionsAction">选项操作</param>
		/// <returns></returns>
		public static DbContextOptionsBuilder UseOracle(
			[NotNull] this DbContextOptionsBuilder optionsBuilder, 
			[NotNull] string connectionString, 
			[CanBeNull] Action<OracleDbContextOptionsBuilder> oracleOptionsAction = null)
		{
			Check.NotNull(optionsBuilder, nameof(optionsBuilder));
			Check.NotEmpty(connectionString, nameof(connectionString));

			// 获得扩展，参见OracleOptionsExtension类
			OracleOptionsExtension extension = (OracleOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnectionString(connectionString);

			// 配置Oracle扩展，将扩展添加到DbContextOptions中
			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

			// 配置报警扩展
			ConfigureWarnings(optionsBuilder);

			// 进行选项配置
			oracleOptionsAction?.Invoke(new OracleDbContextOptionsBuilder(optionsBuilder));

			// 返回
			return optionsBuilder;
		}

		/// <summary>
        /// 配置上下文(Context)以连接到Oracle数据库。
		/// </summary>
        /// <param name="optionsBuilder"> 用于配置上下文的创建器 </param>
        /// <param name="connection">
        /// 用于连接到数据库的现有<see cref="DbConnection" /> 。
        /// 如果连接处于打开状态，则EF不会打开或关闭连接。
        /// 如果连接处于关闭状态，那么EF将根据需要打开和关闭连接。
        /// </param>
        /// <param name="oracleOptionsAction"> 允许附加Oracle特定配置的可选操作 </param>
		/// <returns></returns>
		public static DbContextOptionsBuilder UseOracle(
			[NotNull] this DbContextOptionsBuilder optionsBuilder,
			[NotNull] DbConnection connection, 
			[CanBeNull] Action<OracleDbContextOptionsBuilder> oracleOptionsAction = null)
		{
			Check.NotNull(optionsBuilder, nameof(optionsBuilder));
			Check.NotNull(connection, nameof(connection));

			// 获得扩展，参见OracleOptionsExtension类
			OracleOptionsExtension extension = (OracleOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnection(connection);

			// 配置Oracle扩展，将扩展添加到DbContextOptions中
			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

			// 配置报警扩展
			ConfigureWarnings(optionsBuilder);

			// 进行选项配置
			oracleOptionsAction?.Invoke(new OracleDbContextOptionsBuilder(optionsBuilder));

			// 返回
			return optionsBuilder;
		}

		/// <summary>
        /// 配置上下文(Context)以连接到Oracle数据库
		/// </summary>
		/// <typeparam name="TContext"></typeparam>
		/// <param name="optionsBuilder">数据库上下文选项创建器</param>
        /// <param name="connectionString"> 要连接到的数据库的连接字符串 </param>
        /// <param name="oracleOptionsAction"> 允许附加Oracle特定配置的可选操作 </param>
        /// <returns> 选项生成器，以便可以链接进一步的配置 </returns>
		public static DbContextOptionsBuilder<TContext> UseOracle<TContext>(
			[NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder, 
			[NotNull] string connectionString, 
			[CanBeNull] Action<OracleDbContextOptionsBuilder> oracleOptionsAction = null) where TContext : DbContext
		{
			Check.NotNull(optionsBuilder, nameof(optionsBuilder));
			Check.NotEmpty(connectionString, nameof(connectionString));

			DbContextOptionsBuilder<TContext> context = (DbContextOptionsBuilder<TContext>)UseOracle((DbContextOptionsBuilder)optionsBuilder, connectionString, oracleOptionsAction);
			
			return context;
		}

		/// <summary>
		/// 配置上下文(Context)以连接到Oracle数据库
		/// </summary>
		/// <typeparam name="TContext"> 要配置的上下文类型 </typeparam>
		/// <param name="optionsBuilder">数据库上下文选项创建器</param>
		/// <param name="connection">
		/// 用于连接到数据库的现有<see cref="DbConnection" /> 。
		/// 如果连接处于打开状态，则EF不会打开或关闭连接。
		/// 如果连接处于关闭状态，那么EF将根据需要打开和关闭连接。
		/// </param>
		/// <param name="oracleOptionsAction"> 允许附加Oracle特定配置的可选操作 </param>
		/// <returns> 选项生成器，以便可以链接进一步的配置 </returns>
		public static DbContextOptionsBuilder<TContext> UseOracle<TContext>(
			[NotNull] this DbContextOptionsBuilder<TContext> optionsBuilder, 
			[NotNull] DbConnection connection, 
			[CanBeNull] Action<OracleDbContextOptionsBuilder> oracleOptionsAction = null) where TContext : DbContext
		{
			Check.NotNull(optionsBuilder, nameof(optionsBuilder));
			Check.NotNull(connection, nameof(connection));

			DbContextOptionsBuilder<TContext> context = (DbContextOptionsBuilder<TContext>)UseOracle((DbContextOptionsBuilder)optionsBuilder, connection, oracleOptionsAction);

			return context;
		}

		/// <summary>
		/// 获得OracleOptions扩展，如果系统中已经存在则返回既有扩展，否则新建一个OracleOptionsExtension
		/// </summary>
		/// <param name="optionsBuilder"></param>
		/// <returns></returns>
		private static OracleOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
		{
			return optionsBuilder.Options.FindExtension<OracleOptionsExtension>() ?? new OracleOptionsExtension();
		}

		/// <summary>
		/// 配置报警扩展
		/// </summary>
		/// <param name="optionsBuilder"></param>
		private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
		{
			CoreOptionsExtension coreOptionsExtension = optionsBuilder.Options.FindExtension<CoreOptionsExtension>() ?? new CoreOptionsExtension();
			coreOptionsExtension = coreOptionsExtension.WithWarningsConfiguration(coreOptionsExtension.WarningsConfiguration.TryWithExplicit(RelationalEventId.AmbientTransactionWarning, WarningBehavior.Throw));
			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
		}
	}
}

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
	/// ���ݿ�������ѡ����չ
	/// </summary>
	public static class OracleDbContextOptionsExtensions
	{
		/// <summary>
        /// ����������(Context)�����ӵ�Oracle���ݿ�
		/// </summary>
		/// <param name="optionsBuilder">���ݿ�������ѡ�����</param>
		/// <param name="connectionString">���Ӵ�</param>
		/// <param name="oracleOptionsAction">ѡ�����</param>
		/// <returns></returns>
		public static DbContextOptionsBuilder UseOracle(
			[NotNull] this DbContextOptionsBuilder optionsBuilder, 
			[NotNull] string connectionString, 
			[CanBeNull] Action<OracleDbContextOptionsBuilder> oracleOptionsAction = null)
		{
			Check.NotNull(optionsBuilder, nameof(optionsBuilder));
			Check.NotEmpty(connectionString, nameof(connectionString));

			// �����չ���μ�OracleOptionsExtension��
			OracleOptionsExtension extension = (OracleOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnectionString(connectionString);

			// ����Oracle��չ������չ��ӵ�DbContextOptions��
			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

			// ���ñ�����չ
			ConfigureWarnings(optionsBuilder);

			// ����ѡ������
			oracleOptionsAction?.Invoke(new OracleDbContextOptionsBuilder(optionsBuilder));

			// ����
			return optionsBuilder;
		}

		/// <summary>
        /// ����������(Context)�����ӵ�Oracle���ݿ⡣
		/// </summary>
        /// <param name="optionsBuilder"> �������������ĵĴ����� </param>
        /// <param name="connection">
        /// �������ӵ����ݿ������<see cref="DbConnection" /> ��
        /// ������Ӵ��ڴ�״̬����EF����򿪻�ر����ӡ�
        /// ������Ӵ��ڹر�״̬����ôEF��������Ҫ�򿪺͹ر����ӡ�
        /// </param>
        /// <param name="oracleOptionsAction"> ������Oracle�ض����õĿ�ѡ���� </param>
		/// <returns></returns>
		public static DbContextOptionsBuilder UseOracle(
			[NotNull] this DbContextOptionsBuilder optionsBuilder,
			[NotNull] DbConnection connection, 
			[CanBeNull] Action<OracleDbContextOptionsBuilder> oracleOptionsAction = null)
		{
			Check.NotNull(optionsBuilder, nameof(optionsBuilder));
			Check.NotNull(connection, nameof(connection));

			// �����չ���μ�OracleOptionsExtension��
			OracleOptionsExtension extension = (OracleOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnection(connection);

			// ����Oracle��չ������չ��ӵ�DbContextOptions��
			((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

			// ���ñ�����չ
			ConfigureWarnings(optionsBuilder);

			// ����ѡ������
			oracleOptionsAction?.Invoke(new OracleDbContextOptionsBuilder(optionsBuilder));

			// ����
			return optionsBuilder;
		}

		/// <summary>
        /// ����������(Context)�����ӵ�Oracle���ݿ�
		/// </summary>
		/// <typeparam name="TContext"></typeparam>
		/// <param name="optionsBuilder">���ݿ�������ѡ�����</param>
        /// <param name="connectionString"> Ҫ���ӵ������ݿ�������ַ��� </param>
        /// <param name="oracleOptionsAction"> ������Oracle�ض����õĿ�ѡ���� </param>
        /// <returns> ѡ�����������Ա�������ӽ�һ�������� </returns>
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
		/// ����������(Context)�����ӵ�Oracle���ݿ�
		/// </summary>
		/// <typeparam name="TContext"> Ҫ���õ����������� </typeparam>
		/// <param name="optionsBuilder">���ݿ�������ѡ�����</param>
		/// <param name="connection">
		/// �������ӵ����ݿ������<see cref="DbConnection" /> ��
		/// ������Ӵ��ڴ�״̬����EF����򿪻�ر����ӡ�
		/// ������Ӵ��ڹر�״̬����ôEF��������Ҫ�򿪺͹ر����ӡ�
		/// </param>
		/// <param name="oracleOptionsAction"> ������Oracle�ض����õĿ�ѡ���� </param>
		/// <returns> ѡ�����������Ա�������ӽ�һ�������� </returns>
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
		/// ���OracleOptions��չ�����ϵͳ���Ѿ������򷵻ؼ�����չ�������½�һ��OracleOptionsExtension
		/// </summary>
		/// <param name="optionsBuilder"></param>
		/// <returns></returns>
		private static OracleOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
		{
			return optionsBuilder.Options.FindExtension<OracleOptionsExtension>() ?? new OracleOptionsExtension();
		}

		/// <summary>
		/// ���ñ�����չ
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

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Internal;
using Oracle.EntityFrameworkCore.Metadata.Conventions;
using Oracle.EntityFrameworkCore.Migrations;
using Oracle.EntityFrameworkCore.Migrations.Internal;
using Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal;
using Oracle.EntityFrameworkCore.Query.Internal;
using Oracle.EntityFrameworkCore.Query.Sql.Internal;
using Oracle.EntityFrameworkCore.Storage.Internal;
using Oracle.EntityFrameworkCore.Update.Internal;
using Oracle.EntityFrameworkCore.Utilities;
using Oracle.EntityFrameworkCore.ValueGeneration.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// ���񼯺���չ
	/// </summary>
	public static class OracleServiceCollectionExtensions
	{
		/// <summary>
		///  ���Oracleʵ���� 
		/// </summary>
		/// <param name="serviceCollection">���񼯺�</param>
		/// <returns></returns>
		public static IServiceCollection AddEntityFrameworkOracle([NotNull] this IServiceCollection serviceCollection)
		{
			Check.NotNull(serviceCollection, nameof(serviceCollection));

			// ע����񡣽���ʵ���ܷ����ʵ����δע��ʱ��ӡ�����ķ�Χ��ʵ�����Լ����塣
			var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
				.TryAdd<IDatabaseProvider, DatabaseProvider<OracleOptionsExtension>>()
				.TryAdd<IValueGeneratorCache>(p => p.GetService<IOracleValueGeneratorCache>())
				.TryAdd<IRelationalTypeMappingSource, OracleTypeMappingSource>()
				.TryAdd<ISqlGenerationHelper, OracleSqlGenerationHelper>()
				.TryAdd<IMigrationsAnnotationProvider, OracleMigrationsAnnotationProvider>()
				.TryAdd<IModelValidator, OracleModelValidator>()
				.TryAdd<IConventionSetBuilder, OracleConventionSetBuilder>()
				.TryAdd<IUpdateSqlGenerator, OracleUpdateSqlGenerator>()
				.TryAdd<ISingletonUpdateSqlGenerator, OracleUpdateSqlGenerator>()
				.TryAdd<IModificationCommandBatchFactory, OracleModificationCommandBatchFactory>()
				.TryAdd<IValueGeneratorSelector, OracleValueGeneratorSelector>()
				.TryAdd<IRelationalConnection>(p => p.GetService<IOracleConnection>())
				.TryAdd<IMigrationsSqlGenerator, OracleMigrationsSqlGenerator>()
				.TryAdd<IRelationalDatabaseCreator, OracleDatabaseCreator>()
				.TryAdd<IHistoryRepository, OracleHistoryRepository>()
				.TryAdd<ICompiledQueryCacheKeyGenerator, OracleCompiledQueryCacheKeyGenerator>()
				.TryAdd<IExecutionStrategyFactory, OracleExecutionStrategyFactory>()
				.TryAdd<IQueryCompilationContextFactory, OracleQueryCompilationContextFactory>()
				.TryAdd<IMemberTranslator, OracleCompositeMemberTranslator>()
				.TryAdd<ICompositeMethodCallTranslator, OracleCompositeMethodCallTranslator>()
				.TryAdd<IQuerySqlGeneratorFactory, OracleQuerySqlGeneratorFactory>()
				.TryAdd<IRelationalCommandBuilderFactory, OracleRelationalCommandBuilderFactory>()
				.TryAdd<IMigrationCommandExecutor, OracleMigrationCommandExecutor>()
				.TryAdd<ISingletonOptions, IOracleOptions>((IServiceProvider p) => p.GetService<IOracleOptions>())
				.TryAddProviderSpecificServices(delegate (ServiceCollectionMap b)
				{
					b.TryAddSingleton<IOracleValueGeneratorCache, OracleValueGeneratorCache>()
					 .TryAddSingleton<IOracleOptions, OracleOptions>()
					 .TryAddScoped<IOracleSequenceValueGeneratorFactory, OracleSequenceValueGeneratorFactory>()
					 .TryAddScoped<IOracleConnection, OracleRelationalConnection>();
				});

			// ���Խ�����ע�ᵽ�ںˡ����ݿ��ṩ��������ڷ���ע������һ�����ô˷��������������ṩ�������ע��֮��
			builder.TryAddCoreServices();

			return serviceCollection;
		}
	}
}

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;
using Oracle.EntityFrameworkCore.Internal;
using Oracle.EntityFrameworkCore.Scaffolding.Internal;
using Oracle.EntityFrameworkCore.Storage.Internal;

namespace Oracle.EntityFrameworkCore.Design.Internal
{
    /// <summary>
    /// 设计时服务
    /// </summary>
    public class OracleDesignTimeServices : IDesignTimeServices
    {
        /// <summary>
        /// 配置设计时服务
        /// </summary>
        /// <param name="serviceCollection">服务集合</param>
        public virtual void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<IRelationalTypeMappingSource, OracleTypeMappingSource>()
                .AddSingleton<IDatabaseModelFactory, OracleDatabaseModelFactory>()
                .AddSingleton<IProviderConfigurationCodeGenerator, OracleCodeGenerator>()
                .AddSingleton<IAnnotationCodeGenerator, OracleAnnotationCodeGenerator>()
                .TryAddSingleton<IOracleOptions, OracleOptions>();
        }
    }
}

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Oracle.EntityFrameworkCore.Query.Internal
{
	/// <summary>
	/// 查询编译上下文
	/// </summary>
	public class OracleQueryCompilationContext : RelationalQueryCompilationContext
	{
		internal string _oracleSQLCompatibility = "12";

		/// <summary>
		/// 是否支持横向连接
		/// </summary>
		public override bool IsLateralJoinSupported
		{
			get
			{
				if (_oracleSQLCompatibility == "11")
				{
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// 最大表别名长度
		/// </summary>
		public override int MaxTableAliasLength
		{
			get { return 30; }
		}

		/// <summary>
		/// 实例化查询编译上下文
		/// </summary>
		/// <param name="dependencies">查询编译上下文依赖</param>
		/// <param name="linqOperatorProvider">Linq操作数提供器</param>
		/// <param name="queryMethodProvider">查询方法提供器</param>
		/// <param name="trackQueryResults">跟踪查询结果</param>
		/// <param name="oracleSQLCompatibility">兼容SQL。"11"表示11.2G及以前的数据库, "12"表示12c及以后的数据库</param>
		public OracleQueryCompilationContext(
			[NotNull] QueryCompilationContextDependencies dependencies, 
			[NotNull] ILinqOperatorProvider linqOperatorProvider,
			[NotNull] IQueryMethodProvider queryMethodProvider, 
			bool trackQueryResults, 
			string oracleSQLCompatibility)
			: base(dependencies, linqOperatorProvider, queryMethodProvider, trackQueryResults)
		{
			_oracleSQLCompatibility = oracleSQLCompatibility;
		}
	}
}

using Microsoft.EntityFrameworkCore.Infrastructure;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;

namespace Oracle.EntityFrameworkCore.Internal
{
	/// <summary>
	/// 选项
	/// </summary>
	public class OracleOptions : IOracleOptions, ISingletonOptions
	{
		/// <summary>
		/// 兼容OracleSQL。"11"表示11.2G及以前的数据库, "12"表示12c及以后的数据库
		/// </summary>
		public virtual string OracleSQLCompatibility
		{
			get;
			private set;
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="options">数据库上下文选项</param>
		public virtual void Initialize(IDbContextOptions options)
		{
			OracleOptionsExtension oracleOptionsExtension = options.FindExtension<OracleOptionsExtension>() ?? new OracleOptionsExtension();
			OracleSQLCompatibility = oracleOptionsExtension.OracleSQLCompatibility;
		}

		/// <summary>
		/// 验证
		/// </summary>
		/// <param name="options">数据库上下文选项</param>
		public virtual void Validate(IDbContextOptions options)
		{
			OracleOptionsExtension oracleOptionsExtension = options.FindExtension<OracleOptionsExtension>() ?? new OracleOptionsExtension();
			if (OracleSQLCompatibility != null)
			{
				OracleSQLCompatibility.Equals(oracleOptionsExtension.OracleSQLCompatibility);
			}
		}
	}
}

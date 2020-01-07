using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Oracle.EntityFrameworkCore.Migrations.Operations
{
	/// <summary>
	/// 删除用户操作
	/// </summary>
	public class OracleDropUserOperation : MigrationOperation
	{
		/// <summary>
		/// 用户名
		/// </summary>
		public virtual string UserName
		{
			get;
			[param: NotNull]
			set;
		}
	}
}

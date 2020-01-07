using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Oracle.EntityFrameworkCore.Migrations.Operations
{
	/// <summary>
	/// 创建用户操作
	/// </summary>
	public class OracleCreateUserOperation : MigrationOperation
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

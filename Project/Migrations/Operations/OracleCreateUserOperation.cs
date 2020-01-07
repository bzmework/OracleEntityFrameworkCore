using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Oracle.EntityFrameworkCore.Migrations.Operations
{
	/// <summary>
	/// �����û�����
	/// </summary>
	public class OracleCreateUserOperation : MigrationOperation
	{
		/// <summary>
		/// �û���
		/// </summary>
		public virtual string UserName
		{
			get;
			[param: NotNull]
			set;
		}
	}
}

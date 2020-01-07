using Microsoft.EntityFrameworkCore.Infrastructure;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;

namespace Oracle.EntityFrameworkCore.Internal
{
	/// <summary>
	/// ѡ��
	/// </summary>
	public class OracleOptions : IOracleOptions, ISingletonOptions
	{
		/// <summary>
		/// ����OracleSQL��"11"��ʾ11.2G����ǰ�����ݿ�, "12"��ʾ12c���Ժ�����ݿ�
		/// </summary>
		public virtual string OracleSQLCompatibility
		{
			get;
			private set;
		}

		/// <summary>
		/// ��ʼ��
		/// </summary>
		/// <param name="options">���ݿ�������ѡ��</param>
		public virtual void Initialize(IDbContextOptions options)
		{
			OracleOptionsExtension oracleOptionsExtension = options.FindExtension<OracleOptionsExtension>() ?? new OracleOptionsExtension();
			OracleSQLCompatibility = oracleOptionsExtension.OracleSQLCompatibility;
		}

		/// <summary>
		/// ��֤
		/// </summary>
		/// <param name="options">���ݿ�������ѡ��</param>
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

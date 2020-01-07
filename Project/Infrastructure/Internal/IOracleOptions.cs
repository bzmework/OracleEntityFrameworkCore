using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Oracle.EntityFrameworkCore.Infrastructure.Internal
{
	/// <summary>
	/// Oracleѡ�� 
	/// </summary>
	public interface IOracleOptions : ISingletonOptions
	{
		/// <summary>
		/// ����OracleSQL
		/// </summary>
		string OracleSQLCompatibility
		{
			get;
		}
	}
}

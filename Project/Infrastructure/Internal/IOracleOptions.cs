using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Oracle.EntityFrameworkCore.Infrastructure.Internal
{
	/// <summary>
	/// Oracle—°œÓ 
	/// </summary>
	public interface IOracleOptions : ISingletonOptions
	{
		/// <summary>
		/// ºÊ»›OracleSQL
		/// </summary>
		string OracleSQLCompatibility
		{
			get;
		}
	}
}

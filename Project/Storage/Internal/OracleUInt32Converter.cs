using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq.Expressions;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// UInt32×ª»»Æ÷
	/// </summary>
	internal class OracleUInt32Converter : ValueConverter<uint, long>
	{
		/// <summary>
		/// ÊµÀý»¯
		/// </summary>
		internal OracleUInt32Converter()
			: base((Expression<Func<uint, long>>)((uint x) => ToStore(x)), (Expression<Func<long, uint>>)((long x) => FromStore(x)), (ConverterMappingHints)null)
		{
			//
		}

		private static long ToStore(uint val)
		{
			return Convert.ToInt64(val);
		}

		private static uint FromStore(long val)
		{
			return Convert.ToUInt32(val);
		}
	}
}

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq.Expressions;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// UInt16×ª»»Æ÷
	/// </summary>
	internal class OracleUInt16Converter : ValueConverter<ushort, int>
	{
		/// <summary>
		/// ÊµÀý»¯
		/// </summary>
		internal OracleUInt16Converter()
			: base((Expression<Func<ushort, int>>)((ushort x) => ToStore(x)), (Expression<Func<int, ushort>>)((int x) => FromStore(x)), (ConverterMappingHints)null)
		{
			//
		}

		private static int ToStore(ushort val)
		{
			return Convert.ToInt32(val);
		}

		private static ushort FromStore(int val)
		{
			return Convert.ToUInt16(val);
		}
	}
}

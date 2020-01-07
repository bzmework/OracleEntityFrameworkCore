using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// SByte×ª»»Æ÷
	/// </summary>
	internal class OracleSByteConverter : ValueConverter<sbyte, short>
	{
		/// <summary>
		/// ÊµÀý»¯
		/// </summary>
		internal OracleSByteConverter()
			: base((Expression<Func<sbyte, short>>)((sbyte x) => ToStore(x)), (Expression<Func<short, sbyte>>)((short x) => FromStore(x)), (ConverterMappingHints)null)
		{
			//
		}

		private static short ToStore(sbyte val)
		{
			return Convert.ToInt16(val);
		}

		private static sbyte FromStore(short val)
		{
			return Convert.ToSByte(val);
		}
	}
}

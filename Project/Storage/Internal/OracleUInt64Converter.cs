using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq.Expressions;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// UInt64×ª»»Æ÷
	/// </summary>
	internal class OracleUInt64Converter : ValueConverter<ulong, decimal>
	{
		/// <summary>
		/// ÊµÀý»¯
		/// </summary>
		internal OracleUInt64Converter()
			: base((Expression<Func<ulong, decimal>>)((ulong x) => ToStore(x)), (Expression<Func<decimal, ulong>>)((decimal x) => FromStore(x)), (ConverterMappingHints)null)
		{
			//
		}

		private static decimal ToStore(ulong val)
		{
			return val;
		}

		private static ulong FromStore(decimal val)
		{
			return (ulong)val;
		}
	}
}

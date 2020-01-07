using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// Bool×ª»»Æ÷
	/// </summary>
	internal class OracleBoolConverter : ValueConverter<bool, short>
	{
		/// <summary>
		/// ÊµÀý»¯
		/// </summary>
		internal OracleBoolConverter()
			: base((Expression<Func<bool, short>>)((bool x) => ToStore(x)), (Expression<Func<short, bool>>)((short x) => FromStore(x)), (ConverterMappingHints)null)
		{
			//
		}

		private static short ToStore(bool val)
		{
			return (short)(val ? 1 : 0);
		}

		private static bool FromStore(short val)
		{
			if (val == 0)
			{
				return false;
			}
			return true;
		}
	}
}

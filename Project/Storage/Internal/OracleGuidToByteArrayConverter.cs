using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// GuidToByteArray×ª»»Æ÷
	/// </summary>
	internal class OracleGuidToByteArrayConverter : ValueConverter<Guid, byte[]>
	{
		/// <summary>
		/// ÊµÀý»¯
		/// </summary>
		internal OracleGuidToByteArrayConverter()
			: base((Expression<Func<Guid, byte[]>>)((Guid x) => ToStore(x)), (Expression<Func<byte[], Guid>>)((byte[] x) => FromStore(x)), (ConverterMappingHints)null)
		{
			//
		}

		private static byte[] ToStore(Guid val)
		{
			return val.ToByteArray();
		}

		private static Guid FromStore(byte[] val)
		{
			return new Guid(val);
		}
	}
}

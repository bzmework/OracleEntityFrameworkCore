using System;
using System.Data.Common;
using JetBrains.Annotations;

namespace Oracle.EntityFrameworkCore.Scaffolding.Internal
{
	/// <summary>
	/// DataReader��չ
	/// </summary>
	public static class OracleDataReaderExtension
	{
		/// <summary>
		/// ���ֵ
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader">DbDataReader</param>
		/// <param name="name">����</param>
		/// <returns></returns>
		public static T GetValueOrDefault<T>(
			[NotNull] this DbDataReader reader, 
			[NotNull] string name)
		{
			int ordinal = reader.GetOrdinal(name);
			if (!reader.IsDBNull(ordinal))
			{
				return (T)GetValue<T>(reader.GetValue(ordinal));
			}
			return default(T);
		}

		/// <summary>
		/// ���ֵ
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="record">DbDataRecord</param>
		/// <param name="name">����</param>
		/// <returns></returns>
		public static T GetValueOrDefault<T>(
			[NotNull] this DbDataRecord record,
			[NotNull] string name)
		{
			int ordinal = record.GetOrdinal(name);
			if (!record.IsDBNull(ordinal))
			{
				return (T)GetValue<T>(record.GetValue(ordinal));
			}
			return default(T);
		}

		/// <summary>
		/// ���ֵ
		/// </summary>
		/// <param name="record">DbDataRecord</param>
		/// <param name="name">����</param>
		/// <returns></returns>
		public static object GetValue(this DbDataRecord record, [NotNull] string name)
		{
			int ordinal = record.GetOrdinal(name);
			if (!record.IsDBNull(ordinal))
			{
				return record.GetValue(ordinal);
			}
			return null;
		}

		/// <summary>
		/// ���ֵ
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="valueRecord">ֵ��¼</param>
		/// <returns></returns>
		private static object GetValue<T>(object valueRecord)
		{
            switch (typeof(T).Name)
            {
                case nameof(Int32):
                    return Convert.ToInt32(valueRecord);
                case nameof(Boolean):
                    return Convert.ToBoolean(valueRecord);
                default:
                    return valueRecord;
            }
		}
	}
}

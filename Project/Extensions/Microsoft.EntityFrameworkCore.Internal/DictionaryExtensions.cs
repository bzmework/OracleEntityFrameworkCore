using JetBrains.Annotations;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.EntityFrameworkCore.Internal
{
	/// <summary>
	/// Dictionary扩展
	/// </summary>
	[DebuggerStepThrough]
	internal static class DictionaryExtensions
	{
		/// <summary>
		/// 获得或增加行
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="source">源字典</param>
		/// <param name="key">关键字</param>
		/// <returns></returns>
		public static TValue GetOrAddNew<TKey, TValue>(
			[NotNull] this IDictionary<TKey, TValue> source,
			[NotNull] TKey key) where TValue : new()
		{
			if (!source.TryGetValue(key, out TValue value))
			{
				value = new TValue();
				source.Add(key, value);
			}
			return value;
		}

		/// <summary>
		/// 查找
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="source">源字典</param>
		/// <param name="key">关键字</param>
		/// <returns></returns>
		public static TValue Find<TKey, TValue>(
			[NotNull] this IDictionary<TKey, TValue> source,
			[NotNull] TKey key)
		{
			if (source.TryGetValue(key, out TValue value))
			{
				return value;
			}
			return default(TValue);
		}
	}
}

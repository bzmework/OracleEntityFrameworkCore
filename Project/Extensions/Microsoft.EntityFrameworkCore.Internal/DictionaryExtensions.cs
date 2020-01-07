using JetBrains.Annotations;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.EntityFrameworkCore.Internal
{
	/// <summary>
	/// Dictionary��չ
	/// </summary>
	[DebuggerStepThrough]
	internal static class DictionaryExtensions
	{
		/// <summary>
		/// ��û�������
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="source">Դ�ֵ�</param>
		/// <param name="key">�ؼ���</param>
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
		/// ����
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="source">Դ�ֵ�</param>
		/// <param name="key">�ؼ���</param>
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

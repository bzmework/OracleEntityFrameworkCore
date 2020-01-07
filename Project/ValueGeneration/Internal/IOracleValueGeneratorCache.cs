using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Oracle.EntityFrameworkCore.ValueGeneration.Internal
{
	/// <summary>
	/// 值生成缓存
	/// </summary>
	public interface IOracleValueGeneratorCache : IValueGeneratorCache
	{
		/// <summary>
		/// 获得或增加序列状态
		/// </summary>
		/// <param name="property">属性</param>
		/// <returns></returns>
		OracleSequenceValueGeneratorState GetOrAddSequenceState([NotNull] IProperty property);
	}
}

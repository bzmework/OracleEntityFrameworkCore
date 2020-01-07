using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Oracle.EntityFrameworkCore.Storage.Internal;

namespace Oracle.EntityFrameworkCore.ValueGeneration.Internal
{
	/// <summary>
	/// 序列值生成器工厂接口
	/// </summary>
	public interface IOracleSequenceValueGeneratorFactory
	{
		/// <summary>
		/// 创建
		/// </summary>
		/// <param name="property">属性</param>
		/// <param name="generatorState">序列值生成器状态</param>
		/// <param name="connection">连接对象</param>
		/// <returns></returns>
		ValueGenerator Create(
			[NotNull] IProperty property, 
			[NotNull] OracleSequenceValueGeneratorState generatorState,
			[NotNull] IOracleConnection connection);
	}
}

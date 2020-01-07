using Microsoft.EntityFrameworkCore.Metadata;

namespace Oracle.EntityFrameworkCore.Metadata
{
	/// <summary>
	/// 属性注解
	/// </summary>
	public interface IOraclePropertyAnnotations : IRelationalPropertyAnnotations
	{
		/// <summary>
		/// Oracle值生成策略
		/// </summary>
		OracleValueGenerationStrategy? ValueGenerationStrategy
		{
			get;
		}

		/// <summary>
		/// HiLo序列名
		/// </summary>
		string HiLoSequenceName
		{
			get;
		}

		/// <summary>
		/// 查找HiLo序列
		/// </summary>
		/// <returns></returns>
		ISequence FindHiLoSequence();
	}
}

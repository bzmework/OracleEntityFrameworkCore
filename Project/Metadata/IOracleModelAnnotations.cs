using Microsoft.EntityFrameworkCore.Metadata;

namespace Oracle.EntityFrameworkCore.Metadata
{
	/// <summary>
	/// 模型注解
	/// </summary>
	public interface IOracleModelAnnotations : IRelationalModelAnnotations
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
	}
}

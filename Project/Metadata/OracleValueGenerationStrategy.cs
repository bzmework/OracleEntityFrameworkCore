namespace Oracle.EntityFrameworkCore.Metadata
{
	/// <summary>
	/// 值生成策略
	/// </summary>
	public enum OracleValueGenerationStrategy
	{
		/// <summary>
		/// 按序列HiLo
		/// </summary>
		SequenceHiLo,

		/// <summary>
		/// 按ID标识列
		/// </summary>
		IdentityColumn
	}
}

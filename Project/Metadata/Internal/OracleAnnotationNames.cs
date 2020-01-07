namespace Oracle.EntityFrameworkCore.Metadata.Internal
{
	/// <summary>
	/// Oracle注解名
	/// </summary>
	public static class OracleAnnotationNames
	{
		/// <summary>
		/// 前缀
		/// </summary>
		public const string Prefix = "Oracle:";

		/// <summary>
		/// 值生成策略
		/// </summary>
        public const string ValueGenerationStrategy = Prefix + "ValueGenerationStrategy";

		/// <summary>
		/// HiLo序列名
		/// </summary>
        public const string HiLoSequenceName = Prefix + "HiLoSequenceName";
	}
}

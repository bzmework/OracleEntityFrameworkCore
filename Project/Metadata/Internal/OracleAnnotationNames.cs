namespace Oracle.EntityFrameworkCore.Metadata.Internal
{
	/// <summary>
	/// Oracleע����
	/// </summary>
	public static class OracleAnnotationNames
	{
		/// <summary>
		/// ǰ׺
		/// </summary>
		public const string Prefix = "Oracle:";

		/// <summary>
		/// ֵ���ɲ���
		/// </summary>
        public const string ValueGenerationStrategy = Prefix + "ValueGenerationStrategy";

		/// <summary>
		/// HiLo������
		/// </summary>
        public const string HiLoSequenceName = Prefix + "HiLoSequenceName";
	}
}

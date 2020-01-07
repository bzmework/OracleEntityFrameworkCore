using Microsoft.EntityFrameworkCore.Metadata;

namespace Oracle.EntityFrameworkCore.Metadata
{
	/// <summary>
	/// ģ��ע��
	/// </summary>
	public interface IOracleModelAnnotations : IRelationalModelAnnotations
	{
		/// <summary>
		/// Oracleֵ���ɲ���
		/// </summary>
		OracleValueGenerationStrategy? ValueGenerationStrategy
		{
			get;
		}

		/// <summary>
		/// HiLo������
		/// </summary>
		string HiLoSequenceName
		{
			get;
		}
	}
}

using Microsoft.EntityFrameworkCore.Metadata;

namespace Oracle.EntityFrameworkCore.Metadata
{
	/// <summary>
	/// ����ע��
	/// </summary>
	public interface IOraclePropertyAnnotations : IRelationalPropertyAnnotations
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

		/// <summary>
		/// ����HiLo����
		/// </summary>
		/// <returns></returns>
		ISequence FindHiLoSequence();
	}
}

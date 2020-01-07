using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.ValueGeneration.Internal
{
	/// <summary>
	/// ����ֵ������״̬
	/// </summary>
	public class OracleSequenceValueGeneratorState : HiLoValueGeneratorState
	{
		/// <summary>
		/// ����
		/// </summary>
		public virtual ISequence Sequence
		{
			get;
		}

		/// <summary>
		/// ��ʼ��
		/// </summary>
		/// <param name="sequence">����</param>
		public OracleSequenceValueGeneratorState([NotNull] ISequence sequence)
			: base(Check.NotNull(sequence, nameof(sequence)).IncrementBy)
		{
			Sequence = sequence;
		}
	}
}

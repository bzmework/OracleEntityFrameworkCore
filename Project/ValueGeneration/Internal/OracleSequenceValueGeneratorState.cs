using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.ValueGeneration.Internal
{
	/// <summary>
	/// 序列值生成器状态
	/// </summary>
	public class OracleSequenceValueGeneratorState : HiLoValueGeneratorState
	{
		/// <summary>
		/// 序列
		/// </summary>
		public virtual ISequence Sequence
		{
			get;
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="sequence">序列</param>
		public OracleSequenceValueGeneratorState([NotNull] ISequence sequence)
			: base(Check.NotNull(sequence, nameof(sequence)).IncrementBy)
		{
			Sequence = sequence;
		}
	}
}

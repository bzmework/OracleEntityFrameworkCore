using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Oracle.EntityFrameworkCore.ValueGeneration.Internal
{
	/// <summary>
	/// ֵ���ɻ���
	/// </summary>
	public interface IOracleValueGeneratorCache : IValueGeneratorCache
	{
		/// <summary>
		/// ��û���������״̬
		/// </summary>
		/// <param name="property">����</param>
		/// <returns></returns>
		OracleSequenceValueGeneratorState GetOrAddSequenceState([NotNull] IProperty property);
	}
}

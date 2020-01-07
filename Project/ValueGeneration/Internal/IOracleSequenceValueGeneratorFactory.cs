using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Oracle.EntityFrameworkCore.Storage.Internal;

namespace Oracle.EntityFrameworkCore.ValueGeneration.Internal
{
	/// <summary>
	/// ����ֵ�����������ӿ�
	/// </summary>
	public interface IOracleSequenceValueGeneratorFactory
	{
		/// <summary>
		/// ����
		/// </summary>
		/// <param name="property">����</param>
		/// <param name="generatorState">����ֵ������״̬</param>
		/// <param name="connection">���Ӷ���</param>
		/// <returns></returns>
		ValueGenerator Create(
			[NotNull] IProperty property, 
			[NotNull] OracleSequenceValueGeneratorState generatorState,
			[NotNull] IOracleConnection connection);
	}
}

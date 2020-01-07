using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Collections.Generic;
using System.Text;

namespace Oracle.EntityFrameworkCore.Update.Internal
{
	/// <summary>
	/// UpdateSql�������ӿ�
	/// </summary>
	public interface IOracleUpdateSqlGenerator : IUpdateSqlGenerator, ISingletonUpdateSqlGenerator
	{
		/// <summary>
		/// ��������������
		/// </summary>
		/// <param name="commandStringBuilder">�����ַ���������</param>
		/// <param name="variablesInsert">�������</param>
		/// <param name="modificationCommands">�޸�����</param>
		/// <param name="commandPosition">����λ��</param>
		/// <param name="cursorPosition">�α�λ��</param>
		/// <param name="cursorPositionList">�α�λ���б�</param>
		/// <returns></returns>
		ResultSetMapping AppendBatchInsertOperation(StringBuilder commandStringBuilder, Dictionary<string, string> variablesInsert, IReadOnlyDictionary<ModificationCommand, int> modificationCommands, int commandPosition, ref int cursorPosition, List<int> cursorPositionList);

		/// <summary>
		/// ����������²���
		/// </summary>
		/// <param name="commandStringBuilder">�����ַ���������</param>
		/// <param name="variablesCommand">�������</param>
		/// <param name="modificationCommands">�޸�����</param>
		/// <param name="commandPosition">����λ��</param>
		/// <param name="cursorPosition">�α�λ��</param>
		/// <returns></returns>
		ResultSetMapping AppendBatchUpdateOperation(StringBuilder commandStringBuilder, StringBuilder variablesCommand, IReadOnlyList<ModificationCommand> modificationCommands, int commandPosition, ref int cursorPosition);

		/// <summary>
		/// �������ɾ������
		/// </summary>
		/// <param name="commandStringBuilder">�����ַ���������</param>
		/// <param name="variablesCommand">�������</param>
		/// <param name="modificationCommands">�޸�����</param>
		/// <param name="commandPosition">����λ��</param>
		/// <param name="cursorPosition">�α�λ��</param>
		/// <returns></returns>
		ResultSetMapping AppendBatchDeleteOperation(StringBuilder commandStringBuilder, StringBuilder variablesCommand, IReadOnlyList<ModificationCommand> modificationCommands, int commandPosition, ref int cursorPosition);
	}
}

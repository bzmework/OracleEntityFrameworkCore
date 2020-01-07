using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Collections.Generic;
using System.Text;

namespace Oracle.EntityFrameworkCore.Update.Internal
{
	/// <summary>
	/// UpdateSql生成器接口
	/// </summary>
	public interface IOracleUpdateSqlGenerator : IUpdateSqlGenerator, ISingletonUpdateSqlGenerator
	{
		/// <summary>
		/// 添加批量插入操作
		/// </summary>
		/// <param name="commandStringBuilder">命令字符串创建器</param>
		/// <param name="variablesInsert">插入变量</param>
		/// <param name="modificationCommands">修改命令</param>
		/// <param name="commandPosition">命令位置</param>
		/// <param name="cursorPosition">游标位置</param>
		/// <param name="cursorPositionList">游标位置列表</param>
		/// <returns></returns>
		ResultSetMapping AppendBatchInsertOperation(StringBuilder commandStringBuilder, Dictionary<string, string> variablesInsert, IReadOnlyDictionary<ModificationCommand, int> modificationCommands, int commandPosition, ref int cursorPosition, List<int> cursorPositionList);

		/// <summary>
		/// 添加批量更新操作
		/// </summary>
		/// <param name="commandStringBuilder">命令字符串创建器</param>
		/// <param name="variablesCommand">命令变量</param>
		/// <param name="modificationCommands">修改命令</param>
		/// <param name="commandPosition">命令位置</param>
		/// <param name="cursorPosition">游标位置</param>
		/// <returns></returns>
		ResultSetMapping AppendBatchUpdateOperation(StringBuilder commandStringBuilder, StringBuilder variablesCommand, IReadOnlyList<ModificationCommand> modificationCommands, int commandPosition, ref int cursorPosition);

		/// <summary>
		/// 添加批量删除操作
		/// </summary>
		/// <param name="commandStringBuilder">命令字符串创建器</param>
		/// <param name="variablesCommand">命令变量</param>
		/// <param name="modificationCommands">修改命令</param>
		/// <param name="commandPosition">命令位置</param>
		/// <param name="cursorPosition">游标位置</param>
		/// <returns></returns>
		ResultSetMapping AppendBatchDeleteOperation(StringBuilder commandStringBuilder, StringBuilder variablesCommand, IReadOnlyList<ModificationCommand> modificationCommands, int commandPosition, ref int cursorPosition);
	}
}

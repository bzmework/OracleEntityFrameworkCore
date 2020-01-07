using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Migrations
{
	/// <summary>
	/// 迁移命令 
	/// </summary>
	public class OracleMigrationCommand : MigrationCommand
	{
		private readonly IRelationalCommand _relationalCommand;

		/// <summary>
		/// 事务抑制
		/// </summary>
		public override bool TransactionSuppressed
		{
			get;
		}

		/// <summary>
		/// 命令文本
		/// </summary>
		public override string CommandText
		{
			get { return _relationalCommand.CommandText; }
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="relationalCommand">关系命令</param>
		/// <param name="transactionSuppressed">事务抑制</param>
		public OracleMigrationCommand(
			[NotNull] IRelationalCommand relationalCommand,
			bool transactionSuppressed = false)
			: base(relationalCommand, transactionSuppressed)
		{
			Check.NotNull(relationalCommand, nameof(relationalCommand));
			_relationalCommand = relationalCommand;
			TransactionSuppressed = transactionSuppressed;
		}

		/// <summary>
		/// 执行并返回一行一列数据
		/// </summary>
		/// <param name="connection">连接对象</param>
		/// <param name="parameterValues">参数值</param>
		/// <returns></returns>
		public object ExecuteScalar(
			[NotNull] IRelationalConnection connection, 
			[CanBeNull] IReadOnlyDictionary<string, object> parameterValues = null)
		{
			return _relationalCommand.ExecuteScalar(Check.NotNull(connection, nameof(connection)), parameterValues);
		}
	}
}

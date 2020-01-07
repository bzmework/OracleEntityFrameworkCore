using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Migrations
{
	/// <summary>
	/// Ǩ������ 
	/// </summary>
	public class OracleMigrationCommand : MigrationCommand
	{
		private readonly IRelationalCommand _relationalCommand;

		/// <summary>
		/// ��������
		/// </summary>
		public override bool TransactionSuppressed
		{
			get;
		}

		/// <summary>
		/// �����ı�
		/// </summary>
		public override string CommandText
		{
			get { return _relationalCommand.CommandText; }
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="relationalCommand">��ϵ����</param>
		/// <param name="transactionSuppressed">��������</param>
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
		/// ִ�в�����һ��һ������
		/// </summary>
		/// <param name="connection">���Ӷ���</param>
		/// <param name="parameterValues">����ֵ</param>
		/// <returns></returns>
		public object ExecuteScalar(
			[NotNull] IRelationalConnection connection, 
			[CanBeNull] IReadOnlyDictionary<string, object> parameterValues = null)
		{
			return _relationalCommand.ExecuteScalar(Check.NotNull(connection, nameof(connection)), parameterValues);
		}
	}
}

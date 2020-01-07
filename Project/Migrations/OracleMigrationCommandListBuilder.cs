using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Migrations
{
	/// <summary>
	/// 迁移命令列表创建器
	/// </summary>
	public class OracleMigrationCommandListBuilder : MigrationCommandListBuilder
	{
		private IRelationalCommandBuilder _commandBuilder;

		private readonly List<OracleMigrationCommand> _commands = new List<OracleMigrationCommand>();

		private readonly IRelationalCommandBuilderFactory _commandBuilderFactory;

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="commandBuilderFactory">命令创建器工厂</param>
		public OracleMigrationCommandListBuilder(
			[NotNull] IRelationalCommandBuilderFactory commandBuilderFactory)
			: base(commandBuilderFactory)
		{
			Check.NotNull(commandBuilderFactory, nameof(commandBuilderFactory));
			_commandBuilderFactory = commandBuilderFactory;
			_commandBuilder = commandBuilderFactory.Create();
		}

		/// <summary>
		/// 获得命令列表
		/// </summary>
		/// <returns></returns>
		public override IReadOnlyList<MigrationCommand> GetCommandList()
		{
			return _commands;
		}

		/// <summary>
		/// 结束命令
		/// </summary>
		/// <param name="suppressTransaction">事务抑制</param>
		/// <returns></returns>
		public override MigrationCommandListBuilder EndCommand(bool suppressTransaction = false)
		{
			if (_commandBuilder.GetLength() != 0)
			{
				_commands.Add(new OracleMigrationCommand(_commandBuilder.Build(), suppressTransaction));
				_commandBuilder = _commandBuilderFactory.Create();
			}
			return this;
		}

		/// <summary>
		/// 添加命令
		/// </summary>
		/// <param name="o">对象</param>
		/// <returns></returns>
		public override MigrationCommandListBuilder Append([NotNull] object o)
		{
			Check.NotNull(o, nameof(o));
			_commandBuilder.Append(o);
			return this;
		}

		/// <summary>
		/// 添加行
		/// </summary>
		/// <returns></returns>
		public override MigrationCommandListBuilder AppendLine()
		{
			_commandBuilder.AppendLine();
			return this;
		}

		/// <summary>
		/// 添加行
		/// </summary>
		/// <param name="o">对象</param>
		/// <returns></returns>
		public override MigrationCommandListBuilder AppendLine([NotNull] object o)
		{
			Check.NotNull(o, nameof(o));
			_commandBuilder.AppendLine(o);
			return this;
		}

		/// <summary>
		/// 添加行s
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override MigrationCommandListBuilder AppendLines([NotNull] object o)
		{
			Check.NotNull(o, nameof(o));
			_commandBuilder.AppendLines(o);
			return this;
		}

		/// <summary>
		/// 缩进
		/// </summary>
		/// <returns></returns>
		public override IDisposable Indent()
		{
			return _commandBuilder.Indent();
		}

		/// <summary>
		/// 增加缩进
		/// </summary>
		/// <returns></returns>
		public override MigrationCommandListBuilder IncrementIndent()
		{
			_commandBuilder.IncrementIndent();
			return this;
		}

		/// <summary>
		/// 减少缩进
		/// </summary>
		/// <returns></returns>
		public override MigrationCommandListBuilder DecrementIndent()
		{
			_commandBuilder.DecrementIndent();
			return this;
		}
	}
}

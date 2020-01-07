using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Oracle.EntityFrameworkCore.Storage.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.ValueGeneration.Internal
{
	/// <summary>
	/// 序列HiLoValue生成器
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	public class OracleSequenceHiLoValueGenerator<TValue> : HiLoValueGenerator<TValue>
	{
		private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;

		private readonly IUpdateSqlGenerator _sqlGenerator;

		private readonly IOracleConnection _connection;

		private readonly ISequence _sequence;

		/// <summary>
		/// 生成临时值
		/// </summary>
		public override bool GeneratesTemporaryValues
		{
			get { return false; }
		}

		/// <summary>
		/// 实例化HiLoValue生成器
		/// </summary>
		/// <param name="rawSqlCommandBuilder">RawSql命令创建器</param>
		/// <param name="sqlGenerator">UpdateSql生成器</param>
		/// <param name="generatorState">序列值生成器状态</param>
		/// <param name="connection">连接对象</param>
		public OracleSequenceHiLoValueGenerator(
			[NotNull] IRawSqlCommandBuilder rawSqlCommandBuilder, 
			[NotNull] IUpdateSqlGenerator sqlGenerator, 
			[NotNull] OracleSequenceValueGeneratorState generatorState,
			[NotNull] IOracleConnection connection)
			: base((HiLoValueGeneratorState)generatorState)
		{
			Check.NotNull(rawSqlCommandBuilder, nameof(rawSqlCommandBuilder));
			Check.NotNull(sqlGenerator, nameof(sqlGenerator));
			Check.NotNull(connection, nameof(connection));
			_sequence = generatorState.Sequence;
			_rawSqlCommandBuilder = rawSqlCommandBuilder;
			_sqlGenerator = sqlGenerator;
			_connection = connection;
		}

		/// <summary>
		/// 获得新低值
		/// </summary>
		/// <returns></returns>
		protected override long GetNewLowValue()
		{
			var cmd = _rawSqlCommandBuilder
			.Build(_sqlGenerator.GenerateNextSequenceValueOperation(_sequence.Name, _sequence.Schema))
			.ExecuteScalar(_connection);

			long retValue = (long)Convert.ChangeType(cmd, typeof(long), CultureInfo.InvariantCulture);

			return retValue;
		}

		/// <summary>
		/// 异步获得新低值
		/// </summary>
		/// <param name="cancellationToken">取消Token</param>
		/// <returns></returns>
		protected override async Task<long> GetNewLowValueAsync(CancellationToken cancellationToken = default)
		{
			var cmd = await _rawSqlCommandBuilder
			.Build(_sqlGenerator.GenerateNextSequenceValueOperation(_sequence.Name, _sequence.Schema))
			.ExecuteScalarAsync(_connection, cancellationToken: cancellationToken);

			long retValue = (long)Convert.ChangeType(cmd, typeof(long), CultureInfo.InvariantCulture);
			
			return retValue;
		}
	}
}

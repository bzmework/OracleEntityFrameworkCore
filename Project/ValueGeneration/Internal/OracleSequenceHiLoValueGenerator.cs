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
	/// ����HiLoValue������
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	public class OracleSequenceHiLoValueGenerator<TValue> : HiLoValueGenerator<TValue>
	{
		private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;

		private readonly IUpdateSqlGenerator _sqlGenerator;

		private readonly IOracleConnection _connection;

		private readonly ISequence _sequence;

		/// <summary>
		/// ������ʱֵ
		/// </summary>
		public override bool GeneratesTemporaryValues
		{
			get { return false; }
		}

		/// <summary>
		/// ʵ����HiLoValue������
		/// </summary>
		/// <param name="rawSqlCommandBuilder">RawSql�������</param>
		/// <param name="sqlGenerator">UpdateSql������</param>
		/// <param name="generatorState">����ֵ������״̬</param>
		/// <param name="connection">���Ӷ���</param>
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
		/// ����µ�ֵ
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
		/// �첽����µ�ֵ
		/// </summary>
		/// <param name="cancellationToken">ȡ��Token</param>
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

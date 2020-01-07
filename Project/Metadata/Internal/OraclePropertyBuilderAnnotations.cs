using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Oracle.EntityFrameworkCore.Metadata.Internal
{
	/// <summary>
	/// ���Դ�����ע��
	/// </summary>
	public class OraclePropertyBuilderAnnotations : OraclePropertyAnnotations
	{
		/// <summary>
		/// ����ע��
		/// </summary>
		protected new virtual RelationalAnnotationsBuilder Annotations
		{
			get { return (RelationalAnnotationsBuilder)base.Annotations; }
		}

		/// <summary>
		/// Ӧ�������ͻ
		/// </summary>
		protected override bool ShouldThrowOnConflict
		{
			get { return false; }
		}

		/// <summary>
		/// Ӧ��������Ч����
		/// </summary>
		protected override bool ShouldThrowOnInvalidConfiguration
		{
			get { return Annotations.ConfigurationSource == ConfigurationSource.Explicit; }
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="internalBuilder"></param>
		/// <param name="configurationSource"></param>
		public OraclePropertyBuilderAnnotations(
			[NotNull] InternalPropertyBuilder internalBuilder,
			ConfigurationSource configurationSource)
			: base(new RelationalAnnotationsBuilder(internalBuilder, configurationSource))
		{
			//
		}

		/// <summary>
		/// ��������
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public new virtual bool ColumnName([CanBeNull] string value)
		{
			return SetColumnName(value);
		}

		/// <summary>
		/// ����������
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public new virtual bool ColumnType([CanBeNull] string value)
		{
			return SetColumnType(value);
		}

		/// <summary>
		/// ����Ĭ��ֵSQL
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public new virtual bool DefaultValueSql([CanBeNull] string value)
		{
			return SetDefaultValueSql(value);
		}

		/// <summary>
		/// ���ü�����SQL
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public new virtual bool ComputedColumnSql([CanBeNull] string value)
		{
			return SetComputedColumnSql(value);
		}

		/// <summary>
		/// ����Ĭ��ֵ
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public new virtual bool DefaultValue([CanBeNull] object value)
		{
			return SetDefaultValue(value);
		}

		/// <summary>
		/// ����HiLo������
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public new virtual bool HiLoSequenceName([CanBeNull] string value)
		{
			return SetHiLoSequenceName(value);
		}

		/// <summary>
		/// ����ֵ���ɲ���
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public new virtual bool ValueGenerationStrategy(OracleValueGenerationStrategy? value)
		{
			if (!SetValueGenerationStrategy(value))
			{
				return false;
			}
			if (!value.HasValue)
			{
				HiLoSequenceName(null);
			}
			else if (value.Value == OracleValueGenerationStrategy.IdentityColumn)
			{
				HiLoSequenceName(null);
			}
			return true;
		}
	}
}

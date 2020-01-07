using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Oracle.EntityFrameworkCore.Metadata.Internal
{
	/// <summary>
	/// 属性创建器注解
	/// </summary>
	public class OraclePropertyBuilderAnnotations : OraclePropertyAnnotations
	{
		/// <summary>
		/// 返回注解
		/// </summary>
		protected new virtual RelationalAnnotationsBuilder Annotations
		{
			get { return (RelationalAnnotationsBuilder)base.Annotations; }
		}

		/// <summary>
		/// 应该引起冲突
		/// </summary>
		protected override bool ShouldThrowOnConflict
		{
			get { return false; }
		}

		/// <summary>
		/// 应该引发无效配置
		/// </summary>
		protected override bool ShouldThrowOnInvalidConfiguration
		{
			get { return Annotations.ConfigurationSource == ConfigurationSource.Explicit; }
		}

		/// <summary>
		/// 实例化
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
		/// 设置列名
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public new virtual bool ColumnName([CanBeNull] string value)
		{
			return SetColumnName(value);
		}

		/// <summary>
		/// 设置列类型
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public new virtual bool ColumnType([CanBeNull] string value)
		{
			return SetColumnType(value);
		}

		/// <summary>
		/// 设置默认值SQL
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public new virtual bool DefaultValueSql([CanBeNull] string value)
		{
			return SetDefaultValueSql(value);
		}

		/// <summary>
		/// 设置计算列SQL
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public new virtual bool ComputedColumnSql([CanBeNull] string value)
		{
			return SetComputedColumnSql(value);
		}

		/// <summary>
		/// 设置默认值
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public new virtual bool DefaultValue([CanBeNull] object value)
		{
			return SetDefaultValue(value);
		}

		/// <summary>
		/// 设置HiLo序列名
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public new virtual bool HiLoSequenceName([CanBeNull] string value)
		{
			return SetHiLoSequenceName(value);
		}

		/// <summary>
		/// 设置值生成策略
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

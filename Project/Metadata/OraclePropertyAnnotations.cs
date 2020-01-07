using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Oracle.EntityFrameworkCore.Internal;
using Oracle.EntityFrameworkCore.Metadata.Internal;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Metadata
{
	/// <summary>
	/// 属性注解
	/// </summary>
	public class OraclePropertyAnnotations : RelationalPropertyAnnotations, IOraclePropertyAnnotations, IRelationalPropertyAnnotations
	{
		/// <summary>
		/// HiLo序列名
		/// </summary>
		public virtual string HiLoSequenceName
		{
			get
			{
				return (string)Annotations.Metadata[OracleAnnotationNames.HiLoSequenceName];
			}
			[param: CanBeNull]
			set
			{
				SetHiLoSequenceName(value);
			}
		}

		/// <summary>
		/// Oracle值生成策略
		/// </summary>
		public virtual OracleValueGenerationStrategy? ValueGenerationStrategy
		{
			get
			{
				return GetOracleValueGenerationStrategy(fallbackToModel: true);
			}
			[param: CanBeNull]
			set
			{
				SetValueGenerationStrategy(value);
			}
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="property">属性</param>
		public OraclePropertyAnnotations([NotNull] IProperty property)
			: base(property)
		{
			//
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="annotations">注解</param>

		protected OraclePropertyAnnotations([NotNull] RelationalAnnotations annotations)
			: base(annotations)
		{
			//
		}

		/// <summary>
		/// 设置HiLo序列名
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		protected virtual bool SetHiLoSequenceName([CanBeNull] string value)
		{
			return Annotations.SetAnnotation(OracleAnnotationNames.HiLoSequenceName, Check.NullButNotEmpty(value, nameof(value)));
		}

		/// <summary>
		/// 查找HiLo序列名
		/// </summary>
		/// <returns></returns>
		public virtual ISequence FindHiLoSequence()
		{
			IOracleModelAnnotations oracleModelAnnotations = Property.DeclaringEntityType.Model.Oracle();
			if (ValueGenerationStrategy != OracleValueGenerationStrategy.SequenceHiLo)
			{
				return null;
			}
			string name = HiLoSequenceName ?? oracleModelAnnotations.HiLoSequenceName ?? OracleModelAnnotations.DefaultHiLoSequenceName;
			return oracleModelAnnotations.FindSequence(name);
		}

		/// <summary>
		/// Oracle值生成策略
		/// </summary>
		/// <param name="fallbackToModel">回退模式</param>
		/// <returns></returns>
		public virtual OracleValueGenerationStrategy? GetOracleValueGenerationStrategy(bool fallbackToModel)
		{
			OracleValueGenerationStrategy? result = (OracleValueGenerationStrategy?)Annotations.Metadata[OracleAnnotationNames.ValueGenerationStrategy];
			if (result.HasValue)
			{
				return result;
			}
			IRelationalPropertyAnnotations relationalPropertyAnnotations = Property.Relational();
			if (!fallbackToModel || Property.ValueGenerated != ValueGenerated.OnAdd || relationalPropertyAnnotations.DefaultValue != null || relationalPropertyAnnotations.DefaultValueSql != null || relationalPropertyAnnotations.ComputedColumnSql != null)
			{
				return null;
			}
			OracleValueGenerationStrategy? valueGenerationStrategy = Property.DeclaringEntityType.Model.Oracle().ValueGenerationStrategy;
			if (valueGenerationStrategy == OracleValueGenerationStrategy.SequenceHiLo && IsCompatibleSequenceHiLo(Property))
			{
				return OracleValueGenerationStrategy.SequenceHiLo;
			}
			if (valueGenerationStrategy == OracleValueGenerationStrategy.IdentityColumn && IsCompatibleIdentityColumn(Property))
			{
				return OracleValueGenerationStrategy.IdentityColumn;
			}
			return null;
		}

		/// <summary>
		/// 设置值生成策略
		/// </summary>
		/// <param name="value">值生成策略</param>
		/// <returns></returns>
		protected virtual bool SetValueGenerationStrategy(OracleValueGenerationStrategy? value)
		{
			if (value.HasValue)
			{
				Type clrType = Property.ClrType;
				if (value == OracleValueGenerationStrategy.IdentityColumn && !IsCompatibleIdentityColumn(Property))
				{
					if (ShouldThrowOnInvalidConfiguration)
					{
						throw new ArgumentException(OracleStrings.IdentityBadType(Property.Name, Property.DeclaringEntityType.DisplayName(), clrType.ShortDisplayName()));
					}
					return false;
				}
				if (value == OracleValueGenerationStrategy.SequenceHiLo && !IsCompatibleSequenceHiLo(Property))
				{
					if (ShouldThrowOnInvalidConfiguration)
					{
						throw new ArgumentException(OracleStrings.SequenceBadType(Property.Name, Property.DeclaringEntityType.DisplayName(), clrType.ShortDisplayName()));
					}
					return false;
				}
			}
			if (!CanSetValueGenerationStrategy(value))
			{
				return false;
			}
			if (!ShouldThrowOnConflict && ValueGenerationStrategy != value && value.HasValue)
			{
				ClearAllServerGeneratedValues();
			}
			return Annotations.SetAnnotation(OracleAnnotationNames.ValueGenerationStrategy, value);
		}

		/// <summary>
		/// 是否能设置值生成策略
		/// </summary>
		/// <param name="value">值生成策略</param>
		/// <returns></returns>
		protected virtual bool CanSetValueGenerationStrategy(OracleValueGenerationStrategy? value)
		{
			if (GetOracleValueGenerationStrategy(fallbackToModel: false) == value)
			{
				return true;
			}
			if (!Annotations.CanSetAnnotation(OracleAnnotationNames.ValueGenerationStrategy, value))
			{
				return false;
			}
			if (ShouldThrowOnConflict)
			{
				if (GetDefaultValue(fallback: false) != null)
				{
					throw new InvalidOperationException(RelationalStrings.ConflictingColumnServerGeneration(nameof(ValueGenerationStrategy), Property.Name, nameof(DefaultValue)));
				}
				if (GetDefaultValueSql(fallback: false) != null)
				{
					throw new InvalidOperationException(RelationalStrings.ConflictingColumnServerGeneration(nameof(ValueGenerationStrategy), Property.Name, nameof(DefaultValueSql)));
				}
				if (GetComputedColumnSql(fallback: false) != null)
				{
					throw new InvalidOperationException(RelationalStrings.ConflictingColumnServerGeneration(nameof(ValueGenerationStrategy), Property.Name, nameof(ComputedColumnSql)));
				}
			}
			else if (value.HasValue && (!CanSetDefaultValue(null) || !CanSetDefaultValueSql(null) || !CanSetComputedColumnSql(null)))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 获得默认值
		/// </summary>
		/// <param name="fallback">回退</param>
		/// <returns></returns>
		protected override object GetDefaultValue(bool fallback)
		{
			if (fallback && ValueGenerationStrategy.HasValue)
			{
				return null;
			}
			return base.GetDefaultValue(fallback);
		}

		/// <summary>
		/// 是否能设置默认值
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		protected override bool CanSetDefaultValue(object value)
		{
			if (ShouldThrowOnConflict)
			{
				if (ValueGenerationStrategy.HasValue)
				{
					throw new InvalidOperationException(RelationalStrings.ConflictingColumnServerGeneration(nameof(DefaultValue), Property.Name, nameof(ValueGenerationStrategy)));
				}
			}
			else if (value != null && !CanSetValueGenerationStrategy(null))
			{
				return false;
			}
			return base.CanSetDefaultValue(value);
		}

		/// <summary>
		/// 获得默认值SQL
		/// </summary>
		/// <param name="fallback">回退</param>
		/// <returns></returns>
		protected override string GetDefaultValueSql(bool fallback)
		{
			if (fallback && ValueGenerationStrategy.HasValue)
			{
				return null;
			}
			return base.GetDefaultValueSql(fallback);
		}

		/// <summary>
		/// 是否能设置默认值SQL
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		protected override bool CanSetDefaultValueSql(string value)
		{
			if (ShouldThrowOnConflict)
			{
				if (ValueGenerationStrategy.HasValue)
				{
					throw new InvalidOperationException(RelationalStrings.ConflictingColumnServerGeneration(nameof(DefaultValueSql), Property.Name, nameof(ValueGenerationStrategy)));
				}
			}
			else if (value != null && !CanSetValueGenerationStrategy(null))
			{
				return false;
			}
			return base.CanSetDefaultValueSql(value);
		}

		/// <summary>
		/// 获取计算列SQL
		/// </summary>
		/// <param name="fallback">回退</param>
		/// <returns></returns>
		protected override string GetComputedColumnSql(bool fallback)
		{
			if (fallback && ValueGenerationStrategy.HasValue)
			{
				return null;
			}
			return base.GetComputedColumnSql(fallback);
		}

		/// <summary>
		/// 是否能设置计算列SQL
		/// </summary>
		/// <param name="value">值</param>
		/// <returns></returns>
		protected override bool CanSetComputedColumnSql(string value)
		{
			if (ShouldThrowOnConflict)
			{
				if (ValueGenerationStrategy.HasValue)
				{
					throw new InvalidOperationException(RelationalStrings.ConflictingColumnServerGeneration(nameof(ComputedColumnSql), Property.Name, nameof(ValueGenerationStrategy)));
				}
			}
			else if (value != null && !CanSetValueGenerationStrategy(null))
			{
				return false;
			}
			return base.CanSetComputedColumnSql(value);
		}

		/// <summary>
		/// 清除服务器所有生成的值
		/// </summary>
		protected override void ClearAllServerGeneratedValues()
		{
			SetValueGenerationStrategy(null);
			base.ClearAllServerGeneratedValues();
		}

		private static bool IsCompatibleIdentityColumn(IProperty property)
		{
			Type clrType = property.ClrType;
			if (clrType.IsInteger() || clrType == typeof(decimal))
			{
				return !HasConverter(property);
			}
			return false;
		}

		private static bool IsCompatibleSequenceHiLo(IProperty property)
		{
			if (property.ClrType.IsInteger())
			{
				return !HasConverter(property);
			}
			return false;
		}

		private static bool HasConverter(IProperty property)
		{
			return (property.FindMapping()?.Converter ?? property.GetValueConverter()) != null;
		}
	}
}

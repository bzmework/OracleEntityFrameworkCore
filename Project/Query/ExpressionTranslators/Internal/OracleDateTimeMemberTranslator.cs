using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// DateTime成员转换器
	/// </summary>
	public class OracleDateTimeMemberTranslator : IMemberTranslator
	{
		private static readonly Dictionary<string, string> _datePartMapping = new Dictionary<string, string>
		{
			{ nameof(DateTime.Year), "YEAR" },
			{ nameof(DateTime.Month), "MONTH" },
			{ nameof(DateTime.Day), "DAY" },
			{ nameof(DateTime.Hour), "HOUR" },
			{ nameof(DateTime.Minute), "MINUTE" },
			{ nameof(DateTime.Second), "SECOND" },
		};

		/// <summary>
		/// 转换
		/// </summary>
		/// <param name="memberExpression">成员表达式</param>
		/// <returns></returns>
		public virtual Expression Translate(MemberExpression memberExpression)
		{
			Type declaringType = memberExpression.Member.DeclaringType;
			if (declaringType == typeof(DateTime) || declaringType == typeof(DateTimeOffset))
			{
				string name = memberExpression.Member.Name;
				if (_datePartMapping.TryGetValue(name, out string value))
				{
					if (declaringType == typeof(DateTimeOffset) && (string.Equals(value, "HOUR") || string.Equals(value, "MINUTE")))
					{
						// TODO: See issue#10515
						// datePart = "TIMEZONE_" + datePart;
						return null;
					}
					return new SqlFunctionExpression("EXTRACT", memberExpression.Type, new Expression[2]
					{
						new SqlFragmentExpression(value),
						memberExpression.Expression
					});
				}

				if (name == nameof(DateTime.Now))
				{
					SqlFragmentExpression sqlDateExp = new SqlFragmentExpression("SYSDATE");
					if (!(declaringType == typeof(DateTimeOffset)))
					{
						return sqlDateExp;
					}
					return new ExplicitCastExpression(sqlDateExp, typeof(DateTimeOffset));
				}
				if (name == nameof(DateTime.UtcNow))
				{
					SqlFragmentExpression sysTimeStampExp = new SqlFragmentExpression("SYSTIMESTAMP");
					if (!(declaringType == typeof(DateTimeOffset)))
					{
						return sysTimeStampExp;
					}
					return new ExplicitCastExpression(sysTimeStampExp, typeof(DateTimeOffset));
				}
				if (name == nameof(DateTime.Date))
				{
					return new SqlFunctionExpression("TRUNC", memberExpression.Type, new Expression[1]
					{
						memberExpression.Expression
					});
				}
				if (name == nameof(DateTime.Today))
				{
					return new SqlFunctionExpression("TRUNC", memberExpression.Type, new SqlFragmentExpression[1]
					{
						new SqlFragmentExpression("SYSDATE")
					});
				}
			}
			return null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// String转换器
	/// </summary>
	public class OracleObjectToStringTranslator : IMethodCallTranslator
	{
		private const int DefaultLength = 100;

		private static readonly Dictionary<Type, string> _typeMapping = new Dictionary<Type, string>
		{
			{ typeof(int), "VARCHAR2(11)" },
			{ typeof(long), "VARCHAR2(20)" },
			{ typeof(DateTime), $"VARCHAR2({DefaultLength})" },
			{ typeof(bool), "VARCHAR2(5)" },
			{ typeof(byte), "VARCHAR2(3)" },
			{ typeof(byte[]), $"VARCHAR2({DefaultLength})" },
			{ typeof(double), $"VARCHAR2({DefaultLength})" },
			{ typeof(DateTimeOffset), $"VARCHAR2({DefaultLength})" },
			{ typeof(char), "VARCHAR2(1)" },
			{ typeof(short), "VARCHAR2(6)" },
			{ typeof(float), $"VARCHAR2({DefaultLength})" },
			{ typeof(decimal), $"VARCHAR2({DefaultLength})" },
			{ typeof(TimeSpan), $"VARCHAR2({DefaultLength})" },
			{ typeof(uint), "VARCHAR2(10)" },
			{ typeof(ushort), "VARCHAR2(5)" },
			{ typeof(ulong), "VARCHAR2(19)" },
			{ typeof(sbyte), "VARCHAR2(4)" }
		};

		/// <summary>
		/// 转换
		/// </summary>
		/// <param name="methodCallExpression">方法调用表达式</param>
		/// <returns></returns>
		public virtual Expression Translate(MethodCallExpression methodCallExpression)
		{
            if (methodCallExpression.Method.Name == nameof(ToString)
                && methodCallExpression.Arguments.Count == 0
                && methodCallExpression.Object != null
                && _typeMapping.TryGetValue(
                    methodCallExpression.Object.Type.UnwrapNullableType(),
                    out var storeType))
            {
                return new SqlFunctionExpression(
                    functionName: "CAST",
                    returnType: methodCallExpression.Type,
                    arguments: new[]
                    {
                        methodCallExpression.Object,
                        new SqlFragmentExpression(storeType)
                    });
            }

			return null;
		}
	}
}

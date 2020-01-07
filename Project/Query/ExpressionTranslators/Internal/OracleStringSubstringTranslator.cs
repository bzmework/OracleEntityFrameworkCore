using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// Substring转换器
	/// </summary>
	public class OracleStringSubstringTranslator : IMethodCallTranslator
	{
		private static readonly MethodInfo _methodInfo = typeof(string).GetRuntimeMethod(nameof(string.Substring), new[] 
		{ 
			typeof(int), 
			typeof(int) 
		});

		/// <summary>
		/// 转换
		/// </summary>
		/// <param name="methodCallExpression">方法调用表达式</param>
		/// <returns></returns>
		public virtual Expression Translate(MethodCallExpression methodCallExpression)
		{
			if (_methodInfo.Equals(methodCallExpression.Method)
				// Oracle returns null if length is not greater than 0
				&& (!(methodCallExpression.Arguments[1] is ConstantExpression constantExpression)
				|| (int)constantExpression.Value > 0))
			{
				return new SqlFunctionExpression(
					"SUBSTR",
					methodCallExpression.Type,
					new Expression[]
					{
						methodCallExpression.Object,
                        // Accommodate for Oracle assumption of 1-based string indexes
                        methodCallExpression.Arguments[0].NodeType == ExpressionType.Constant
							? (Expression)Expression.Constant(
								(int)((ConstantExpression)methodCallExpression.Arguments[0]).Value + 1)
							: Expression.Add(
								methodCallExpression.Arguments[0],
								Expression.Constant(1)),
						methodCallExpression.Arguments[1]
					});
			}

			return null;
		}
	}
}

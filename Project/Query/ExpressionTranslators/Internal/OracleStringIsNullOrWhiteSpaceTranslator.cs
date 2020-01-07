using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// IsNullOrWhiteSpace转换器
	/// </summary>
	public class OracleStringIsNullOrWhiteSpaceTranslator : IMethodCallTranslator
	{
		private static readonly MethodInfo _methodInfo = typeof(string).GetRuntimeMethod(nameof(string.IsNullOrWhiteSpace), new[] 
		{ 
			typeof(string) 
		});

		/// <summary>
		/// 转换
		/// </summary>
		/// <param name="methodCallExpression">方法调用表达式</param>
		/// <returns></returns>
		public virtual Expression Translate(MethodCallExpression methodCallExpression)
		{
			if (methodCallExpression.Method.Equals(_methodInfo))
			{
				Expression expression = methodCallExpression.Arguments[0];
				return Expression.MakeBinary(ExpressionType.OrElse, new IsNullExpression(expression), Expression.Equal(new SqlFunctionExpression("LTRIM", typeof(string), new SqlFunctionExpression[1]
				{
					new SqlFunctionExpression("RTRIM", typeof(string), new Expression[1]
					{
						expression
					})
				}), Expression.Constant("", typeof(string))));
			}
			return null;
		}
	}
}

using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// IsNullOrWhiteSpaceת����
	/// </summary>
	public class OracleStringIsNullOrWhiteSpaceTranslator : IMethodCallTranslator
	{
		private static readonly MethodInfo _methodInfo = typeof(string).GetRuntimeMethod(nameof(string.IsNullOrWhiteSpace), new[] 
		{ 
			typeof(string) 
		});

		/// <summary>
		/// ת��
		/// </summary>
		/// <param name="methodCallExpression">�������ñ��ʽ</param>
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

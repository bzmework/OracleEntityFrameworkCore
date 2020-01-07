using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// EndswithoOptimizedת���� 
	/// </summary>
	public class OracleEndsWithOptimizedTranslator : IMethodCallTranslator
	{
		private static readonly MethodInfo _methodInfo = typeof(string).GetRuntimeMethod(nameof(string.EndsWith), new[] 
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
			if (object.Equals(methodCallExpression.Method, _methodInfo))
			{
				Expression expression = methodCallExpression.Arguments[0];
				NullCompensatedExpression nullCompensatedExpression = new NullCompensatedExpression(Expression.Equal(new SqlFunctionExpression("SUBSTR", methodCallExpression.Object.Type, new Expression[2]
				{
					methodCallExpression.Object,
					Expression.Negate(new SqlFunctionExpression("LENGTH", typeof(int), new Expression[1]
					{
						expression
					}))
				}), expression));

				ConstantExpression constantExpression;
				if ((constantExpression = (expression as ConstantExpression)) == null)
				{
					return Expression.OrElse(nullCompensatedExpression, Expression.Equal(expression, Expression.Constant(string.Empty)));
				}

				string obj = (string)constantExpression.Value;
				if (obj == null || obj.Length != 0)
				{
					return nullCompensatedExpression;
				}
				return Expression.Constant(true);
			}
			return null;
		}
	}
}

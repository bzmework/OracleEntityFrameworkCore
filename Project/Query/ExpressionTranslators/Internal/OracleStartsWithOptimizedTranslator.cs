using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// StartsWithOptimized转换器
	/// </summary>
	public class OracleStartsWithOptimizedTranslator : IMethodCallTranslator
	{
		private static readonly MethodInfo _methodInfo = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] 
		{ 
			typeof(string) 
		});

		private static readonly MethodInfo _concat = typeof(string).GetRuntimeMethod(nameof(string.Concat), new[] 
		{ 
			typeof(string), 
			typeof(string) 
		});

		/// <summary>
		/// 转换
		/// </summary>
		/// <param name="methodCallExpression">方法调用表达式</param>
		/// <returns></returns>
		public virtual Expression Translate(MethodCallExpression methodCallExpression)
		{
			if (object.Equals(methodCallExpression.Method, _methodInfo))
			{
				Expression expression = methodCallExpression.Arguments[0];
				BinaryExpression binaryExpression = Expression.AndAlso(new LikeExpression(methodCallExpression.Object, Expression.Add(methodCallExpression.Arguments[0], Expression.Constant("%", typeof(string)), _concat)), new NullCompensatedExpression(Expression.Equal(new SqlFunctionExpression("SUBSTR", methodCallExpression.Object.Type, new Expression[3]
				{
					methodCallExpression.Object,
					Expression.Constant(1),
					new SqlFunctionExpression("LENGTH", typeof(int), new Expression[1]
					{
						expression
					})
				}), expression)));

				ConstantExpression constantExpression;
				if ((constantExpression = (expression as ConstantExpression)) == null)
				{
					return Expression.OrElse(binaryExpression, Expression.Equal(expression, Expression.Constant(string.Empty)));
				}

				string obj = (string)constantExpression.Value;
				if (obj == null || obj.Length != 0)
				{
					return binaryExpression;
				}
				return Expression.Constant(true);
			}
			return null;
		}
	}
}

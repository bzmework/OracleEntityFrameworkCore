using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// 包含优化的转换器 
	/// </summary>
	public class OracleContainsOptimizedTranslator : IMethodCallTranslator
	{
		private static readonly MethodInfo _methodInfo = typeof(string).GetRuntimeMethod(nameof(string.Contains), new[] 
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
			if (object.Equals(methodCallExpression.Method, _methodInfo))
			{
				Expression expression = methodCallExpression.Arguments[0];
				ConstantExpression constantExpression;
				if ((constantExpression = (expression as ConstantExpression)) != null)
				{
					string obj = (string)constantExpression.Value;
					if (obj != null && obj.Length == 0)
					{
						return Expression.Constant(true);
					}
				}
				return Expression.GreaterThan(new SqlFunctionExpression("INSTR", typeof(int), new Expression[2]
				{
					methodCallExpression.Object,
					expression
				}), Expression.Constant(0));
			}
			return null;
		}
	}
}

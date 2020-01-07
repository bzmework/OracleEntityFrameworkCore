using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// Replace转换器
	/// </summary>
	public class OracleStringReplaceTranslator : IMethodCallTranslator
	{
		private static readonly MethodInfo _methodInfo = typeof(string).GetRuntimeMethod(nameof(string.Replace), new[] 
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
			if (!_methodInfo.Equals(methodCallExpression.Method))
			{
				return null;
			}
			return new SqlFunctionExpression("REPLACE", methodCallExpression.Type, new Expression[]
			{
				methodCallExpression.Object
			}.Concat(methodCallExpression.Arguments));
		}
	}
}

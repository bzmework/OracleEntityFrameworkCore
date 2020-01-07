using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// Replaceת����
	/// </summary>
	public class OracleStringReplaceTranslator : IMethodCallTranslator
	{
		private static readonly MethodInfo _methodInfo = typeof(string).GetRuntimeMethod(nameof(string.Replace), new[] 
		{ 
			typeof(string), 
			typeof(string) 
		});

		/// <summary>
		/// ת��
		/// </summary>
		/// <param name="methodCallExpression">�������ñ��ʽ</param>
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

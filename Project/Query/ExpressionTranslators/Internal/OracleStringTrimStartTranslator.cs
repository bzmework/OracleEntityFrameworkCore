using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// TrimStart转换器
	/// </summary>
	public class OracleStringTrimStartTranslator : IMethodCallTranslator
	{
		private static readonly MethodInfo _methodInfoWithoutArgs = typeof(string).GetRuntimeMethod(nameof(string.TrimStart), Array.Empty<Type>());
		private static readonly MethodInfo _methodInfoWithCharArrayArg = typeof(string).GetRuntimeMethod(nameof(string.TrimStart), new[] { typeof(char[]) });

		/// <summary>
		/// 转换
		/// </summary>
		/// <param name="methodCallExpression">方法调用表达式</param>
		/// <returns></returns>
		public virtual Expression Translate(MethodCallExpression methodCallExpression)
		{
			if (_methodInfoWithoutArgs?.Equals(methodCallExpression.Method) == true
				|| _methodInfoWithCharArrayArg.Equals(methodCallExpression.Method)
				// Oracle LTRIM does not take arguments
				&& ((methodCallExpression.Arguments[0] as ConstantExpression)?.Value as Array)?.Length == 0)
			{
				var sqlArguments = new Expression[] { methodCallExpression.Object };

				return new SqlFunctionExpression("LTRIM", methodCallExpression.Type, sqlArguments);
			}

			return null;
		}
	}
}

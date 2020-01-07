using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// TrimEndת����
	/// </summary>
	public class OracleStringTrimEndTranslator : IMethodCallTranslator
	{
		private static readonly MethodInfo _methodInfoWithoutArgs = typeof(string).GetRuntimeMethod(nameof(string.TrimEnd), Array.Empty<Type>());
		private static readonly MethodInfo _methodInfoWithCharArrayArg = typeof(string).GetRuntimeMethod(nameof(string.TrimEnd), new[] { typeof(char[]) });

		/// <summary>
		/// ת��
		/// </summary>
		/// <param name="methodCallExpression">�������ñ��ʽ</param>
		/// <returns></returns>
		public virtual Expression Translate(MethodCallExpression methodCallExpression)
		{
			if (_methodInfoWithoutArgs?.Equals(methodCallExpression.Method) == true
				|| _methodInfoWithCharArrayArg.Equals(methodCallExpression.Method)
				// Oracle RTRIM does not take arguments
				&& ((methodCallExpression.Arguments[0] as ConstantExpression)?.Value as Array)?.Length == 0)
			{
				var sqlArguments = new Expression[] { methodCallExpression.Object };

				return new SqlFunctionExpression("RTRIM", methodCallExpression.Type, sqlArguments);
			}

			return null;
		}
	}
}

using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// Trim转换器
	/// </summary>
	public class OracleStringTrimTranslator : IMethodCallTranslator
	{
		//private static readonly MethodInfo _methodInfoWithoutArgs = RuntimeReflectionExtensions.GetRuntimeMethod(typeof(string), "Trim", Array.Empty<Type>());
		//private static readonly MethodInfo _methodInfoWithCharArrayArg = RuntimeReflectionExtensions.GetRuntimeMethod(typeof(string), "Trim", new Type[1] { typeof(char[]) });
        
        // Method defined in netstandard2.0
        private static readonly MethodInfo _methodInfoWithoutArgs = typeof(string).GetRuntimeMethod(nameof(string.Trim), Array.Empty<Type>());
        private static readonly MethodInfo _methodInfoWithCharArrayArg = typeof(string).GetRuntimeMethod(nameof(string.Trim), new[] { typeof(char[]) });

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="methodCallExpression">方法调用表达式</param>
        /// <returns></returns>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (_methodInfoWithoutArgs.Equals(methodCallExpression.Method)
                || _methodInfoWithCharArrayArg.Equals(methodCallExpression.Method)
                // Oracle LTRIM/RTRIM does not take arguments
                && ((methodCallExpression.Arguments[0] as ConstantExpression)?.Value as Array)?.Length == 0)
            {
                var sqlArguments = new Expression[] { methodCallExpression.Object };

                return new SqlFunctionExpression(
                    "LTRIM",
                    methodCallExpression.Type,
                    new[]
                    {
                        new SqlFunctionExpression(
                            "RTRIM",
                            methodCallExpression.Type,
                            sqlArguments)
                    });
            }

            return null;
        }
	}
}

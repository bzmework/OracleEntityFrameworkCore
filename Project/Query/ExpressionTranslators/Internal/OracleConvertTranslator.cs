using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// Convert转换器 
	/// </summary>
	public class OracleConvertTranslator : IMethodCallTranslator
	{
		private static readonly Dictionary<string, string> _typeMapping = new Dictionary<string, string>
		{
            [nameof(Convert.ToByte)] = "NUMBER(3)",
            [nameof(Convert.ToDecimal)] = "NUMBER(29,4)",
            [nameof(Convert.ToDouble)] = "NUMBER",
            [nameof(Convert.ToInt16)] = "NUMBER(6)",
            [nameof(Convert.ToInt32)] = "NUMBER(10)",
            [nameof(Convert.ToInt64)] = "NUMBER(19)",
            [nameof(Convert.ToString)] = "NVARCHAR2(2000)"
		};

		private static readonly List<Type> _supportedTypes = new List<Type>
		{
			typeof(bool),
			typeof(byte),
			typeof(decimal),
			typeof(double),
			typeof(float),
			typeof(int),
			typeof(long),
			typeof(short),
			typeof(string)
		};

		private static readonly IEnumerable<MethodInfo> _supportedMethods 
			= _typeMapping.Keys
				.SelectMany(
					t => typeof(Convert).GetTypeInfo().GetDeclaredMethods(t)
                        .Where(
							m => m.GetParameters().Length == 1
                                 && _supportedTypes.Contains(m.GetParameters().First().ParameterType)));

		/// <summary>
		/// 转换
		/// </summary>
		/// <param name="methodCallExpression">方法调用表达式</param>
		/// <returns></returns>
		public virtual Expression Translate(MethodCallExpression methodCallExpression)
		{
			if (!_supportedMethods.Contains(methodCallExpression.Method))
			{
				return null;
			}
			return new SqlFunctionExpression("CAST", methodCallExpression.Type, new Expression[]
			{
				methodCallExpression.Arguments[0],
				new SqlFragmentExpression(_typeMapping[methodCallExpression.Method.Name])
			});
		}
	}
}

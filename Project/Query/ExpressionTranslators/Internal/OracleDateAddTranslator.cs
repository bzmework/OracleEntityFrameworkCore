using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// DateAdd转换器
	/// </summary>
	public class OracleDateAddTranslator : IMethodCallTranslator
	{
		private readonly Dictionary<MethodInfo, string> _methodInfoDatePartMapping = new Dictionary<MethodInfo, string>
		{
			{ typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddYears), new[] { typeof(int) }), "year" },
			{ typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMonths), new[] { typeof(int) }), "month" },
			{ typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddDays), new[] { typeof(double) }), "day" },
			{ typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddHours), new[] { typeof(double) }), "hour" },
			{ typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMinutes), new[] { typeof(double) }), "minute" },
			{ typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddSeconds), new[] { typeof(double) }), "second" },
			{ typeof(DateTime).GetRuntimeMethod(nameof(DateTime.AddMilliseconds), new[] { typeof(double) }), "millisecond" }
            //TODO: How to do TIMESTAMP WITH TIME ZONE translation?
			//{ typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddYears), new[] { typeof(int) }), "year" },
			//{ typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddMonths), new[] { typeof(int) }), "month" },
			//{ typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddDays), new[] { typeof(double) }), "day" },
			//{ typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddHours), new[] { typeof(double) }), "hour" },
			//{ typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddMinutes), new[] { typeof(double) }), "minute" },
			//{ typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddSeconds), new[] { typeof(double) }), "second" },
			//{ typeof(DateTimeOffset).GetRuntimeMethod(nameof(DateTimeOffset.AddMilliseconds), new[] { typeof(double) }), "millisecond" }
		};

		/// <summary>
		/// 转换
		/// </summary>
		/// <param name="methodCallExpression">方法调用表达式</param>
		/// <returns></returns>
		public virtual Expression Translate(MethodCallExpression methodCallExpression)
		{
			if (_methodInfoDatePartMapping.TryGetValue(methodCallExpression.Method, out var datePart))
			{
				switch (datePart)
				{
					case "month":
						return new SqlFunctionExpression(
							functionName: "ADD_MONTHS",
							returnType: methodCallExpression.Type,
							arguments: new[]
							{
								methodCallExpression.Object,
								methodCallExpression.Arguments[0]
							});
					case "year":
						return new SqlFunctionExpression(
							functionName: "ADD_MONTHS",
							returnType: methodCallExpression.Type,
							arguments: new[]
							{
								methodCallExpression.Object,
								Expression.Multiply(methodCallExpression.Arguments[0], Expression.Constant(12))
							});
				}

				// TODO: Oracle also allows '+' op where rhs are days
			}
			return null;
		}
	}
}

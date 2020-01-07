using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// Length转换器
	/// </summary>
	public class OracleStringLengthTranslator : IMemberTranslator
	{
		/// <summary>
		/// 转换
		/// </summary>
		/// <param name="memberExpression">方法调用表达式</param>
		/// <returns></returns>
		public virtual Expression Translate(MemberExpression memberExpression)
		{
			if (memberExpression.Expression == null || !(memberExpression.Expression.Type == typeof(string)) || !(memberExpression.Member.Name == "Length"))
			{
				return null;
			}
			return new ExplicitCastExpression(new SqlFunctionExpression("LENGTH", memberExpression.Type, new Expression[1]
			{
				memberExpression.Expression
			}), typeof(int));
		}
	}
}

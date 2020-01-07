using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    /// <summary>
    /// DateTime日期组件转换器
    /// </summary>
    public class OracleDateTimeDateComponentTranslator : IMemberTranslator
    {
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="memberExpression">成员表达式</param>
        /// <returns></returns>
        public virtual Expression Translate(MemberExpression memberExpression)
        { 
            return memberExpression.Expression != null
               && (memberExpression.Expression.Type == typeof(DateTime)
                   || memberExpression.Expression.Type == typeof(DateTimeOffset))
               && memberExpression.Member.Name == nameof(DateTime.Date)
                ? new SqlFunctionExpression(
                    "TRUNC",
                    memberExpression.Type,
                    new[]
                    {
                        memberExpression.Expression
                    })
                : null;
        }
    }
}

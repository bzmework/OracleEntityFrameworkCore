using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    /// <summary>
    /// DateTime�������ת����
    /// </summary>
    public class OracleDateTimeDateComponentTranslator : IMemberTranslator
    {
        /// <summary>
        /// ת��
        /// </summary>
        /// <param name="memberExpression">��Ա���ʽ</param>
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

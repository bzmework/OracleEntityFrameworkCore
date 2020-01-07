using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    /// <summary>
    /// DateTimeNow转换器
    /// </summary>
    public class OracleDateTimeNowTranslator : IMemberTranslator
    {
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="memberExpression">成员表达式</param>
        /// <returns></returns>
        public virtual Expression Translate(MemberExpression memberExpression)
        {
            if (memberExpression.Expression == null
                && memberExpression.Member.DeclaringType == typeof(DateTime))
            {
                switch (memberExpression.Member.Name)
                {
                    case nameof(DateTime.Now):
                        return new SqlFragmentExpression("SYSDATE");
                    case nameof(DateTime.UtcNow):
                        return new SqlFragmentExpression("SYSTIMESTAMP");
                }
            }
            return null;
        }
    }
}

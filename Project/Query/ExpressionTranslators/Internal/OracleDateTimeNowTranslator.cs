using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    /// <summary>
    /// DateTimeNowת����
    /// </summary>
    public class OracleDateTimeNowTranslator : IMemberTranslator
    {
        /// <summary>
        /// ת��
        /// </summary>
        /// <param name="memberExpression">��Ա���ʽ</param>
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

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    /// <summary>
    /// DateTime���ڲ������ת����
    /// </summary>
    public class OracleDateTimeDatePartComponentTranslator : IMemberTranslator
    {
        /// <summary>
        /// ת��
        /// </summary>
        /// <param name="memberExpression">��Ա���ʽ</param>
        /// <returns></returns>
        public virtual Expression Translate(MemberExpression memberExpression)
        {
            string datePart;
            if (memberExpression.Expression != null
                && (memberExpression.Expression.Type == typeof(DateTime) || memberExpression.Expression.Type == typeof(DateTimeOffset))
                && (datePart = GetDatePart(memberExpression.Member.Name)) != null)
            {
                return new SqlFunctionExpression(
                    functionName: "EXTRACT",
                    returnType: memberExpression.Type,
                    arguments: new[]
                    {
                        new SqlFragmentExpression(datePart),
                        memberExpression.Expression
                    });
            }
            return null;
        }

        private static string GetDatePart(string memberName)
        {
            switch (memberName)
            {
                case nameof(DateTime.Year):
                    return "YEAR";
                case nameof(DateTime.Month):
                    return "MONTH";
                case nameof(DateTime.Day):
                    return "DAY";
                case nameof(DateTime.Hour):
                    return "HOUR";
                case nameof(DateTime.Minute):
                    return "MINUTE";
                case nameof(DateTime.Second):
                    return "SECOND";
                default:
                    return null;
            }
        }
    }
}

using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// ToUpper×ª»»Æ÷
	/// </summary>
	public class OracleStringToUpperTranslator : ParameterlessInstanceMethodCallTranslator
	{
		/// <summary>
		/// ÊµÀý»¯
		/// </summary>
		public OracleStringToUpperTranslator()
			: base(typeof(string), "ToUpper", "UPPER")
		{
			//
		}
	}
}

using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// ToLower×ª»»Æ÷
	/// </summary>
	public class OracleStringToLowerTranslator : ParameterlessInstanceMethodCallTranslator
	{
		/// <summary>
		/// ÊµÀý»¯
		/// </summary>
		public OracleStringToLowerTranslator()
			: base(typeof(string), "ToLower", "LOWER")
		{
			//
		}
	}
}

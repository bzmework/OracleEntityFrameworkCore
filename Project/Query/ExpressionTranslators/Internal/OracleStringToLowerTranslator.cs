using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// ToLowerת����
	/// </summary>
	public class OracleStringToLowerTranslator : ParameterlessInstanceMethodCallTranslator
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		public OracleStringToLowerTranslator()
			: base(typeof(string), "ToLower", "LOWER")
		{
			//
		}
	}
}

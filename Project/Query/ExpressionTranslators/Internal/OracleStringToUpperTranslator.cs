using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// ToUpperת����
	/// </summary>
	public class OracleStringToUpperTranslator : ParameterlessInstanceMethodCallTranslator
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		public OracleStringToUpperTranslator()
			: base(typeof(string), "ToUpper", "UPPER")
		{
			//
		}
	}
}

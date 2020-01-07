using System;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// NewGuid×ª»»Æ÷
	/// </summary>
	public class OracleNewGuidTranslator : SingleOverloadStaticMethodCallTranslator
	{
		/// <summary>
		/// ÊµÀý»¯
		/// </summary>
		public OracleNewGuidTranslator()
            : base(typeof(Guid), nameof(Guid.NewGuid), "SYS_GUID")
		{
			//
		}
	}
}

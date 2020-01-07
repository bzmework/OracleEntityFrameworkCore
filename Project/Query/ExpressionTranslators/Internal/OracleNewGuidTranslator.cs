using System;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Oracle.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
	/// <summary>
	/// NewGuidת����
	/// </summary>
	public class OracleNewGuidTranslator : SingleOverloadStaticMethodCallTranslator
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		public OracleNewGuidTranslator()
            : base(typeof(Guid), nameof(Guid.NewGuid), "SYS_GUID")
		{
			//
		}
	}
}

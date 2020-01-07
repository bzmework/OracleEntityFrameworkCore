using System;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace Oracle.EntityFrameworkCore.Scaffolding.Internal
{
	/// <summary>
	/// ����������
	/// </summary>
	public class OracleCodeGenerator : ProviderCodeGenerator
	{
		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="dependencies">�����������ṩ������</param>
		public OracleCodeGenerator(ProviderCodeGeneratorDependencies dependencies)
			: base(dependencies)
		{
			//
		}

		/// <summary>
		/// ����ʹ���ṩ����
		/// </summary>
		/// <param name="connectionString">�����ַ���</param>
		/// <returns></returns>
		[Obsolete]
		public override MethodCallCodeFragment GenerateUseProvider(string connectionString)
		{
			return new MethodCallCodeFragment("UseOracle", connectionString);
		}

		/// <summary>
		/// ����ʹ���ṩ����
		/// </summary>
		/// <param name="connectionString">�����ַ���</param>
		/// <param name="providerOptions">�ṩ��ѡ��</param>
		/// <returns></returns>
		public override MethodCallCodeFragment GenerateUseProvider(string connectionString, MethodCallCodeFragment providerOptions)
		{
			return new MethodCallCodeFragment(
				"UseOracle",
				providerOptions == null
				? new object[] { connectionString }
				: new object[] { connectionString, new NestedClosureCodeFragment("x", providerOptions) });
		}
	}
}

using System;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;

namespace Oracle.EntityFrameworkCore.Scaffolding.Internal
{
	/// <summary>
	/// 代码生成器
	/// </summary>
	public class OracleCodeGenerator : ProviderCodeGenerator
	{
		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">代码生成器提供器依赖</param>
		public OracleCodeGenerator(ProviderCodeGeneratorDependencies dependencies)
			: base(dependencies)
		{
			//
		}

		/// <summary>
		/// 生成使用提供程序
		/// </summary>
		/// <param name="connectionString">连接字符串</param>
		/// <returns></returns>
		[Obsolete]
		public override MethodCallCodeFragment GenerateUseProvider(string connectionString)
		{
			return new MethodCallCodeFragment("UseOracle", connectionString);
		}

		/// <summary>
		/// 生成使用提供程序
		/// </summary>
		/// <param name="connectionString">连接字符串</param>
		/// <param name="providerOptions">提供器选项</param>
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

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Oracle.EntityFrameworkCore.Query.Internal
{
	/// <summary>
	/// ��ѯ����������
	/// </summary>
	public class OracleQueryCompilationContext : RelationalQueryCompilationContext
	{
		internal string _oracleSQLCompatibility = "12";

		/// <summary>
		/// �Ƿ�֧�ֺ�������
		/// </summary>
		public override bool IsLateralJoinSupported
		{
			get
			{
				if (_oracleSQLCompatibility == "11")
				{
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// �����������
		/// </summary>
		public override int MaxTableAliasLength
		{
			get { return 30; }
		}

		/// <summary>
		/// ʵ������ѯ����������
		/// </summary>
		/// <param name="dependencies">��ѯ��������������</param>
		/// <param name="linqOperatorProvider">Linq�������ṩ��</param>
		/// <param name="queryMethodProvider">��ѯ�����ṩ��</param>
		/// <param name="trackQueryResults">���ٲ�ѯ���</param>
		/// <param name="oracleSQLCompatibility">����SQL��"11"��ʾ11.2G����ǰ�����ݿ�, "12"��ʾ12c���Ժ�����ݿ�</param>
		public OracleQueryCompilationContext(
			[NotNull] QueryCompilationContextDependencies dependencies, 
			[NotNull] ILinqOperatorProvider linqOperatorProvider,
			[NotNull] IQueryMethodProvider queryMethodProvider, 
			bool trackQueryResults, 
			string oracleSQLCompatibility)
			: base(dependencies, linqOperatorProvider, queryMethodProvider, trackQueryResults)
		{
			_oracleSQLCompatibility = oracleSQLCompatibility;
		}
	}
}

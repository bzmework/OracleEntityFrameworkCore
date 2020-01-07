using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// ���ӽӿ�
	/// </summary>
	public interface IOracleConnection : IRelationalConnection, IRelationalTransactionManager, IDbContextTransactionManager, IResettableService, IDisposable
	{
		/// <summary>
		/// ����������
		/// </summary>
		/// <returns></returns>
		IOracleConnection CreateMasterConnection();
	}
}

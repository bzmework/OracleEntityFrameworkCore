using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// 连接接口
	/// </summary>
	public interface IOracleConnection : IRelationalConnection, IRelationalTransactionManager, IDbContextTransactionManager, IResettableService, IDisposable
	{
		/// <summary>
		/// 创建主连接
		/// </summary>
		/// <returns></returns>
		IOracleConnection CreateMasterConnection();
	}
}

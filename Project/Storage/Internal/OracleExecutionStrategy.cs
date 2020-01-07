using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Oracle.EntityFrameworkCore.Internal;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// ִ�в���
	/// </summary>
	public class OracleExecutionStrategy : IExecutionStrategy
	{
		private ExecutionStrategyDependencies Dependencies
		{
			get;
		}

		/// <summary>
		/// ʧ��ʱ����
		/// </summary>
		public virtual bool RetriesOnFailure
		{
			get { return false; }
		}

		/// <summary>
		/// ʵ����ִ�в���
		/// </summary>
		/// <param name="dependencies">ִ�в�������</param>
		public OracleExecutionStrategy([NotNull] ExecutionStrategyDependencies dependencies)
		{
			Dependencies = dependencies;
		}

		/// <summary>
		/// ִ�в���
		/// </summary>
		/// <typeparam name="TState"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="state">״̬</param>
		/// <param name="operation">����</param>
		/// <param name="verifySucceeded">��֤�ɹ�</param>
		/// <returns></returns>
		public virtual TResult Execute<TState, TResult>(
			TState state, 
			Func<DbContext, TState, TResult> operation, 
			Func<DbContext, TState, ExecutionResult<TResult>> verifySucceeded)
		{
			try
			{
				return operation(Dependencies.CurrentDbContext.Context, state);
			}
			catch (Exception ex)
			{
				if (ExecutionStrategy.CallOnWrappedException(ex, OracleTransientExceptionDetector.ShouldRetryOn))
				{
					throw new InvalidOperationException(OracleStrings.TransientExceptionDetected, ex);
				}
				throw;
			}
		}

		/// <summary>
		/// �첽ִ��
		/// </summary>
		/// <typeparam name="TState"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="state">״̬</param>
		/// <param name="operation">����</param>
		/// <param name="verifySucceeded">��֤�ɹ�</param>
		/// <param name="cancellationToken">ȡ������</param>
		/// <returns></returns>
		public virtual async Task<TResult> ExecuteAsync<TState, TResult>(
			TState state, 
			Func<DbContext, TState, CancellationToken, Task<TResult>> operation, 
			Func<DbContext, TState, CancellationToken, Task<ExecutionResult<TResult>>> verifySucceeded, 
			CancellationToken cancellationToken)
		{
			try
			{
				return await operation(Dependencies.CurrentDbContext.Context, state, cancellationToken);
			}
			catch (Exception ex)
			{
				if (ExecutionStrategy.CallOnWrappedException(ex, OracleTransientExceptionDetector.ShouldRetryOn))
				{
					throw new InvalidOperationException(OracleStrings.TransientExceptionDetected, ex);
				}
				throw;
			}
		}
	}
}

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
	/// 执行策略
	/// </summary>
	public class OracleExecutionStrategy : IExecutionStrategy
	{
		private ExecutionStrategyDependencies Dependencies
		{
			get;
		}

		/// <summary>
		/// 失败时重试
		/// </summary>
		public virtual bool RetriesOnFailure
		{
			get { return false; }
		}

		/// <summary>
		/// 实例化执行策略
		/// </summary>
		/// <param name="dependencies">执行策略依赖</param>
		public OracleExecutionStrategy([NotNull] ExecutionStrategyDependencies dependencies)
		{
			Dependencies = dependencies;
		}

		/// <summary>
		/// 执行操作
		/// </summary>
		/// <typeparam name="TState"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="state">状态</param>
		/// <param name="operation">操作</param>
		/// <param name="verifySucceeded">验证成功</param>
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
		/// 异步执行
		/// </summary>
		/// <typeparam name="TState"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="state">状态</param>
		/// <param name="operation">操作</param>
		/// <param name="verifySucceeded">验证成功</param>
		/// <param name="cancellationToken">取消令牌</param>
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

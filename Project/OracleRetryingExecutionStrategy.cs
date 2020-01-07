using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Oracle.EntityFrameworkCore.Storage.Internal;
using Oracle.ManagedDataAccess.Client;

namespace Oracle.EntityFrameworkCore
{
	/// <summary>
	/// 重试执行策略 
	/// </summary>
	public class OracleRetryingExecutionStrategy : ExecutionStrategy
	{
		private readonly ICollection<int> _additionalErrorNumbers;

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="context"></param>
		public OracleRetryingExecutionStrategy([NotNull] DbContext context)
			: this(context, ExecutionStrategy.DefaultMaxRetryCount)
		{
			//
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">执行策略依赖</param>
		public OracleRetryingExecutionStrategy([NotNull] ExecutionStrategyDependencies dependencies)
			: this(dependencies, ExecutionStrategy.DefaultMaxRetryCount)
		{
			//
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="context">数据库上下文</param>
		/// <param name="maxRetryCount">最大重试次数</param>
		public OracleRetryingExecutionStrategy([NotNull] DbContext context, int maxRetryCount)
			: this(context, maxRetryCount, ExecutionStrategy.DefaultMaxDelay, null)
		{
			//
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">执行策略依赖</param>
		/// <param name="maxRetryCount">最大重试次数</param>
		public OracleRetryingExecutionStrategy([NotNull] ExecutionStrategyDependencies dependencies, int maxRetryCount)
			: this(dependencies, maxRetryCount, ExecutionStrategy.DefaultMaxDelay, null)
		{
			//
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="context">数据库上下文</param>
		/// <param name="maxRetryCount">最大重试次数</param>
		/// <param name="maxRetryDelay">最大重试延时</param>
		/// <param name="errorNumbersToAdd">错误数量</param>
		public OracleRetryingExecutionStrategy([NotNull] DbContext context, int maxRetryCount, TimeSpan maxRetryDelay, [CanBeNull] ICollection<int> errorNumbersToAdd)
			: base(context, maxRetryCount, maxRetryDelay)
		{
			_additionalErrorNumbers = errorNumbersToAdd;
		}

		/// <summary>
		/// 实例化
		/// </summary>
		/// <param name="dependencies">执行策略依赖</param>
		/// <param name="maxRetryCount">最大重试次数</param>
		/// <param name="maxRetryDelay">最大重试延时</param>
        /// <param name="errorNumbersToAdd"> 应被视为暂时性的其他SQL错误号 </param>
		public OracleRetryingExecutionStrategy(
			[NotNull] ExecutionStrategyDependencies dependencies, 
			int maxRetryCount, 
			TimeSpan maxRetryDelay, 
			[CanBeNull] ICollection<int> errorNumbersToAdd)
			: base(dependencies, maxRetryCount, maxRetryDelay)
		{
			_additionalErrorNumbers = errorNumbersToAdd;
		}

		/// <summary>
		/// 应再重试
		/// </summary>
		/// <param name="exception">异常</param>
		/// <returns></returns>
		protected override bool ShouldRetryOn(Exception exception)
		{
			OracleException val;
			if (_additionalErrorNumbers != null && (val = (exception as OracleException)) != null)
			{
				foreach (OracleError item in (ArrayList)(object)val.Errors)
				{
					OracleError err = (OracleError)(object)item;
					if (_additionalErrorNumbers.Contains(err.Number))
					{
						return true;
					}
				}
			}
			return OracleTransientExceptionDetector.ShouldRetryOn(exception);
		}
	}
}

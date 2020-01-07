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
	/// ����ִ�в��� 
	/// </summary>
	public class OracleRetryingExecutionStrategy : ExecutionStrategy
	{
		private readonly ICollection<int> _additionalErrorNumbers;

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="context"></param>
		public OracleRetryingExecutionStrategy([NotNull] DbContext context)
			: this(context, ExecutionStrategy.DefaultMaxRetryCount)
		{
			//
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="dependencies">ִ�в�������</param>
		public OracleRetryingExecutionStrategy([NotNull] ExecutionStrategyDependencies dependencies)
			: this(dependencies, ExecutionStrategy.DefaultMaxRetryCount)
		{
			//
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="context">���ݿ�������</param>
		/// <param name="maxRetryCount">������Դ���</param>
		public OracleRetryingExecutionStrategy([NotNull] DbContext context, int maxRetryCount)
			: this(context, maxRetryCount, ExecutionStrategy.DefaultMaxDelay, null)
		{
			//
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="dependencies">ִ�в�������</param>
		/// <param name="maxRetryCount">������Դ���</param>
		public OracleRetryingExecutionStrategy([NotNull] ExecutionStrategyDependencies dependencies, int maxRetryCount)
			: this(dependencies, maxRetryCount, ExecutionStrategy.DefaultMaxDelay, null)
		{
			//
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="context">���ݿ�������</param>
		/// <param name="maxRetryCount">������Դ���</param>
		/// <param name="maxRetryDelay">���������ʱ</param>
		/// <param name="errorNumbersToAdd">��������</param>
		public OracleRetryingExecutionStrategy([NotNull] DbContext context, int maxRetryCount, TimeSpan maxRetryDelay, [CanBeNull] ICollection<int> errorNumbersToAdd)
			: base(context, maxRetryCount, maxRetryDelay)
		{
			_additionalErrorNumbers = errorNumbersToAdd;
		}

		/// <summary>
		/// ʵ����
		/// </summary>
		/// <param name="dependencies">ִ�в�������</param>
		/// <param name="maxRetryCount">������Դ���</param>
		/// <param name="maxRetryDelay">���������ʱ</param>
        /// <param name="errorNumbersToAdd"> Ӧ����Ϊ��ʱ�Ե�����SQL����� </param>
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
		/// Ӧ������
		/// </summary>
		/// <param name="exception">�쳣</param>
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

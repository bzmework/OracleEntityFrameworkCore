using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Query.Sql.Internal
{
	/// <summary>
	/// ��ѯSQL������
	/// </summary>
	public class OracleQuerySqlGenerator : DefaultQuerySqlGenerator
	{
		internal string _oracleSQLCompatibility = "12";

		private int Count;

		private bool firstwhereclauseappended;

		private bool is112SqlCompatibility;

		private bool outerSelectRequired;

		private int generateProjectionCallCount;

		private int generateProjectionCallCountCounter;

		private IDiagnosticsLogger<DbLoggerCategory.Query> m_oracleLogger;

		private static readonly HashSet<string> _builtInFunctions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"MAX",
			"MIN",
			"SUM",
			"SUBSTR",
			"INSTR",
			"LENGTH",
			"COUNT"
		};

		/// <summary>
		/// True������
		/// </summary>
		protected override string TypedTrueLiteral
		{
			get { return "1"; }
		}

		/// <summary>
		/// False������
		/// </summary>
		protected override string TypedFalseLiteral
		{
			get { return "0"; }
		}

		/// <summary>
		/// �����ָ���
		/// </summary>
		protected override string AliasSeparator
		{
			get { return " "; }
		}

		/// <summary>
		/// Oracle��ѯSQL������
		/// </summary>
		/// <param name="dependencies">��ѯSQL����������</param>
		/// <param name="selectExpression">Select���ʽ</param>
		/// <param name="oracleSQLCompatibility">����SQL��"11"��ʾ11.2G����ǰ�����ݿ�, "12"��ʾ12c���Ժ�����ݿ�</param>
		/// <param name="logger"></param>
		public OracleQuerySqlGenerator(
			[NotNull] QuerySqlGeneratorDependencies dependencies, 
			[NotNull] SelectExpression selectExpression, 
			string oracleSQLCompatibility, 
			IDiagnosticsLogger<DbLoggerCategory.Query> logger = null)
			: base(dependencies, selectExpression)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Query>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.ctor);
			}

			if (!string.IsNullOrEmpty(oracleSQLCompatibility))
			{
				_oracleSQLCompatibility = oracleSQLCompatibility;
			}

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Query>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.ctor, "OracleSQLCompatibility: " + _oracleSQLCompatibility);
			}

			if (_oracleSQLCompatibility.StartsWith("11"))
			{
				is112SqlCompatibility = true;
			}
			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Query>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// ��д���ɲ�����
		/// </summary>
		/// <param name="expression">���ʽ</param>
		/// <returns></returns>
		protected override string GenerateOperator(Expression expression)
		{
			if (expression.NodeType != 0 || !(expression.Type == typeof(string)))
			{
				return base.GenerateOperator(expression);
			}
			return " || ";
		}

		/// <summary>
		/// ��д�����Ʒ���
		/// </summary>
		/// <param name="binaryExpression">Ҫ���ʵĶ����Ʊ��ʽ</param>
		/// <returns>���ʽ</returns>
		protected override Expression VisitBinary(BinaryExpression binaryExpression)
		{
			Check.NotNull(binaryExpression, nameof(binaryExpression));
			if (is112SqlCompatibility)
			{
				Sql.Append("(");
			}

			switch (binaryExpression.NodeType)
			{
			case ExpressionType.And:
				Sql.Append("BITAND(");
				Visit(binaryExpression.Left);
				Sql.Append(", ");
				Visit(binaryExpression.Right);
				Sql.Append(")");
				if (is112SqlCompatibility)
				{
					Sql.Append(")");
				}
				return binaryExpression;
			case ExpressionType.Or:
				Visit(binaryExpression.Left);
				Sql.Append(" - BITAND(");
				Visit(binaryExpression.Left);
				Sql.Append(", ");
				Visit(binaryExpression.Right);
				Sql.Append(") + ");
				Visit(binaryExpression.Right);
				if (is112SqlCompatibility)
				{
					Sql.Append(")");
				}
				return binaryExpression;
			case ExpressionType.Modulo:
				Sql.Append("MOD(");
				Visit(binaryExpression.Left);
				Sql.Append(", ");
				Visit(binaryExpression.Right);
				Sql.Append(")");
				if (is112SqlCompatibility)
				{
					Sql.Append(")");
				}
				return binaryExpression;
			default:
			{
				if (binaryExpression.Right is ConstantExpression && (binaryExpression.Right as ConstantExpression).Value is string && string.IsNullOrEmpty((binaryExpression.Right as ConstantExpression).Value as string) && (binaryExpression.NodeType == ExpressionType.Equal || binaryExpression.NodeType == ExpressionType.NotEqual))
				{
					Visit(binaryExpression.Left);
					if (binaryExpression.NodeType == ExpressionType.Equal)
					{
						Sql.Append(" IS NULL ");
					}
					else if (binaryExpression.NodeType == ExpressionType.NotEqual)
					{
						Sql.Append(" IS NOT NULL ");
					}
					if (is112SqlCompatibility)
					{
						Sql.Append(")");
					}
					return binaryExpression;
				}
				Expression result = base.VisitBinary(binaryExpression);
				if (is112SqlCompatibility)
				{
					Sql.Append(")");
				}
				return result;
			}
			}
		}

		/// <summary>
		/// ��дOrderBy
		/// </summary>
		/// <param name="orderings">�����б�</param>
		protected override void GenerateOrderBy(IReadOnlyList<Ordering> orderings)
		{
			orderings = orderings.Where((Ordering o) => o.Expression.NodeType != ExpressionType.Constant && o.Expression.NodeType != ExpressionType.Parameter).ToList();
			if (orderings.Count > 0)
			{
				base.GenerateOrderBy(orderings);
			}
		}

		/// <summary>
		/// ��дOrdering
		/// </summary>
		/// <param name="ordering">����</param>
		protected override void GenerateOrdering(Ordering ordering)
		{
			Check.NotNull(ordering, nameof(ordering));
			Expression expression = ordering.Expression;
			if (expression.NodeType != ExpressionType.Constant && expression.NodeType != ExpressionType.Parameter)
			{
				base.GenerateOrdering(ordering);
				if (ordering.OrderingDirection == OrderingDirection.Asc)
				{
					Sql.Append(" NULLS FIRST");
				}
			}
		}

		/// <summary>
		/// ��дTop
		/// </summary>
		/// <param name="selectExpression"></param>
		protected override void GenerateTop(SelectExpression selectExpression)
		{
			// Not supported
		}

		/// <summary>
		/// ��дLimitOffset��ҳ(12c֧�ֵ�ԭ����ҳ)
		/// </summary>
		/// <param name="selectExpression"></param>
		protected override void GenerateLimitOffset(SelectExpression selectExpression)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.GenerateLimitOffset);
				}

				Check.NotNull(selectExpression, nameof(selectExpression));
				if (selectExpression.Limit != null && selectExpression.Offset == null)
				{
					Sql.AppendLine().Append("FETCH FIRST ");
					Visit(selectExpression.Limit);
					Sql.Append(" ROWS ONLY");
				}
				else
				{
					base.GenerateLimitOffset(selectExpression);
				}
				if (selectExpression.Limit != null)
				{
					if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
					{
						Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.SQL, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.GenerateLimitOffset, $"Limit:{selectExpression.Limit.ToString()}");
					}
				}
				else if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.SQL, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.GenerateLimitOffset, "Limit:null");
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.GenerateLimitOffset, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.GenerateLimitOffset);
				}
			}
		}

		/// <summary>
		/// ��дFromSql 
		/// </summary>
		/// <param name="fromSqlExpression"></param>
		/// <returns></returns>
		public override Expression VisitFromSql(FromSqlExpression fromSqlExpression)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitFromSql);
				}

				Check.NotNull(fromSqlExpression, nameof(fromSqlExpression));
				Sql.AppendLine("(");
				using (Sql.Indent())
				{
					base.GenerateFromSql(fromSqlExpression.Sql, fromSqlExpression.Arguments, ParameterValues);
				}
				Sql.Append(") ").Append(SqlGenerator.DelimitIdentifier(fromSqlExpression.Alias));
				return fromSqlExpression;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitFromSql, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitFromSql);
				}
			}
		}

		/// <summary>
		/// ����αFrom�Ӿ� 
		/// </summary>
		protected override void GeneratePseudoFromClause()
		{
			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.GeneratePseudoFromClause);
			}

			Sql.Append(" FROM DUAL");

			if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			{
				Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.GeneratePseudoFromClause);
			}
		}

		/// <summary>
		/// ��д���򽻲�����
		/// </summary>
		/// <param name="crossJoinLateralExpression">������ʽ</param>
		/// <returns></returns>
		public override Expression VisitCrossJoinLateral(CrossJoinLateralExpression crossJoinLateralExpression)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitCrossJoinLateral);
				}

				Check.NotNull(crossJoinLateralExpression, nameof(crossJoinLateralExpression));
				Sql.Append("CROSS APPLY ");
				Visit(crossJoinLateralExpression.TableExpression);
				return crossJoinLateralExpression;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitCrossJoinLateral, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitCrossJoinLateral);
				}
			}
		}

		/// <summary>
		/// ��дSql����
		/// </summary>
		/// <param name="sqlFunctionExpression">Sql�������ʽ</param>
		/// <returns></returns>
		public override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitSqlFunction);
				}

				switch (sqlFunctionExpression.FunctionName)
				{
				case "EXTRACT":
				{
					Sql.Append(sqlFunctionExpression.FunctionName);
					Sql.Append("(");
					Visit(sqlFunctionExpression.Arguments[0]);
					Sql.Append(" FROM ");
					Visit(sqlFunctionExpression.Arguments[1]);
					Sql.Append(")");
					return sqlFunctionExpression;
				}
				case "CAST":
				{
					Sql.Append(sqlFunctionExpression.FunctionName);
					Sql.Append("(");
					Visit(sqlFunctionExpression.Arguments[0]);
					Sql.Append(" AS ");
					Visit(sqlFunctionExpression.Arguments[1]);
					Sql.Append(")");
					return sqlFunctionExpression;
				}
				case "AVG":
				{
					if (!(sqlFunctionExpression.Type == typeof(decimal)))
					{
						break;
					}
					Sql.Append("CAST(");
					base.VisitSqlFunction(sqlFunctionExpression);
					Sql.Append(" AS NUMBER(29,4))");
					return sqlFunctionExpression;
				}
				case "SUM":
				{
					if (!(sqlFunctionExpression.Type == typeof(decimal)))
					{
						break;
					}
					Sql.Append("CAST(");
					base.VisitSqlFunction(sqlFunctionExpression);
					Sql.Append(" AS NUMBER(29,4))");
					return sqlFunctionExpression;
				}
				case "INSTR":
				{
					ParameterExpression parameterExpression;
					if ((parameterExpression = (sqlFunctionExpression.Arguments[1] as ParameterExpression)) != null && ParameterValues.TryGetValue(parameterExpression.Name, out object value))
					{
						string obj = (string)value;
						if (obj != null && obj.Length == 0)
						{
							return Visit(Expression.Constant(1));
						}
					}
					break;
				}
				case "ADD_MONTHS":
					{
						Sql.Append("CAST(");
						base.VisitSqlFunction(sqlFunctionExpression);
						Sql.Append(" AS TIMESTAMP)");
						return sqlFunctionExpression;
					}
				}

				return base.VisitSqlFunction((!_builtInFunctions.Contains(sqlFunctionExpression.FunctionName) && sqlFunctionExpression.Instance == null) ? new SqlFunctionExpression(SqlGenerator.DelimitIdentifier(sqlFunctionExpression.FunctionName), sqlFunctionExpression.Type, null, sqlFunctionExpression.Arguments) : sqlFunctionExpression);
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitSqlFunction, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitSqlFunction);
				}
			}
		}

		/// <summary>
		/// ��д����ͶӰ���Ա��ʽ���´���
		/// </summary>
		/// <param name="projection">ͶӰ���ʽ</param>
		protected override void GenerateProjection(Expression projection)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.GenerateProjection);
				}

				AliasExpression aliasExpression = projection as AliasExpression;
				Expression expressionProcess = aliasExpression?.Expression ?? projection;
				Expression updatedExperssion = ExplicitCastToBool(expressionProcess);

				expressionProcess = aliasExpression != null
					? new AliasExpression(aliasExpression.Alias, updatedExperssion)
					: updatedExperssion;

				// �˴������OracleSqlGenerationHelper�з�����д���ʽ������Ϊ����ֶμ������ŵ�
				base.GenerateProjection(expressionProcess);

				if (is112SqlCompatibility && outerSelectRequired)
				{
					if (!(projection is AliasExpression))
					{
						Sql.Append(" K" + generateProjectionCallCountCounter);
					}
					generateProjectionCallCountCounter++;
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.GenerateProjection, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.GenerateProjection);
				}
			}
		}

		/// <summary>
		/// ��дSelect
		/// </summary>
		/// <param name="selectExpression">Select���ʽ</param>
		/// <returns></returns>
		public override Expression VisitSelect(SelectExpression selectExpression)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitSelect);
				}

				if (is112SqlCompatibility)
				{
					VisitSelectForDB112OrLess(selectExpression);
				}
				else
				{
					VisitSelectForDB121OrMore(selectExpression);
				}
				return selectExpression;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitSelect, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitSelect);
				}
			}
		}

		/// <summary>
		/// Select(���ڻ����11.2G)
		/// </summary>
		/// <param name="selectExpression">Select���ʽ</param>
		/// <returns></returns>
		public Expression VisitSelectForDB112OrLess(SelectExpression selectExpression)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitSelectForDB112OrLess);
				}

				Check.NotNull(selectExpression, nameof(selectExpression));
				IDisposable disposable = null;
				int num = 0;
				int aliasCount = 0;
				bool flag = false;

				if (selectExpression.Alias != null)
				{
					Sql.AppendLine("(");
					disposable = Sql.Indent();
				}

				// OrderBy����
				if ((selectExpression.OrderBy.Count > 0 && selectExpression.Limit != null) || selectExpression.Offset != null)
				{
					outerSelectRequired = true;
					string alias = null;
					new List<string>();
					Count++;
					aliasCount = Count;
					if (selectExpression != null && selectExpression.ProjectStarTable != null)
					{
						alias = selectExpression.ProjectStarTable.Alias;
						selectExpression.ProjectStarTable.Alias = "m" + aliasCount;
					}
					Sql.AppendLine("Select ");
					if (selectExpression.IsDistinct)
					{
						Sql.Append("DISTINCT ");
					}
					GenerateTop(selectExpression);
					
					flag = false;
					if (selectExpression.IsProjectStar)
					{
						Sql.Append(SqlGenerator.DelimitIdentifier(selectExpression.ProjectStarTable.Alias)).Append(".*");
						flag = true;
					}
					if (selectExpression.Projection.Count > 0)
					{
						if (selectExpression.IsProjectStar)
						{
							Sql.Append(", ");
						}
						generateProjectionCallCount = selectExpression.Projection.Count;
						for (int i = 0; i < generateProjectionCallCount - 1; i++)
						{
							if (selectExpression.Projection[i] is ColumnExpression)
							{
								Sql.Append(" K" + i);
								Sql.Append(" \"" + ((ColumnExpression)selectExpression.Projection[i]).Name + "\"");
								Sql.Append(",");
							}
							else if (selectExpression.Projection[i] is ColumnReferenceExpression)
							{
								Sql.Append(" K" + i);
								Sql.Append(" \"" + ((ColumnReferenceExpression)selectExpression.Projection[i]).Name + "\"");
								Sql.Append(",");
							}
							else if (selectExpression.Projection[i] is AliasExpression)
							{
								Sql.Append(" \"" + ((AliasExpression)selectExpression.Projection[i]).Alias + "\"");
								Sql.Append(",");
							}
							else
							{
								Sql.Append(" K" + i);
								Sql.Append(",");
							}
						}
						if (selectExpression.Projection[generateProjectionCallCount - 1] is ColumnExpression)
						{
							Sql.Append(" K" + (generateProjectionCallCount - 1));
							Sql.Append(" \"" + ((ColumnExpression)selectExpression.Projection[generateProjectionCallCount - 1]).Name + "\"");
						}
						else if (selectExpression.Projection[generateProjectionCallCount - 1] is ColumnReferenceExpression)
						{
							Sql.Append(" K" + (generateProjectionCallCount - 1));
							Sql.Append(" \"" + ((ColumnReferenceExpression)selectExpression.Projection[generateProjectionCallCount - 1]).Name + "\"");
						}
						else if (selectExpression.Projection[generateProjectionCallCount - 1] is AliasExpression)
						{
							Sql.Append(" \"" + ((AliasExpression)selectExpression.Projection[generateProjectionCallCount - 1]).Alias + "\"");
						}
						else
						{
							Sql.Append(" K" + (generateProjectionCallCount - 1));
						}
						flag = true;
					}
					if (!flag)
					{
						Sql.Append("1");
					}
					Sql.Append(" from");
					Sql.AppendLine("(");
					if (selectExpression != null && selectExpression.ProjectStarTable != null)
					{
						selectExpression.ProjectStarTable.Alias = alias;
					}
				}

				// ��ҳ����
				if (selectExpression.Offset != null)
				{
					Count++;
					num = Count;
					Sql.AppendLine("select \"m" + num + "\".*, rownum r" + num + " from");
					Sql.AppendLine("(");
				}
				Sql.Append("SELECT ");

				// Distinct����
				if (selectExpression.IsDistinct)
				{
					Sql.Append("DISTINCT ");
				}
				GenerateTop(selectExpression);

				flag = false;
				if (selectExpression.IsProjectStar)
				{
					Sql.Append(SqlGenerator.DelimitIdentifier(selectExpression.ProjectStarTable.Alias)).Append(".*");
					flag = true;
				}
				if (selectExpression.Projection.Count > 0)
				{
					if (selectExpression.IsProjectStar)
					{
						Sql.Append(", ");
					}
					base.GenerateList(selectExpression.Projection, GenerateProjection);
					flag = true;
				}
				if (!flag)
				{
					Sql.Append("1");
				}

				outerSelectRequired = false;
				generateProjectionCallCount = 0;
				generateProjectionCallCountCounter = 0;
				if (selectExpression.Tables.Count > 0)
				{
					Sql.AppendLine().Append("FROM ");
					base.GenerateList(selectExpression.Tables, delegate(IRelationalCommandBuilder sql)
					{
						sql.AppendLine();
					});
				}
				else
				{
					GeneratePseudoFromClause();
				}

				if (selectExpression.Predicate != null)
				{
					GeneratePredicate(selectExpression.Predicate);
					firstwhereclauseappended = true;
				}

				if (selectExpression.OrderBy.Count == 0 && selectExpression.Offset == null && selectExpression.Limit != null)
				{
					if (firstwhereclauseappended)
					{
						Sql.AppendLine().Append("and rownum <= ");
						Visit(selectExpression.Limit);
					}
					else
					{
						Sql.AppendLine().Append("where rownum <= ");
						Visit(selectExpression.Limit);
					}
				}
				firstwhereclauseappended = false;

				if (selectExpression.GroupBy.Count > 0)
				{
					Sql.AppendLine();
					Sql.Append("GROUP BY ");
					base.GenerateList(selectExpression.GroupBy);
				}
				if (selectExpression.Having != null)
				{
					base.GenerateHaving(selectExpression.Having);
				}
				if (selectExpression.OrderBy.Count > 0)
				{
					Sql.AppendLine();
					this.GenerateOrderBy(selectExpression.OrderBy);
				}
				if (selectExpression.Offset != null)
				{
					Sql.AppendLine().Append(") \"m" + num + "\"");
				}
				if ((selectExpression.OrderBy.Count > 0 && selectExpression.Limit != null) || selectExpression.Offset != null)
				{
					Sql.AppendLine().Append(") \"m" + aliasCount + "\"");
					if (selectExpression.Limit != null && selectExpression.Offset == null)
					{
						Sql.AppendLine().Append("where rownum <= ");
						Visit(selectExpression.Limit);
					}
					if (selectExpression.Limit == null && selectExpression.Offset != null)
					{
						Sql.AppendLine().Append("where r" + num + " > ");
						Visit(selectExpression.Offset);
					}
					if (selectExpression.Limit != null && selectExpression.Offset != null)
					{
						Sql.AppendLine().Append("where r" + num + " > ");
						Visit(selectExpression.Offset);
						Sql.AppendLine().Append("and r" + num + " <= (");
						Visit(selectExpression.Offset);
						Sql.Append(" + ");
						Visit(selectExpression.Limit);
						Sql.Append(")");
					}
				}
				if (disposable != null)
				{
					disposable.Dispose();
					Sql.AppendLine().Append(")");
					if (selectExpression.Alias.Length > 0)
					{
						Sql.Append(AliasSeparator).Append(SqlGenerator.DelimitIdentifier(selectExpression.Alias));
					}
				}

				return selectExpression;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitSelectForDB112OrLess, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitSelectForDB112OrLess);
				}
			}
		}

		/// <summary>
		/// Select(���ڻ����12.1)
		/// </summary>
		/// <param name="selectExpression">Select���ʽ</param>
		/// <returns></returns>
		public Expression VisitSelectForDB121OrMore(SelectExpression selectExpression)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitSelectForDB121OrMore);
				}

				base.VisitSelect(selectExpression);
				return selectExpression;
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitSelectForDB121OrMore, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Query>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleQuerySqlGenerator, OracleTraceFuncName.VisitSelectForDB121OrMore);
				}
			}
		}

		private static Expression ExplicitCastToBool(Expression expression)
		{
			BinaryExpression obj = expression as BinaryExpression;
			if (obj == null || obj.NodeType != ExpressionType.Coalesce || !(expression.Type.UnwrapNullableType() == typeof(bool)))
			{
				return expression;
			}
			return new ExplicitCastExpression(expression, expression.Type);
		}
	}
}

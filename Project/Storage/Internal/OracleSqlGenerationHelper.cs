using System;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Migrations;
using Oracle.EntityFrameworkCore.Utilities;

namespace Oracle.EntityFrameworkCore.Storage.Internal
{
	/// <summary>
	/// Sql���ɰ�������
	/// �ṩ����������SQL����ķ���
	/// ������ͨ�������ݿ��ṩ���򣨺�������չ����ʹ�á�
	/// ��ͨ��������Ӧ�ó�������С�
	/// </summary>
	public class OracleSqlGenerationHelper : RelationalSqlGenerationHelper
	{
		private IDiagnosticsLogger<DbLoggerCategory.Database.Command> m_oracleLogger;

		/// <summary>
		/// ʵ����Sql���ɰ�����
		/// </summary>
		/// <param name="dependencies">Sql���ɰ���������</param>
		/// <param name="logger">��־</param>
		public OracleSqlGenerationHelper(
			[NotNull] RelationalSqlGenerationHelperDependencies dependencies,
			IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger)
			: base(dependencies)
		{
			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.ctor);
			}

			m_oracleLogger = logger;

			if (Check.IsTraceEnabled(logger?.Logger))
			{
				Trace<DbLoggerCategory.Database.Command>.Write(logger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.ctor);
			}
		}

		/// <summary>
		/// ����������SQL������ֹ�� 
		/// </summary>
		public override string BatchTerminator
		{
			get { return "/" + Environment.NewLine + Environment.NewLine; }
		}

		/// <summary>
		/// Ϊ�����ĺ�ѡ����������Ч�Ĳ������� 
		/// </summary>
		/// <param name="name">�����ĺ�ѡ����</param>
		/// <returns> ���ں�ѡ���Ƶ���Ч����</returns>
		public override string GenerateParameterName(string name)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.GenerateParameterName);
				}

				while (name.StartsWith("_", StringComparison.Ordinal) || Regex.IsMatch(name, "^\\d"))
				{
					name = name.Substring(1);
				}
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.GenerateParameterName, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.GenerateParameterName);
				}
			}
			if (Encoding.UTF8.GetByteCount(name) > 30)
			{
				name = OracleMigrationsSqlGenerator.DeriveObjectName(null, name);
			}
			return ":" + name;
		}

		/// <summary>
		/// Ϊ�����ĺ�ѡ����д����Ч�Ĳ�����
		/// </summary>
		/// <param name="builder">Ҫ�����ɵ��ַ���д����ַ���������</param>
		/// <param name="name">�����ĺ�ѡ����</param>
		public override void GenerateParameterName(StringBuilder builder, string name)
		{
			try
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.GenerateParameterName);
				}

				builder.Append(GenerateParameterName(name));
			}
			catch (Exception ex)
			{
				if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.GenerateParameterName, ex.ToString());
				}
				throw;
			}
			finally
			{
				if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
				{
					Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.GenerateParameterName);
				}
			}
		}

		/// <summary>
		/// ���ɱ�ʶ���������������ȣ��ķָ�SQL��ʾ��ʽ
		/// </summary>
		/// <param name="identifier">Ҫ�ָ��ı�ʶ��</param>
		/// <returns>���ɵ��ַ���</returns>
		public override string DelimitIdentifier(string identifier)
		{
			// ˵����
			// Oracle�Ķ���(�����еȵ�)���Ǵ�д��(��������Ҫ)�������ִ�Сд��������˫�����Ժ�ʹ�Сд���У����磺
			// DEPARTMENT���"Department"����Ϊ��������ͬ�ı�
			// �ڽ���SQLת��ʱ���������޷��Ƿ�ֹ�����ȶ����а����ո���ʵ����û�б�Ҫ��
			// ��Ȼ�����а����ո������ݿ�֧�ֵ����ԣ�����һ�������ϵͳ�ж�������Ⲣ�ž������а������ţ���Ϊ���ʹ�ú͹����������Ĳ��㡣
			// ��ˣ���Ӧ�������Ž�����������������Ϊ������SQL�Ŀɶ��ԣ�Ҳ��Ӧ�ý�����ת���ɴ�д��
			//string id = Check.NotEmpty(identifier, nameof(identifier));
			//return "\"" + base.EscapeIdentifier(id.ToUpper()) + "\"";

			return base.EscapeIdentifier(identifier);
		}


		/// <summary>
		/// д���ʶ���������������ȣ��ķָ�SQL��ʾ��ʽ��
		/// </summary>
		/// <param name="builder">Ҫ�����ɵ��ַ���д����ַ���������</param>
		/// <param name="identifier">Ҫ�ָ��ı�ʶ��</param>
		public override void DelimitIdentifier(StringBuilder builder, string identifier)
		{
			// ˵����
			// Oracle�Ķ���(�����еȵ�)���Ǵ�д��(��������Ҫ)�������ִ�Сд��������˫�����Ժ�ʹ�Сд���У����磺
			// DEPARTMENT���"Department"����Ϊ��������ͬ�ı�
			// �ڽ���SQLת��ʱ���������޷��Ƿ�ֹ�����ȶ����а����ո���ʵ����û�б�Ҫ��
			// ��Ȼ�����а����ո������ݿ�֧�ֵ����ԣ�����һ�������ϵͳ�ж�������Ⲣ�ž������а������ţ���Ϊ���ʹ�ú͹����������Ĳ��㡣
			// ��ˣ���Ӧ�������Ž�����������������Ϊ������SQL�Ŀɶ��ԣ�Ҳ��Ӧ�ý�����ת���ɴ�д��

			//try
			//{
			//	if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			//	{
			//		Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Entry, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.DelimitIdentifier);
			//	}

			//	Check.NotEmpty(identifier, nameof(identifier));
			//	builder.Append('"');
			//	base.EscapeIdentifier(builder, identifier.ToUpper());
			//	builder.Append('"');
			//}
			//catch (Exception ex)
			//{
			//	if (Check.IsErrorEnabled(m_oracleLogger?.Logger))
			//	{
			//		Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Error, OracleTraceTag.Error, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.DelimitIdentifier, ex.ToString());
			//	}
			//	throw;
			//}
			//finally
			//{
			//	if (Check.IsTraceEnabled(m_oracleLogger?.Logger))
			//	{
			//		Trace<DbLoggerCategory.Database.Command>.Write(m_oracleLogger, LogLevel.Trace, OracleTraceTag.Exit, OracleTraceClassName.OracleSqlGenerationHelper, OracleTraceFuncName.DelimitIdentifier);
			//	}
			//}

			base.EscapeIdentifier(builder, identifier);
		}
	}
}

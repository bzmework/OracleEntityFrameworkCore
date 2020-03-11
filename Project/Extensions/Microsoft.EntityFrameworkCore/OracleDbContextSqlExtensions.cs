using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Oracle.ManagedDataAccess.Client;
using System.Globalization;
using Oracle.ManagedDataAccess.Types;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// 数据库上下文Sql扩展，支持采用sql查询或执行。
    /// 暂不支持参数化sql。
    /// </summary>
    public static class OracleDbContextSqlExtensions
    {
        /// <summary>
        /// 执行Sq查询, 将结果集转换为实体集合
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="db">数据库上下文</param>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>实体集合</returns>
        public static List<T> ExecuteQuery<T>(this DbContext db, string sql) where T : class
        {
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                // 参数化
                //var ifs = db.Database as IInfrastructure<IServiceProvider>;
                //var conn = ifs.GetService<IRelationalConnection>();
                //var cmd = ifs.GetService<IRawSqlCommandBuilder>();
                //var rawSqlCommand = cmd.Build(sql);

                //using (var reader = rawSqlCommand.ExecuteReader(conn))
                //{
                //    using (reader.DbDataReader)
                //    {
                //        return ConvertReader<T>(reader.DbDataReader);
                //    }
                //}

                List<T> et = null;
                var conn = db.Database.GetDbConnection();
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        OpenConn(conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        var reader = cmd.ExecuteReader();
                        using (reader)
                        {
                            et = ConvertReader<T>(reader);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return et;
            }
        }

        /// <summary>
        /// 执行Sq查询
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteQuery(this DbContext db, string sql)
        {
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                DataTable dt = null;
                var conn = db.Database.GetDbConnection();
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        OpenConn(conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        var reader = cmd.ExecuteReader();
                        using (reader)
                        {
                            dt = ConvertReader(reader);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return dt;
            }
        }

        /// <summary>
        /// 异步执行Sq查询, 将结果集转换为实体集合
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="db">数据库上下文</param>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>数据集合</returns>
        public static async Task<List<T>> ExecuteQueryAsync<T>(this DbContext db, string sql) where T : class
        {
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                List<T> et = null;
                var conn = db.Database.GetDbConnection();
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        OpenConn(conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        var reader = await cmd.ExecuteReaderAsync();
                        using (reader)
                        {
                            et = ConvertReader<T>(reader);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return et;
            }
        }

        /// <summary>
        /// 异步执行Sq查询
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>DataTable</returns>
        public static async Task<DataTable> ExecuteQueryAsync(this DbContext db, string sql)
        {
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                DataTable dt = null;
                var conn = db.Database.GetDbConnection();
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        OpenConn(conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        var reader = await cmd.ExecuteReaderAsync();
                        using (reader)
                        {
                            dt = ConvertReader(reader);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return dt;
            }
        }

        /// <summary>
        /// 执行命令,返回第一行,第一列的值,并将结果转换为T类型
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="db">数据库上下文</param>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>结果集的第一行,第一列</returns>
        public static T ExecuteScalar<T>(this DbContext db, string sql)
        {
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                T t = default;
                var conn = db.Database.GetDbConnection();
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        OpenConn(conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        var result = cmd.ExecuteScalar();
                        t = ConvertScalar<T>(result);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return t;
            }
        }

        /// <summary>
        /// 异步执行命令,返回第一行,第一列的值,并将结果转换为T类型
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="db">数据库上下文</param>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>结果集的第一行,第一列</returns>
        public static async Task<T> ExecuteScalarAsync<T>(this DbContext db, string sql)
        {
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                T t = default;
                var conn = db.Database.GetDbConnection();
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        OpenConn(conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        var result = await cmd.ExecuteScalarAsync();
                        t = ConvertScalar<T>(result);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return t;
            }
        }

        /// <summary>
        /// 执行命令,返回第一行,第一列的值
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>结果集的第一行,第一列</returns>
        public static object ExecuteScalar(this DbContext db, string sql)
        {
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                Object result = null;
                var conn = db.Database.GetDbConnection();
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        OpenConn(conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        result = cmd.ExecuteScalar();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 异步执行命令,返回第一行,第一列的值
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>结果集的第一行,第一列</returns>
        public static async Task<object> ExecuteScalarAsync(this DbContext db, string sql)
        {
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                Object result = null;
                var conn = db.Database.GetDbConnection();
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        OpenConn(conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        result = await cmd.ExecuteScalarAsync();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 执行命令,并返回影响行数
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="sql">执行的SQL语句</param>
        /// <returns>影响行数</returns>
        public static int Execute(this DbContext db, string sql)
        {
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                int result = -1;
                var conn = db.Database.GetDbConnection();
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        OpenConn(conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 执行命令,并返回影响行数
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="sql">执行的SQL语句</param>
        /// <returns>影响行数</returns>
        public static async Task<int> ExecuteAsync(this DbContext db, string sql)
        {
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                int result = 0;
                var conn = db.Database.GetDbConnection();
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        OpenConn(conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        result = await cmd.ExecuteNonQueryAsync();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 调用Oracle函数
        /// </summary>
        /// <typeparam name="T">返回的数据类型,支持的数据类型有：DateTime,String,Decimal,Double,Single,Int64,Int32,Int16,Byte,Boolean</typeparam>
        /// <param name="db">数据库上下文</param>
        /// <param name="funcName">函数名称</param>
        /// <param name="paramList">参数列表,输入参数支持的数据类型有：DateTime,String,Decimal,Double,Single,Int64,Int32,Int16,Byte,Boolean</param>
        /// <returns></returns>
        public static T CallFunc<T>(this DbContext db, string funcName, params object[] paramList)
        {
            // 验证返回类型是否受支持
            CheckTypeIsSupported(typeof(T));

            // 验证参数类型是否受支持
            foreach (var param in paramList)
            {
                if (param == null)
                {
                    throw new Exception("参数不能为null值");
                }
                else
                {
                    CheckTypeIsSupported(param.GetType());
                }
            }

            // 调用Oracle函数
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                T result = default;
                var conn = db.Database.GetDbConnection(); // 获取当前数据库连接
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        // 设置存储过程名称
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.CommandText = "{?=Call GetCurrencyDec(?)}"; // ADO的这种调用方式不再支持
                        cmd.CommandText = funcName;

                        // 设置返回参数
                        OracleParameter pReturn = new OracleParameter();
                        //pReturn.ParameterName = ":returnValue"; // 采用变量绑定
                        //pReturn.ParameterName = "@returnValue"; // 或者采用参数名
                        var rtypeCode = Type.GetTypeCode(typeof(T));
                        switch (rtypeCode)
                        {
                            case TypeCode.DateTime: // 日期
                                pReturn.OracleDbType = OracleDbType.Date; 
                                break;
                            case TypeCode.String: // 字符串
                                pReturn.OracleDbType = OracleDbType.Varchar2;
                                pReturn.Size = 4000; // 采用最大兼容长度
                                break;
                            case TypeCode.Decimal: // 数值
                                pReturn.OracleDbType = OracleDbType.Decimal;
                                break;
                            case TypeCode.Double:
                                pReturn.OracleDbType = OracleDbType.Double;
                                break;
                            case TypeCode.Single:
                                pReturn.OracleDbType = OracleDbType.Single;
                                break;
                            case TypeCode.Int64:
                                pReturn.OracleDbType = OracleDbType.Int64;
                                break;
                            case TypeCode.Int32:
                                pReturn.OracleDbType = OracleDbType.Int32;
                                break;
                            case TypeCode.Int16:
                                pReturn.OracleDbType = OracleDbType.Int16;
                                break;
                            case TypeCode.Byte:
                                pReturn.OracleDbType = OracleDbType.Byte;
                                break;
                            case TypeCode.Boolean: // 布尔
                                pReturn.OracleDbType = OracleDbType.Boolean; 
                                break;
                            default:
                                throw new Exception($"不支持返回类型{rtypeCode.ToString()}");
                        }
                        pReturn.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(pReturn);

                        // 设置输入输出参数
                        if (paramList != null && paramList.Length > 0)
                        {
                            foreach(var param in paramList)
                            {
                                
                                var typeCode = Type.GetTypeCode(param.GetType());
                                switch (typeCode)
                                {
                                    case TypeCode.DateTime: // 日期
                                        {
                                            OracleParameter pDate = new OracleParameter();
                                            pDate.OracleDbType = OracleDbType.Date;
                                            pDate.Direction = ParameterDirection.Input;
                                            pDate.Value = param;
                                            cmd.Parameters.Add(pDate);
                                        }
                                        break;
                                    case TypeCode.String: // 字符串
                                        {
                                            OracleParameter pStr = new OracleParameter();
                                            pStr.OracleDbType = OracleDbType.Varchar2;
                                            pStr.Direction = ParameterDirection.Input;
                                            pStr.Size = 4000; // 采用最大兼容长度
                                            pStr.Value = C2Dbs(param);
                                            cmd.Parameters.Add(pStr);
                                        }
                                        break;
                                    case TypeCode.Decimal: // 数值
                                        {
                                            OracleParameter pDec = new OracleParameter();
                                            pDec.OracleDbType = OracleDbType.Decimal;
                                            pDec.Direction = ParameterDirection.Input;
                                            pDec.Value = param;
                                            cmd.Parameters.Add(pDec);
                                        }
                                        break;
                                    case TypeCode.Double:
                                        {
                                            OracleParameter pDbl = new OracleParameter();
                                            pDbl.OracleDbType = OracleDbType.Double;
                                            pDbl.Direction = ParameterDirection.Input;
                                            pDbl.Value = param;
                                            cmd.Parameters.Add(pDbl);
                                        }
                                        break;
                                    case TypeCode.Single:
                                        {
                                            OracleParameter pFlt = new OracleParameter();
                                            pFlt.OracleDbType = OracleDbType.Single;
                                            pFlt.Direction = ParameterDirection.Input;
                                            pFlt.Value = param;
                                            cmd.Parameters.Add(pFlt);
                                        }
                                        break;
                                    case TypeCode.Int64:
                                        {
                                            OracleParameter pLng = new OracleParameter();
                                            pLng.OracleDbType = OracleDbType.Int64;
                                            pLng.Direction = ParameterDirection.Input;
                                            pLng.Value = param;
                                            cmd.Parameters.Add(pLng);
                                        }
                                        break;
                                    case TypeCode.Int32:
                                        {
                                            OracleParameter pInt = new OracleParameter();
                                            pInt.OracleDbType = OracleDbType.Int32;
                                            pInt.Direction = ParameterDirection.Input;
                                            pInt.Value = param;
                                            cmd.Parameters.Add(pInt);
                                        }
                                        break;
                                    case TypeCode.Int16:
                                        {
                                            OracleParameter pShort = new OracleParameter();
                                            pShort.OracleDbType = OracleDbType.Int16;
                                            pShort.Direction = ParameterDirection.Input;
                                            pShort.Value = param;
                                            cmd.Parameters.Add(pShort);
                                        }
                                        break;
                                    case TypeCode.Byte:
                                        {
                                            OracleParameter pByte = new OracleParameter();
                                            pByte.OracleDbType = OracleDbType.Byte;
                                            pByte.Direction = ParameterDirection.Input;
                                            pByte.Value = param;
                                            cmd.Parameters.Add(pByte);
                                        }
                                        break;
                                    case TypeCode.Boolean: // 布尔
                                        {
                                            OracleParameter pBool = new OracleParameter();
                                            pBool.OracleDbType = OracleDbType.Boolean;
                                            pBool.Direction = ParameterDirection.Input;
                                            pBool.Value = ((bool)param) ? 1 : 0;
                                            cmd.Parameters.Add(pBool);
                                        }
                                        break;
                                    default:
                                        {
                                            throw new Exception($"不支持的参数类型{typeCode.ToString()}");
                                        }
                                }
                            }
                        }

                        // 连接到数据库执行
                        OpenConn(conn);
                        bool ok = cmd.ExecuteNonQuery() == -1;
                        if(ok)
                        {
                            // 设置返回值
                            object value = null;
                            var retValue = pReturn.Value;
                            var typeName = retValue.GetType().Name;
                            switch (typeName)
                            {
                                case nameof(OracleDate): // 日期
                                    switch (rtypeCode)
                                    {
                                        case TypeCode.String: // 字符串
                                            value = C2Str((OracleDate)retValue); break;
                                        case TypeCode.DateTime: // 日期
                                            value = C2Date((OracleDate)retValue); break;
                                        default:
                                            throw new Exception($"不支持将{nameof(OracleDate)}类型转换成返回类型{rtypeCode.ToString()}");
                                    }
                                    break;
                                case nameof(OracleString): // 字符串
                                    switch (rtypeCode)
                                    {
                                        case TypeCode.String: // 字符串
                                            value = C2Str((OracleString)retValue); break;
                                        default:
                                            throw new Exception($"不支持将{nameof(OracleString)}类型转换成返回类型{rtypeCode.ToString()}");
                                    }
                                    break;

                                case nameof(OracleDecimal): // 数值
                                    {
                                        switch (rtypeCode)
                                        {
                                            case TypeCode.String: // 字符串
                                                value = C2Str((OracleDecimal)retValue); break;
                                            case TypeCode.Decimal: // 数值
                                                value = C2Dec((OracleDecimal)retValue); break;
                                            case TypeCode.Double:
                                                value = C2Dbl((OracleDecimal)retValue); break;
                                            case TypeCode.Single:
                                                value = C2Flt((OracleDecimal)retValue); break;
                                            case TypeCode.Int64:
                                                value = C2Lng((OracleDecimal)retValue); break;
                                            case TypeCode.Int32:
                                                value = C2Int((OracleDecimal)retValue); break;
                                            case TypeCode.Int16:
                                                value = C2Short((OracleDecimal)retValue); break;
                                            case TypeCode.Byte:
                                                value = C2Byte((OracleDecimal)retValue); break;
                                            case TypeCode.Boolean: // 布尔
                                                value = C2Bool((OracleDecimal)retValue); break;
                                            default:
                                                throw new Exception($"不支持将{nameof(OracleDecimal)}类型转换成返回类型{rtypeCode.ToString()}");
                                        }
                                    }
                                    break;
                                case nameof(OracleBoolean): // 布尔
                                    switch (rtypeCode)
                                    {
                                        case TypeCode.String: // 字符串
                                            value = C2Str((OracleBoolean)retValue); break;
                                        case TypeCode.Boolean: // 日期
                                            value = C2Bool((OracleBoolean)retValue); break;
                                        default:
                                            throw new Exception($"不支持将{nameof(OracleBoolean)}类型转换成返回类型{rtypeCode.ToString()}");
                                    }
                                    break;
                                default:
                                    throw new Exception($"不支持返回类型{rtypeCode.ToString()}");
                            }

                            result = (T)value;
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 调用Oracle函数
        /// </summary>
        /// <typeparam name="T">返回的数据类型,支持的数据类型有：DateTime,String,Decimal,Double,Single,Int64,Int32,Int16,Byte,Boolean</typeparam>
        /// <param name="db">数据库上下文</param>
        /// <param name="funcName">函数名称</param>
        /// <param name="paramList">参数列表, 其成员必须是object[]，且只能有2个元素，第一个元素只能是:In/Out/InOut，第二个元素支持的数据类型有：DateTime,String,Decimal,Double,Single,Int64,Int32,Int16,Byte,Boolean</param>
        /// <param name="valueList">输出值列表, 支持的数据类型有：DateTime,String,Decimal,Double,Single,Int64,Int32,Int16,Byte,Boolean</param>
        /// <returns></returns>
        public static T CallFunc<T>(this DbContext db, string funcName, IList<object[]> paramList, IList<object> valueList)
        {
            // 验证返回类型是否受支持
            CheckTypeIsSupported(typeof(T));

            // 验证输入参数类型是否受支持
            foreach (var param in paramList)
            {
                if(param.Length > 2)
                {
                    throw new Exception("object[]只能有2个元素");
                }
                else
                {
                    var pDir = param[0].ToString();
                    var pVal = param[1];
                    if (!(pDir.Equals("In", StringComparison.OrdinalIgnoreCase) 
                        || pDir.Equals("Out", StringComparison.OrdinalIgnoreCase)
                        || pDir.Equals("InOut", StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new Exception("object[]第一个元素只能是:In/Out/InOut");
                    }
                    else
                    {
                        if (pVal == null)
                        {
                            throw new Exception("参数不能为null值");
                        }
                        else
                        {
                            CheckTypeIsSupported(pVal.GetType());
                        }
                    }
                }
            }

            // 调用Oracle函数
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                T result = default;
                var conn = db.Database.GetDbConnection(); // 获取当前数据库连接
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        // 设置存储过程名称
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.CommandText = "{?=Call GetCurrencyDec(?)}"; // ADO的这种调用方式不再支持
                        cmd.CommandText = funcName;

                        // 设置返回参数
                        OracleParameter pReturn = new OracleParameter();
                        //pReturn.ParameterName = ":returnValue"; // 采用变量绑定
                        //pReturn.ParameterName = "@returnValue"; // 或者采用参数名
                        var rtypeCode = Type.GetTypeCode(typeof(T));
                        switch (rtypeCode)
                        {
                            case TypeCode.DateTime: // 日期
                                pReturn.OracleDbType = OracleDbType.Date;
                                break;
                            case TypeCode.String: // 字符串
                                pReturn.OracleDbType = OracleDbType.Varchar2;
                                pReturn.Size = 4000; // 采用最大兼容长度
                                break;
                            case TypeCode.Decimal: // 数值
                                pReturn.OracleDbType = OracleDbType.Decimal;
                                break;
                            case TypeCode.Double:
                                pReturn.OracleDbType = OracleDbType.Double;
                                break;
                            case TypeCode.Single:
                                pReturn.OracleDbType = OracleDbType.Single;
                                break;
                            case TypeCode.Int64:
                                pReturn.OracleDbType = OracleDbType.Int64;
                                break;
                            case TypeCode.Int32:
                                pReturn.OracleDbType = OracleDbType.Int32;
                                break;
                            case TypeCode.Int16:
                                pReturn.OracleDbType = OracleDbType.Int16;
                                break;
                            case TypeCode.Byte:
                                pReturn.OracleDbType = OracleDbType.Byte;
                                break;
                            case TypeCode.Boolean: // 布尔
                                pReturn.OracleDbType = OracleDbType.Boolean;
                                break;
                            default:
                                throw new Exception($"不支持返回类型{rtypeCode.ToString()}");
                        }
                        pReturn.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(pReturn);

                        // 设置输入/输出参数
                        if (paramList != null && paramList.Count > 0)
                        {
                            foreach (var param in paramList)
                            {
                                // 参数方向
                                ParameterDirection paramDirection;
                                var paramValue = param[0];
                                if (paramValue.ToString().Equals("In", StringComparison.OrdinalIgnoreCase))
                                {
                                    paramDirection = ParameterDirection.Input;
                                }
                                else if (paramValue.ToString().Equals("Out", StringComparison.OrdinalIgnoreCase))
                                {
                                    paramDirection = ParameterDirection.Output;
                                }
                                else
                                {
                                    paramDirection = ParameterDirection.InputOutput;
                                }

                                // 参数值
                                paramValue = param[1];
                                var typeCode = Type.GetTypeCode(paramValue.GetType());
                                switch (typeCode)
                                {
                                    case TypeCode.DateTime: // 日期
                                        {
                                            OracleParameter pDate = new OracleParameter();
                                            pDate.OracleDbType = OracleDbType.Date;
                                            pDate.Direction = paramDirection;
                                            pDate.Value = paramValue;
                                            cmd.Parameters.Add(pDate);
                                        }
                                        break;
                                    case TypeCode.String: // 字符串
                                        {
                                            OracleParameter pStr = new OracleParameter();
                                            pStr.OracleDbType = OracleDbType.Varchar2;
                                            pStr.Direction = paramDirection;
                                            pStr.Size = 4000; // 采用最大兼容长度
                                            pStr.Value = C2Dbs(paramValue);
                                            cmd.Parameters.Add(pStr);
                                        }
                                        break;
                                    case TypeCode.Decimal: // 数值
                                        {
                                            OracleParameter pDec = new OracleParameter();
                                            pDec.OracleDbType = OracleDbType.Decimal;
                                            pDec.Direction = paramDirection;
                                            pDec.Value = paramValue;
                                            cmd.Parameters.Add(pDec);
                                        }
                                        break;
                                    case TypeCode.Double:
                                        {
                                            OracleParameter pDbl = new OracleParameter();
                                            pDbl.OracleDbType = OracleDbType.Double;
                                            pDbl.Direction = paramDirection;
                                            pDbl.Value = paramValue;
                                            cmd.Parameters.Add(pDbl);
                                        }
                                        break;
                                    case TypeCode.Single:
                                        {
                                            OracleParameter pFlt = new OracleParameter();
                                            pFlt.OracleDbType = OracleDbType.Single;
                                            pFlt.Direction = paramDirection;
                                            pFlt.Value = paramValue;
                                            cmd.Parameters.Add(pFlt);
                                        }
                                        break;
                                    case TypeCode.Int64:
                                        {
                                            OracleParameter pLng = new OracleParameter();
                                            pLng.OracleDbType = OracleDbType.Int64;
                                            pLng.Direction = paramDirection;
                                            pLng.Value = paramValue;
                                            cmd.Parameters.Add(pLng);
                                        }
                                        break;
                                    case TypeCode.Int32:
                                        {
                                            OracleParameter pInt = new OracleParameter();
                                            pInt.OracleDbType = OracleDbType.Int32;
                                            pInt.Direction = paramDirection;
                                            pInt.Value = paramValue;
                                            cmd.Parameters.Add(pInt);
                                        }
                                        break;
                                    case TypeCode.Int16:
                                        {
                                            OracleParameter pShort = new OracleParameter();
                                            pShort.OracleDbType = OracleDbType.Int16;
                                            pShort.Direction = paramDirection;
                                            pShort.Value = paramValue;
                                            cmd.Parameters.Add(pShort);
                                        }
                                        break;
                                    case TypeCode.Byte:
                                        {
                                            OracleParameter pByte = new OracleParameter();
                                            pByte.OracleDbType = OracleDbType.Byte;
                                            pByte.Direction = paramDirection;
                                            pByte.Value = paramValue;
                                            cmd.Parameters.Add(pByte);
                                        }
                                        break;
                                    case TypeCode.Boolean: // 布尔
                                        {
                                            OracleParameter pBool = new OracleParameter();
                                            pBool.OracleDbType = OracleDbType.Double;
                                            pBool.Direction = paramDirection;
                                            pBool.Value = ((bool)paramValue) ? 1 : 0;
                                            cmd.Parameters.Add(pBool);
                                        }
                                        break;
                                    default:
                                        {
                                            throw new Exception($"不支持的参数类型{typeCode.ToString()}");
                                        }
                                }
                            }
                        }

                        // 连接到数据库执行
                        OpenConn(conn);
                        bool ok = cmd.ExecuteNonQuery() == -1;
                        if (ok)
                        {
                            object value;
                            string typeName;

                            // 设置输出值
                            value = null;
                            valueList.Clear();
                            for (int i = 1; i < cmd.Parameters.Count; i++)
                            {
                                if (cmd.Parameters[i].Direction == ParameterDirection.Output || 
                                    cmd.Parameters[i].Direction == ParameterDirection.InputOutput)
                                {
                                    var outValue = cmd.Parameters[i].Value;
                                    typeName = outValue.GetType().Name;
                                    switch (typeName)
                                    {
                                        case nameof(OracleDate): // 日期
                                            value = C2Date((OracleDate)outValue); break;
                                        case nameof(OracleString): // 字符串
                                            value = C2Str((OracleString)outValue); break;
                                        case nameof(OracleDecimal): // 数值
                                            value = C2Dec((OracleDecimal)outValue); break;
                                        case nameof(OracleBoolean): // 布尔
                                            value = C2Bool((OracleBoolean)outValue); break;
                                        default:
                                            throw new Exception($"不支持返回类型{typeName}");
                                    }
                                    valueList.Add(value);
                                }
                            }

                            // 设置返回值
                            value = null;
                            var retValue = pReturn.Value;
                            typeName = retValue.GetType().Name;
                            switch (typeName)
                            {
                                case nameof(OracleDate): // 日期
                                    switch (rtypeCode)
                                    {
                                        case TypeCode.String: // 字符串
                                            value = C2Str((OracleDate)retValue); break;
                                        case TypeCode.DateTime: // 日期
                                            value = C2Date((OracleDate)retValue); break;
                                        default:
                                            throw new Exception($"不支持将{nameof(OracleDate)}类型转换成返回类型{rtypeCode.ToString()}");
                                    }
                                    break;
                                case nameof(OracleString): // 字符串
                                    switch (rtypeCode)
                                    {
                                        case TypeCode.String: // 字符串
                                            value =C2Str((OracleString)retValue); break;
                                        default:
                                            throw new Exception($"不支持将{nameof(OracleString)}类型转换成返回类型{rtypeCode.ToString()}");
                                    }
                                    break;

                                case nameof(OracleDecimal): // 数值
                                    {
                                        switch (rtypeCode)
                                        {
                                            case TypeCode.String: // 字符串
                                                value = C2Str((OracleDecimal)retValue); break;
                                            case TypeCode.Decimal: // 数值
                                                value = C2Dec((OracleDecimal)retValue); break;
                                            case TypeCode.Double:
                                                value = C2Dbl((OracleDecimal)retValue); break;
                                            case TypeCode.Single:
                                                value = C2Flt((OracleDecimal)retValue); break;
                                            case TypeCode.Int64:
                                                value = C2Lng((OracleDecimal)retValue); break;
                                            case TypeCode.Int32:
                                                value = C2Int((OracleDecimal)retValue); break;
                                            case TypeCode.Int16:
                                                value = C2Short((OracleDecimal)retValue); break;
                                            case TypeCode.Byte:
                                                value = C2Byte((OracleDecimal)retValue); break;
                                            case TypeCode.Boolean: // 布尔
                                                value = C2Bool((OracleDecimal)retValue); break;
                                            default:
                                                throw new Exception($"不支持将{nameof(OracleDecimal)}类型转换成返回类型{rtypeCode.ToString()}");
                                        }
                                    }
                                    break;
                                case nameof(OracleBoolean): // 布尔
                                    switch (rtypeCode)
                                    {
                                        case TypeCode.String: // 字符串
                                            value = C2Str((OracleBoolean)retValue); break;
                                        case TypeCode.Boolean: // 日期
                                            value = C2Bool((OracleBoolean)retValue); break;
                                        default:
                                            throw new Exception($"不支持将{nameof(OracleBoolean)}类型转换成返回类型{rtypeCode.ToString()}");
                                    }
                                    break;
                                default:
                                    throw new Exception($"不支持返回类型{rtypeCode.ToString()}");
                            }

                            result = (T)value;
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 调用Oracle过程
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="procName">过程名称</param>
        /// <param name="paramList">参数列表,输入参数支持的数据类型有：DateTime,String,Decimal,Double,Single,Int64,Int32,Int16,Byte,Boolean</param>
        /// <returns></returns>
        public static bool CallProc(this DbContext db, string procName, params object[] paramList)
        {
            // 验证参数类型是否受支持
            foreach (var param in paramList)
            {
                if (param == null)
                {
                    throw new Exception("参数不能为null值");
                }
                else
                {
                    CheckTypeIsSupported(param.GetType());
                }
            }

            // 调用Oracle函数
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                bool result = false;
                var conn = db.Database.GetDbConnection(); // 获取当前数据库连接
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        // 设置存储过程名称
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.CommandText = "{?=Call GetCurrencyDec(?)}"; // ADO的这种调用方式不再支持
                        cmd.CommandText = procName;

                        // 设置输入输出参数
                        if (paramList != null && paramList.Length > 0)
                        {
                            foreach (var param in paramList)
                            {

                                var typeCode = Type.GetTypeCode(param.GetType());
                                switch (typeCode)
                                {
                                    case TypeCode.DateTime: // 日期
                                        {
                                            OracleParameter pDate = new OracleParameter();
                                            pDate.OracleDbType = OracleDbType.Date;
                                            pDate.Direction = ParameterDirection.Input;
                                            pDate.Value = param;
                                            cmd.Parameters.Add(pDate);
                                        }
                                        break;
                                    case TypeCode.String: // 字符串
                                        {
                                            OracleParameter pStr = new OracleParameter();
                                            pStr.OracleDbType = OracleDbType.Varchar2;
                                            pStr.Direction = ParameterDirection.Input;
                                            pStr.Size = 4000; // 采用最大兼容长度
                                            pStr.Value = C2Dbs(param);
                                            cmd.Parameters.Add(pStr);
                                        }
                                        break;
                                    case TypeCode.Decimal: // 数值
                                        {
                                            OracleParameter pDec = new OracleParameter();
                                            pDec.OracleDbType = OracleDbType.Decimal;
                                            pDec.Direction = ParameterDirection.Input;
                                            pDec.Value = param;
                                            cmd.Parameters.Add(pDec);
                                        }
                                        break;
                                    case TypeCode.Double:
                                        {
                                            OracleParameter pDbl = new OracleParameter();
                                            pDbl.OracleDbType = OracleDbType.Double;
                                            pDbl.Direction = ParameterDirection.Input;
                                            pDbl.Value = param;
                                            cmd.Parameters.Add(pDbl);
                                        }
                                        break;
                                    case TypeCode.Single:
                                        {
                                            OracleParameter pFlt = new OracleParameter();
                                            pFlt.OracleDbType = OracleDbType.Single;
                                            pFlt.Direction = ParameterDirection.Input;
                                            pFlt.Value = param;
                                            cmd.Parameters.Add(pFlt);
                                        }
                                        break;
                                    case TypeCode.Int64:
                                        {
                                            OracleParameter pLng = new OracleParameter();
                                            pLng.OracleDbType = OracleDbType.Int64;
                                            pLng.Direction = ParameterDirection.Input;
                                            pLng.Value = param;
                                            cmd.Parameters.Add(pLng);
                                        }
                                        break;
                                    case TypeCode.Int32:
                                        {
                                            OracleParameter pInt = new OracleParameter();
                                            pInt.OracleDbType = OracleDbType.Int32;
                                            pInt.Direction = ParameterDirection.Input;
                                            pInt.Value = param;
                                            cmd.Parameters.Add(pInt);
                                        }
                                        break;
                                    case TypeCode.Int16:
                                        {
                                            OracleParameter pShort = new OracleParameter();
                                            pShort.OracleDbType = OracleDbType.Int16;
                                            pShort.Direction = ParameterDirection.Input;
                                            pShort.Value = param;
                                            cmd.Parameters.Add(pShort);
                                        }
                                        break;
                                    case TypeCode.Byte:
                                        {
                                            OracleParameter pByte = new OracleParameter();
                                            pByte.OracleDbType = OracleDbType.Byte;
                                            pByte.Direction = ParameterDirection.Input;
                                            pByte.Value = param;
                                            cmd.Parameters.Add(pByte);
                                        }
                                        break;
                                    case TypeCode.Boolean: // 布尔
                                        {
                                            OracleParameter pBool = new OracleParameter();
                                            pBool.OracleDbType = OracleDbType.Boolean;
                                            pBool.Direction = ParameterDirection.Input;
                                            pBool.Value = ((bool)param) ? 1 : 0;
                                            cmd.Parameters.Add(pBool);
                                        }
                                        break;
                                    default:
                                        {
                                            throw new Exception($"不支持的参数类型{typeCode.ToString()}");
                                        }
                                }
                            }
                        }

                        // 连接到数据库执行
                        OpenConn(conn);
                        result = cmd.ExecuteNonQuery() == -1;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 调用Oracle函数
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <param name="procName">过程名称</param>
        /// <param name="paramList">参数列表, 其成员必须是object[]，且只能有2个元素，第一个元素只能是:In/Out/InOut，第二个元素支持的数据类型有：DateTime,String,Decimal,Double,Single,Int64,Int32,Int16,UInt64,UInt32,UInt16,Byte,Boolean</param>
        /// <param name="valueList">输出值列表, 支持的数据类型有：DateTime,String,Decimal,Double,Single,Int64,Int32,Int16,Byte,Boolean</param>
        /// <returns></returns>
        public static bool CallProc(this DbContext db, string procName, IList<object[]> paramList, IList<object> valueList)
        {
            // 验证输入参数类型是否受支持
            foreach (var param in paramList)
            {
                if (param.Length > 2)
                {
                    throw new Exception("object[]只能有2个元素");
                }
                else
                {
                    var pDir = param[0].ToString();
                    var pVal = param[1];
                    if (!(pDir.Equals("In", StringComparison.OrdinalIgnoreCase)
                        || pDir.Equals("Out", StringComparison.OrdinalIgnoreCase)
                        || pDir.Equals("InOut", StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new Exception("object[]第一个元素只能是:In/Out/InOut");
                    }
                    else
                    {
                        if (pVal == null)
                        {
                            throw new Exception("参数不能为null值");
                        }
                        else
                        {
                            CheckTypeIsSupported(pVal.GetType());
                        }
                    }
                }
            }

            // 调用Oracle函数
            var concurrencyDetector = db.Database.GetService<IConcurrencyDetector>();
            using (concurrencyDetector.EnterCriticalSection())
            {
                bool result = false;
                var conn = db.Database.GetDbConnection(); // 获取当前数据库连接
                using (var cmd = conn.CreateCommand())
                {
                    try
                    {
                        // 设置存储过程名称
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.CommandText = "{?=Call GetCurrencyDec(?)}"; // ADO的这种调用方式不再支持
                        cmd.CommandText = procName;

                        // 设置输入/输出参数
                        if (paramList != null && paramList.Count > 0)
                        {
                            foreach (var param in paramList)
                            {
                                // 参数方向
                                ParameterDirection paramDirection;
                                var paramValue = param[0];
                                if (paramValue.ToString().Equals("In", StringComparison.OrdinalIgnoreCase))
                                {
                                    paramDirection = ParameterDirection.Input;
                                }
                                else if (paramValue.ToString().Equals("Out", StringComparison.OrdinalIgnoreCase))
                                {
                                    paramDirection = ParameterDirection.Output;
                                }
                                else
                                {
                                    paramDirection = ParameterDirection.InputOutput;
                                }

                                // 参数值
                                paramValue = param[1];
                                var typeCode = Type.GetTypeCode(paramValue.GetType());
                                switch (typeCode)
                                {
                                    case TypeCode.DateTime: // 日期
                                        {
                                            OracleParameter pDate = new OracleParameter();
                                            pDate.OracleDbType = OracleDbType.Date;
                                            pDate.Direction = paramDirection;
                                            pDate.Value = paramValue;
                                            cmd.Parameters.Add(pDate);
                                        }
                                        break;
                                    case TypeCode.String: // 字符串
                                        {
                                            OracleParameter pStr = new OracleParameter();
                                            pStr.OracleDbType = OracleDbType.Varchar2;
                                            pStr.Direction = paramDirection;
                                            pStr.Size = 4000; // 采用最大兼容长度
                                            pStr.Value = C2Dbs(paramValue);
                                            cmd.Parameters.Add(pStr);
                                        }
                                        break;
                                    case TypeCode.Decimal: // 数值
                                        {
                                            OracleParameter pDec = new OracleParameter();
                                            pDec.OracleDbType = OracleDbType.Decimal;
                                            pDec.Direction = paramDirection;
                                            pDec.Value = paramValue;
                                            cmd.Parameters.Add(pDec);
                                        }
                                        break;
                                    case TypeCode.Double:
                                        {
                                            OracleParameter pDbl = new OracleParameter();
                                            pDbl.OracleDbType = OracleDbType.Double;
                                            pDbl.Direction = paramDirection;
                                            pDbl.Value = paramValue;
                                            cmd.Parameters.Add(pDbl);
                                        }
                                        break;
                                    case TypeCode.Single:
                                        {
                                            OracleParameter pFlt = new OracleParameter();
                                            pFlt.OracleDbType = OracleDbType.Single;
                                            pFlt.Direction = paramDirection;
                                            pFlt.Value = paramValue;
                                            cmd.Parameters.Add(pFlt);
                                        }
                                        break;
                                    case TypeCode.Int64:
                                        {
                                            OracleParameter pLng = new OracleParameter();
                                            pLng.OracleDbType = OracleDbType.Int64;
                                            pLng.Direction = paramDirection;
                                            pLng.Value = paramValue;
                                            cmd.Parameters.Add(pLng);
                                        }
                                        break;
                                    case TypeCode.Int32:
                                        {
                                            OracleParameter pInt = new OracleParameter();
                                            pInt.OracleDbType = OracleDbType.Int32;
                                            pInt.Direction = paramDirection;
                                            pInt.Value = paramValue;
                                            cmd.Parameters.Add(pInt);
                                        }
                                        break;
                                    case TypeCode.Int16:
                                        {
                                            OracleParameter pShort = new OracleParameter();
                                            pShort.OracleDbType = OracleDbType.Int16;
                                            pShort.Direction = paramDirection;
                                            pShort.Value = paramValue;
                                            cmd.Parameters.Add(pShort);
                                        }
                                        break;
                                    case TypeCode.Byte:
                                        {
                                            OracleParameter pByte = new OracleParameter();
                                            pByte.OracleDbType = OracleDbType.Byte;
                                            pByte.Direction = paramDirection;
                                            pByte.Value = paramValue;
                                            cmd.Parameters.Add(pByte);
                                        }
                                        break;
                                    case TypeCode.Boolean: // 布尔
                                        {
                                            OracleParameter pBool = new OracleParameter();
                                            pBool.OracleDbType = OracleDbType.Boolean;
                                            pBool.Direction = paramDirection;
                                            pBool.Value = ((bool)paramValue) ? 1 : 0;
                                            cmd.Parameters.Add(pBool);
                                        }
                                        break;
                                    default:
                                        {
                                            throw new Exception($"不支持的参数类型{typeCode.ToString()}");
                                        }
                                }
                            }
                        }

                        // 连接到数据库执行
                        OpenConn(conn);
                        result = cmd.ExecuteNonQuery() == -1;
                        if (result)
                        {
                            object value;
                            string typeName;

                            // 设置输出值
                            value = null;
                            valueList.Clear();
                            for (int i = 1; i < cmd.Parameters.Count; i++)
                            {
                                if (cmd.Parameters[i].Direction == ParameterDirection.Output ||
                                    cmd.Parameters[i].Direction == ParameterDirection.InputOutput)
                                {
                                    var outValue = cmd.Parameters[i].Value;
                                    typeName = outValue.GetType().Name;
                                    switch (typeName)
                                    {
                                        case nameof(OracleDate): // 日期
                                            value = C2Date((OracleDate)outValue); break;
                                        case nameof(OracleString): // 字符串
                                            value = C2Str((OracleString)outValue); break;
                                        case nameof(OracleDecimal): // 数值
                                            value = C2Dec((OracleDecimal)outValue); break;
                                        case nameof(OracleBoolean): // 布尔
                                            value = C2Bool((OracleBoolean)outValue); break;
                                        default:
                                            throw new Exception($"不支持返回类型{typeName}");
                                    }
                                    valueList.Add(value);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        CloseConn(conn);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 验证参数类型是否受支持
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private static void CheckTypeIsSupported(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.DateTime: // 日期
                case TypeCode.String: // 字符串
                case TypeCode.Decimal: // 数值
                case TypeCode.Double:
                case TypeCode.Single:
                case TypeCode.Int64:
                case TypeCode.Int32:
                case TypeCode.Int16:
                case TypeCode.Byte:
                case TypeCode.Boolean: // 布尔
                    break;
                default:
                    throw new Exception($"不支持返回类型{typeCode.ToString()}");
            }
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <returns>成功返回True,失败返回False</returns>
        public static bool BeginTrans(this DbContext db)
        {
            bool ret = false;
            try
            {
                if (db.Database.CurrentTransaction == null)
                {
                    db.Database.BeginTransaction();
                }
                ret = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ret;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <returns>成功返回True,失败返回False</returns>
        public static bool CommitTrans(this DbContext db)
        {
            bool ret = false;
            try
            {
                if (db.Database.CurrentTransaction != null)
                {
                    db.Database.CommitTransaction();
                }
                ret = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ret;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="db">数据库上下文</param>
        /// <returns>成功返回True,失败返回False</returns>
        public static bool RollbackTrans(this DbContext db)
        {
            bool ret = false;
            try
            {
                if (db.Database.CurrentTransaction != null)
                {
                    db.Database.RollbackTransaction();
                }
                ret = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ret;
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        private static void OpenConn(DbConnection conn)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        private static void CloseConn(DbConnection conn)
        {
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 转换ExecuteScalar结果为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static T ConvertScalar<T>(object obj)
        {
            if (obj == null || DBNull.Value.Equals(obj))
                return default(T);

            if (obj is T)
                return (T)obj;

            Type type = typeof(T);

            if (type == typeof(object))
                return (T)obj;

            return (T)Convert.ChangeType(obj, type);
        }

        /// <summary>
        /// 转换ExecuteReader结果为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static List<T> ConvertReader<T>(DbDataReader reader) where T : class
        {
            // 获得类型
            Type type = typeof(T);
            var dict = TypeCache.GetType(type);

            // 获得列名
            int count = reader.FieldCount;
            string[] colNames = new string[count];
            for (int i = 0; i < count; i++)
            {
                colNames[i] = reader.GetName(i);
            }

            //int currentIndex = 0;
            //int startIndex = pageSize * pageIndex;
            List<T> list = new List<T>();
            while (reader.Read())
            {
                //if (startIndex > currentIndex++)
                //    continue;

                //if (pageSize > 0 && (currentIndex - startIndex) > pageSize)
                //    break;

                T obj = Activator.CreateInstance(type) as T;
                for (int i = 0; i < colNames.Length; i++)
                {
                    string name = colNames[i];
                    if (dict.TryGetValue(name, out PropertyMap info))
                    {
                        object val = reader.GetValue(i);
                        if (val != null && DBNull.Value.Equals(val) == false)
                        {
                            info.PropertyInfo.SetValue(obj, ConvertValue(val, info.PropertyInfo.PropertyType));
                        }
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// 转换ExecuteReader结果为DataTable
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static DataTable ConvertReader(DbDataReader reader)
        {
            DataTable table = new DataTable();

            int fieldCount = reader.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
            }

            //int currentIndex = 0;
            //int startIndex = pageSize * pageIndex;
            object[] values = new object[fieldCount];
            try
            { 
                table.BeginLoadData();
                while (reader.Read())
                {
                    //if (startIndex > currentIndex++)
                    //    continue;

                    //if (pageSize > 0 && (currentIndex - startIndex) > pageSize)
                    //    break;

                    reader.GetValues(values);
                    table.LoadDataRow(values, true);
                }
            }
            finally
            {
                table.EndLoadData();
            }

            return table;
        }

        /// <summary>
        /// 根据类型转换值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static object ConvertValue(object value, Type targetType)
        {
            if (value == null)
                return null;

            if (targetType == typeof(string))
                return value.ToString();

            Type type = Nullable.GetUnderlyingType(targetType) ?? targetType;
            if (value.GetType() == type)
            {
                return value;
            }

            if (type == typeof(Guid) && value.GetType() == typeof(string))
            {
                return new Guid(value.ToString());
            }

            if (type.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
            }

            return System.Convert.ChangeType(value, type);
        }

        /// <summary>
        /// 将Oracle字符串转换成系统字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string C2Str(OracleString value)
        {
            if (value.IsNull)
            {
                return "";
            }
            else
            {
                return value.Value;
            }
        }

        /// <summary>
        /// 将Oracle日期转换成系统字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string C2Str(OracleDate value)
        {
            if (value.IsNull)
            {
                return "";
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// 将Oracle数值转换成系统字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string C2Str(OracleDecimal value)
        {
            if (value.IsNull)
            {
                return "";
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// 将Oracle布尔转换成系统字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string C2Str(OracleBoolean value)
        {
            if (value.IsNull)
            {
                return "";
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// 将Oracle日期转换成系统日期
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static DateTime C2Date(OracleDate value)
        {
            if (value.IsNull)
            {
                return default;
            }
            else
            {
                return value.Value;
            }
        }

        /// <summary>
        /// 将Oracle数值转换成系统数值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static decimal C2Dec(OracleDecimal value)
        {
            if (value.IsNull)
            {
                return default;
            }
            else
            {
                return value.Value;
            }
        }

        /// <summary>
        /// 将Oracle数值转换成系统Double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static double C2Dbl(OracleDecimal value)
        {
            if (value.IsNull)
            {
                return default;
            }
            else
            {
                return value.ToDouble();
            }
        }

        /// <summary>
        /// 将Oracle数值转换成系统Single
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static float C2Flt(OracleDecimal value)
        {
            if (value.IsNull)
            {
                return default;
            }
            else
            {
                return value.ToSingle();
            }
        }

        /// <summary>
        /// 将Oracle数值转换成系统Int64
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static long C2Lng(OracleDecimal value)
        {
            if (value.IsNull)
            {
                return default;
            }
            else
            {
                return value.ToInt64();
            }
        }

        /// <summary>
        /// 将Oracle数值转换成系统Int32
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int C2Int(OracleDecimal value)
        {
            if (value.IsNull)
            {
                return default;
            }
            else
            {
                return value.ToInt32();
            }
        }

        /// <summary>
        /// 将Oracle数值转换成系统Int16
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static short C2Short(OracleDecimal value)
        {
            if (value.IsNull)
            {
                return default;
            }
            else
            {
                return value.ToInt16();
            }
        }

        /// <summary>
        /// 将Oracle数值转换成系统Byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte C2Byte(OracleDecimal value)
        {
            if (value.IsNull)
            {
                return default;
            }
            else
            {
                return value.ToByte();
            }
        }

        /// <summary>
        /// 将Oracle数值转换成系统Boolean
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool C2Bool(OracleDecimal value)
        {
            if (value.IsNull)
            {
                return default;
            }
            else
            {
                return value.Value > 0 ? true : false;
            }
        }

        /// <summary>
        /// 将Oracle布尔转换成系统Boolean
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool C2Bool(OracleBoolean value)
        {
            if (value.IsNull)
            {
                return default;
            }
            else
            {
                return value.Value;
            }
        }

        /// <summary>
        /// 将object转换成数据库非空字符串
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <returns>转换后的值</returns>
        private static string C2Dbs(object value)
        {
            if (value == null) return " ";
            try
            {
                string val = Convert.ToString(value, CultureInfo.InvariantCulture);
                return string.IsNullOrEmpty(val) ? " " : val;
            }
            catch
            {
                return " ";
            }
        }
    }

    /// <summary>
    /// 类型缓存
    /// </summary>
    internal class TypeCache
    {
        private static BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public;
        private static ConcurrentDictionary<string, Dictionary<string, PropertyMap>> dicTypeInfo = new ConcurrentDictionary<string, Dictionary<string, PropertyMap>>();

        public static Dictionary<string, PropertyMap> GetType(Type type)
        {
            if (!dicTypeInfo.TryGetValue(type.FullName, out Dictionary<string, PropertyMap> description) || description == null)
            {
                PropertyInfo[] properties = type.GetProperties(bindFlags);
                int length = properties.Length;

                description = new Dictionary<string, PropertyMap>(length, StringComparer.OrdinalIgnoreCase);
                foreach (PropertyInfo prop in properties)
                {
                    PropertyMap info = null;
                    var attrColumn = prop.GetCustomAttribute<ColumnAttribute>();
                    info = new PropertyMap { ColumnName = attrColumn != null ? attrColumn.Name : prop.Name, PropertyInfo = prop };
                    description[info.ColumnName] = info;
                }
                dicTypeInfo[type.FullName] = description;
            }

            return description;
        }
    }

    /// <summary>
    /// 属性映射
    /// </summary>
    internal class PropertyMap
    {
        public string ColumnName { get; set; }

        public PropertyInfo PropertyInfo { get; set; }
    }
}

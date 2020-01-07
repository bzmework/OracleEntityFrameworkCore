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
                        CloseConn(conn);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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
                        CloseConn(conn);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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
                        CloseConn(conn);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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
                        CloseConn(conn);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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
                        CloseConn(conn);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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
                        CloseConn(conn);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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
                        CloseConn(conn);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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
                        CloseConn(conn);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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
        public static int ExecuteNonQuery(this DbContext db, string sql)
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
                        result = cmd.ExecuteNonQuery();
                        CloseConn(conn);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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
        public static async Task<int> ExecuteNonQueryAsync(this DbContext db, string sql)
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
                        CloseConn(conn);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        CloseConn(conn);
                    }
                }
                return result;
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

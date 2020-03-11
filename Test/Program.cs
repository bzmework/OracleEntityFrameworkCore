using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // 先将TestSql目录中的：
            // Test.sql
            // GetCurrencyDec.sql
            // 在数据库中执行

            using (OracleDbContext db = new OracleDbContext())
            {
                //List<object[]> paramList1 = new List<object[]>()
                //{
                //    new object[] {"In", "Account" },
                //    new object[] {"In", $"strAccountCode" },
                //    new object[] {"In", 1002 },
                //    new object[] {"In", 1000 },
                //    new object[] {"Out", "Error" }
                //};
                //List<object> valueList1 = new List<object>();
                //var ret = db.CallFunc<byte>("CardCodeMerge", paramList1, valueList1);

                var v = db.DepartmentTypes.FirstOrDefault<DepartmentType>();

                //关联查询
                var x = (from p in db.Rights
                         join q in db.Modules
                         on p.ModuleID equals q.ModuleID
                         select new { p.RightsID, p.RightsCode, p.RightsName, q.ModuleName })
                         .OrderBy(e => e.RightsCode)
                         .Skip(1) // 取第1页
                         .Take(6) // 每页6条
                         .ToList();

                // Linq查询
                var query = from deptTypes in db.DepartmentTypes
                            where deptTypes.DepartmentTypeID == 1000 &&
                                deptTypes.DepartmentTypeCode == "002" &&
                                deptTypes.OrganizationID == 1
                            select deptTypes; // 创建查询
                var result = query.Skip(0).Take(2).ToList(); // 执行查询


                // Linq查询
                var varOther = db.Rights.Where(s => s.RightsCode == "0010001");
                var varDuplicate = varOther.FirstOrDefault();

                // 存储过程调用
                var cmd = db.Database.GetDbConnection().CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.CommandText = "{?=Call GetCurrencyDec(?)}"; // ADO的这种调用方式不再支持
                cmd.CommandText = "GetCurrencyDec"; // 设置存储过程名称

                OracleParameter param0 = new OracleParameter();
                //param0.ParameterName = ":returnValue"; // 采用变量绑定
                //param0.ParameterName = "@returnValue"; // 或者采用参数名
                param0.OracleDbType = OracleDbType.Double;
                param0.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(param0);
                 
                OracleParameter param1 = new OracleParameter();
                param1.OracleDbType = OracleDbType.Double;
                param1.Direction = ParameterDirection.Input;
                param1.Value = 1;
                cmd.Parameters.Add(param1);

                cmd.Connection.Open();
                bool blnOk = cmd.ExecuteNonQuery() == -1;
                var retValue = param0.Value;
                //var retValue = cmd.ExecuteReader(); 
                cmd.Connection.Close();

                // 调用Oracle函数
                var num = db.CallFunc<int>("ReturnNumberTest", 100);
                var str = db.CallFunc<string>("ReturnStringTest", "");
                var date = db.CallFunc<DateTime>("ReturnDateTest", DateTime.Now);

                // 调用Oracle函数(带参数返回)
                var paramList = new List<object[]>()
                {
                    new object[] {"In", 100 },
                    new object[] {"Out", DateTime.Now },
                    new object[] {"InOut", "hello" }
                };
                var valueList = new List<object>();
                num = db.CallFunc<int>("OutParamFuncTest", paramList, valueList);

                // 调用Oracle过程
                var ok = db.CallProc("ProcTest", DateTime.Now);

                // 调用Oracle过程(带参数返回)
                paramList = new List<object[]>()
                {
                    new object[] {"In", 100 },
                    new object[] {"Out", DateTime.Now },
                    new object[] {"InOut", "hello" }
                };
                valueList = new List<object>();
                ok = db.CallProc("OutParamProcTest", paramList, valueList);

                // 采用sql查询返回实体集合
                List<Rights> listRights = db.ExecuteQuery<Rights>("select * from Rights");

                // 采用sql查询返回DataTable
                DataTable dt = db.ExecuteQuery("select lngRightsID, strRightsCode from Rights");

                // 查询权限总数
                int total = db.Rights.Count();
                var x1 = db.Rights.GroupBy(e => e.ModuleID).Select(e => new { e.Key, count = e.Count() }).ToList();
                Console.WriteLine($"权限总数：{total}");

                // 批量修改权限
                /*
                var rightList = new List<Rights>();
                for (int i = 100; i < 103; i++)
                {
                    var item = new Rights
                    {
                        RightsID = i,
                        RightsCode = "test_code_" + i,
                        RightsName = "test_name_" + i,
                        Description = " ",
                        ModuleID = 1
                    };
                    rightList.Add(item);
                }
                db.UpdateRange(rightList);
                */

                // 查询一个权限项
                var rightItem = db.Rights.FirstOrDefault(e => e.RightsCode == "0010001");
                Console.WriteLine($"第一个权限项：{rightItem.RightsCode}, {rightItem.RightsName}");

                // 查询多个权限项
                string[] arrRightNo = new string[] { "0010001", "0010002" };
                var list = db.Rights.Where(e => arrRightNo.Contains(e.RightsCode)).ToList();

                // 分页查询演示
                var pageList = db.Rights
                    .Where(e => e.ModuleID == 1)
                    .OrderBy(e => e.RightsCode)
                    .Skip(2) // 取第2页
                    .Take(2) // 每页2条
                    .ToList();
                Console.WriteLine($"分页查询权限：{pageList.Count}");

                // 统计查询
                string minRightCode = db.Rights.Min(e => e.RightsCode);
                string maxRightCode = db.Rights.Max(e => e.RightsCode);
                string[] arrRightCode = db.Rights.Select(e => e.RightsCode).Distinct().ToArray();
                var sumValue = db.Rights.Sum(e => e.RightsID);

                // 名称过滤
                var rightNames = db.Rights.Where(e => e.RightsName == "增加" || e.RightsName == "删除").ToList();

                // 批量新增权限
                /*
                var rightList = new List<Rights>();
                for (int i = 8; i < 16; i++)
                {
                    var item = new Rights
                    {
                        RightsID = i,
                        RightsCode = "test_code_" + i,
                        RightsName = "test_name_" + i,
                        Description = " ",
                        ModuleID = 1
                    };
                    rightList.Add(item);
                }
                db.AddRange(rightList);
                db.SaveChanges(); // 注意：如果保存成功会立即生效，其它修改、删除一样。
                */



                // 批量删除权限
                //db.RemoveRange(db.Rights.Where(e => rightList.Select(t => t.RightsID).Contains(e.RightsID)));
                //db.SaveChanges();

                // 新增一个权限
                var itemNew = new Rights
                {
                    RightsID = 20,
                    RightsCode = "test_code_20",
                    RightsName = "test_name_20",
                    Description = " ",
                    ModuleID = 1
                };
                db.Entry(itemNew).State = EntityState.Added;
                Console.WriteLine($"新增一个权限Id：{itemNew.RightsID} 数据");

                // 修改一个权限
                long rightID = 2;
                var itemModify = db.Rights.Find(rightID);
                itemModify.RightsName = "testModify";
                db.Entry(itemModify).State = EntityState.Modified;
                Console.WriteLine($"修改权限Id：{rightID} 的名称为：{itemModify.RightsName}");

                // 删除一个权限
                rightID = 4;
                var delItem = db.Rights.Find(rightID);
                db.Remove(delItem);
                Console.WriteLine($"删除权限Id：{rightID}");

                // 保存新增、修改、删除
                db.SaveChanges();

            }

            Console.Read();
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApp
{
    /// <summary>
    /// 权限表
    /// </summary>
    [Table("Rights")]
    public class Rights
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        /// <returns></returns>
        [Key, Column("lngRightsID", TypeName = "number(19)")]
        public long RightsID { get; set; }

        /// <summary>
        /// 权限编码 编号规则：Format(lngModuleID,3) + Format(序号,4)。例如：0010001
        /// </summary>
        /// <returns></returns>
        [Required, Column("strRightsCode", TypeName = "varchar2(100)"), MaxLength(100)]
        public string RightsCode { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        /// <returns></returns>
        [Required, Column("strRightsName", TypeName = "varchar2(100)"), MaxLength(100)]
        public string RightsName { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        /// <returns></returns>
        [Column("strDescription", TypeName = "varchar2(200)"), MaxLength(200)]
        public string Description { get; set; }

        /// <summary>
        /// 所属模块ID 0,编码权限; >0,模块权限
        /// </summary>
        /// <returns></returns>
        [Column("lngModuleID", TypeName = "number(19)")]
        public long ModuleID { get; set; }

    }
}

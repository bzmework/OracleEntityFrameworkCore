using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ConsoleApp
{
    ///<summary>
    ///模块表
    ///</summary>
    [Table("Modules")]
    public class Modules
    {
        ///<summary>
        /// 模块ID 
        ///</summary> 
        [Key, Column("lngModuleID", TypeName = "number(19)")]
        public long ModuleID { get; set; }

        ///<summary>
        /// 模块名称
        ///</summary> 
        [Required, Column("strModuleName", TypeName = "varchar2(100)"), MaxLength(100)]
        public string ModuleName { get; set; }

        ///<summary>
        /// 模块描述 
        ///</summary> 
        [Column("strDescription", TypeName = "varchar2(100)"), MaxLength(200)]
        [Required]
        public string Description { get; set; } 

        ///<summary>
        /// 封存标志 
        ///</summary> 
        [Column("blnIsInActive", TypeName = "number(10)")]
        public bool IsInActive { get; set; } 
    }
}

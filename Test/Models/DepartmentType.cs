using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApp
{
    
    ///<summary>
    ///--部门类型;
    ///--Drop Table DepartmentType; --便于修改后重建表。
    ///</summary>
    [Table("DepartmentType")]
    public class DepartmentType
    {
        ///<summary>
        ///键值ID,Key
        ///</summary>
        [Key, Column("lngDepartmentTypeID", TypeName = "Number(19)")]
        public long DepartmentTypeID { get; set; }

        ///<summary>
        ///机构ID
        ///</summary>
        [Column("lngOrganizationID", TypeName = "Number(19)")]
        public long OrganizationID { get; set; }

        ///<summary>
        ///科室类别编码
        ///</summary>
        [Required, Column("strDepartmentTypeCode", TypeName = "Varchar2(80)"), MaxLength(80)]
        public string DepartmentTypeCode { get; set; }

        ///<summary>
        ///科室类别名称
        ///</summary>
        [Required, Column("strDepartmentTypeName", TypeName = "Varchar2(100)"), MaxLength(100)]
        public string DepartmentTypeName { get; set; }

        ///<summary>
        ///是否停用
        ///</summary>
        [Column("blnIsInActive", TypeName = "Number(1)")]
        public bool IsInActive { get; set; }

        ///<summary>
        ///备注
        ///</summary>
        [Column("strDescription", TypeName = "Varchar2(500)"), MaxLength(500)]
        public string Description { get; set; }

        ///<summary>
        ///创建日期
        ///</summary>
        [Column("strCreateDate", TypeName = "Varchar2(20)"), MaxLength(20)]
        public string CreateDate { get; set; }

        ///<summary>
        ///创建人ID
        ///</summary>
        [Column("lngOperatorID", TypeName = "Number(19)")]
        public long OperatorID { get; set; }

    }

    

}

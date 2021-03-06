# OracleEntityFrameworkCore

OracleEntityFrameworkCore是一个.Net Core的Oracle实体框架，支持11.2g及其以前版本和12c及其以后版本的Oracle数据库。修正了Oracle官方最新发布的OracleEntityFrameworkCore(2.19.60)存在的缺陷。对一些关键部分进行了完善，具体是：   
   
1、去掉Linq生成SQL时加上引号(Oracle对引号的对象大小写敏感)，一个稳定可靠的系统在对象名称(表,字段等等)中不应该包含空格，这不便于使用和管理，因此加上引号没有必要；   
2、对模型注解时类型映射进行了优化。   
3、支持原生SQL查询返回实体作为数据源，以方便用Linq在内存中查询数据，例如：
```
// 采用sql查询返回实体集合   
List<Rights> listRights = db.ExecuteQuery<Rights>("select * from Rights");
   
// 采用sql查询返回DataTable   
DataTable dt = db.ExecuteQuery("select lngRightsID, strRightsCode from Rights"); 
```

4、简化存储过程调用，例如：   
```
// 调用函数
var num = db.CallFunc<int>("ReturnNumberTest", 100);
var str = db.CallFunc<string>("ReturnStringTest", "");
var date = db.CallFunc<DateTime>("ReturnDateTest", DateTime.Now);

// 调用函数(带参数返回)
var paramList = new List<object[]>()
{
    new object[] {"In", 100 },
    new object[] {"Out", DateTime.Now },
    new object[] {"InOut", "hello" }
};
var valueList = new List<object>();
num = db.CallFunc<int>("OutParamFuncTest", paramList, valueList);

// 调用过程
var ok = db.CallProc("ProcTest", DateTime.Now);

// 调用过程(带参数返回)
paramList = new List<object[]>()
{
    new object[] {"In", 100 },
    new object[] {"Out", DateTime.Now },
    new object[] {"InOut", "hello" }
};
valueList = new List<object>();
ok = db.CallProc("OutParamProcTest", paramList, valueList);
```
  
关于表的定义和模型设计：   
   
根据以往的经验，在我们的系统中习惯于用匈牙利命名法来命名表的字段，   
也只用Oracle的number和varchar2,blob类型，不用或很少用其它类型，目的就是为了简单，   
例如对于日期字段，我们会这样设计：strDate  varchar2(20) default ' ' not null   
写入的时候必须要求格式化成yyyy-MM-dd hh:mm:ss的统一标准格式，例如：2019-01-01   
当然，见仁见智，各有所好，这只是我们认为非常好的一种设计方式而已。   
   
对于模型注解，建议您在设计表时这样设计：
   
```
Create Table MyTable   
(   
   blnValue1  number(1)       default 0   not null, -- bool(对应c#类型,下同)   
   bytValue2  number(3)       default 0   not null, -- byte   
   intValue3  number(10)      default 0   not null, -- int   
   lngValue4  number(19)      default 0   not null, -- long   
   dblValue5  number(29,9)    default 0   not null, -- double/decimal   
   strValue6  varchar2(20)    default ' ' not null  -- string   
)   
```    
然后这样定义模型：
  
```
[Table("MyTable")]   
public class MyEntity   
{     
    [Column("blnValue", TypeName = "number(1)")]    
    public bool Value1 { get; set; }    
       
    [Column("bytValue", TypeName = "number(3)")]   
    public byte Value2 { get; set; }   
       
    [Column("intValue", TypeName = "number(10)")]   
    public int Value3 { get; set; }   
       
    [Key, Column("lngValue", TypeName = "number(19)")]   
    public long Value4 { get; set; }   
       
    [Column("dblValue", TypeName = "number(29,9)")]   
    public double Value5 { get; set; } 或 public decimal Value5 { get; set; }    
       
    [Column("strValue", TypeName = "varchar2(20)"), MaxLength(20)]   
    public string Value6 { get; set; }   
}   
```
   
需要注意的是注解并不是必须的，例如： 
```
public class Right   
{   
   public long RightId { get; set; }   
   public string RightName { get; set; }   
   public string Description { get; set; }   
}
```
此时Right就是数据库表的一个直接实体。更多应用请参见测试示例。   

```补充说明```   
前面建议对于日期类型的字段建议设计成字符串varchar2(20) 。但在实践中会发现许多组件是根据数据类型进行校验绑定的，
例如DevExtreme组件根据日期类型绑定日期控件、JQuery校验根据日期类型进行等，此时如果将日期字段设计成字符串就不能满足需求，
但是便利同时也带来了问题。日期字段的最大问题是客户端日期格式和服务器日期格式一致性问题，这会给软件后期维护带来困境，这就是为什么将日期字段设计成字符串varchar2(20) 的原因。   

编译环境：   
Windows 10   
Visual Studio 2019   
 
QQ讨论群：```948127686```   

--权限表
Create Table Rights(
  lngRightsID          number(10)       default 0   not null,--权限ID
  strRightsCode        varchar2(100)    default ' ' not null,--权限编号 编号规则：Format(lngModuleID,3) + Format(序号,4)。例如：0010001
  strRightsName        varchar2(100)    default ' ' not null,--权限名称
  strDescription       varchar2(200)    default ' ' not null,--权限描述
  lngModuleID          number(10)       default 0   not null --所属模块ID 0,编码权限; >0,模块权限
);

--模块表
Create Table Modules(
  lngModuleID         number(10)       default 0   not null,--模块ID
  strModuleName       varchar2(100)    default ' ' not null,--模块名称
  strDescription      varchar2(200)    default ' ' not null,--模块描述
  blnIsInActive       number(1)        default 0   not null --封存标志
);


insert into rights(lngRightsID, strRightsCode, strRightsName, strDescription, lngModuleID )
select 1, '0010001', '增加', ' ', 1 from dual union
select 2, '0010002', '修改', ' ', 1 from dual union
select 3, '0010003', '删除', ' ', 1 from dual union
select 4, '0010004', '查询', ' ', 1 from dual;

insert into Modules(lngModuleID, strModuleName, strDescription, blnIsInActive)
select 1, '基础管理', ' ', 0 from dual
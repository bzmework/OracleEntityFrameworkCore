--Ȩ�ޱ�
Create Table Rights(
  lngRightsID          number(10)       default 0   not null,--Ȩ��ID
  strRightsCode        varchar2(100)    default ' ' not null,--Ȩ�ޱ�� ��Ź���Format(lngModuleID,3) + Format(���,4)�����磺0010001
  strRightsName        varchar2(100)    default ' ' not null,--Ȩ������
  strDescription       varchar2(200)    default ' ' not null,--Ȩ������
  lngModuleID          number(10)       default 0   not null --����ģ��ID 0,����Ȩ��; >0,ģ��Ȩ��
);

--ģ���
Create Table Modules(
  lngModuleID         number(10)       default 0   not null,--ģ��ID
  strModuleName       varchar2(100)    default ' ' not null,--ģ������
  strDescription      varchar2(200)    default ' ' not null,--ģ������
  blnIsInActive       number(1)        default 0   not null --����־
);


insert into rights(lngRightsID, strRightsCode, strRightsName, strDescription, lngModuleID )
select 1, '0010001', '����', ' ', 1 from dual union
select 2, '0010002', '�޸�', ' ', 1 from dual union
select 3, '0010003', 'ɾ��', ' ', 1 from dual union
select 4, '0010004', '��ѯ', ' ', 1 from dual;

insert into Modules(lngModuleID, strModuleName, strDescription, blnIsInActive)
select 1, '��������', ' ', 0 from dual
create or replace procedure OutParamProcTest(value1_in in number, value2_out out date, value3_in_out in out varchar2) IS

    i number := 100;

BEGIN

    value2_out := sysdate;
    value3_in_out := 'world';

EXCEPTION

   WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE ('error: ' || SQLERRM);

END OutParamProcTest;
create or replace function OutParamFuncTest(value1_in in number, value2_out out date, value3_in_out in out varchar2) RETURN number IS

    i number := 100;

BEGIN

    value2_out := sysdate;
    value3_in_out := 'world';
    return i;

EXCEPTION

   WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE ('error: ' || SQLERRM);

END OutParamFuncTest;
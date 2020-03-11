create or replace function ReturnStringTest(value_in in varchar2) RETURN varchar2 IS

    val varchar2(100) := 'world';

BEGIN

    return val;

EXCEPTION

   WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE ('error: ' || SQLERRM);

END ReturnStringTest;

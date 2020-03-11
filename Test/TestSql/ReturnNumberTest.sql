create or replace function ReturnNumberTest(value_in in number) RETURN number IS

    val number := 200;

BEGIN

    return val;

EXCEPTION

   WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE ('error: ' || SQLERRM);

END ReturnNumberTest;

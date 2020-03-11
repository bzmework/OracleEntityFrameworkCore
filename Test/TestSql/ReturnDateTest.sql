create or replace function ReturnDateTest(value_in in date) RETURN date IS

    val date := sysdate;

BEGIN

    return val;

EXCEPTION

   WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE ('error: ' || SQLERRM);

END ReturnDateTest;

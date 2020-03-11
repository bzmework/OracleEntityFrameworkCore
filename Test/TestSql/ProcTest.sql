create or replace procedure ProcTest(value_in in date) IS

    val date := sysdate;
    i number := 100;
    
BEGIN

    i := 200;

EXCEPTION

   WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE ('error: ' || SQLERRM);

END ProcTest;

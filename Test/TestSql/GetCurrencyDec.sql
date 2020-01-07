/*
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
@Module Name:  GetCurrencyDec.sql
@Main Func:    获得币种位数
@Author:       denglf
@Last Modify:  2017-12-31
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
lngCurrencyID_in，币种
*/
CREATE OR REPLACE FUNCTION GetCurrencyDec(lngCurrencyID_in IN NUMBER) RETURN NUMBER IS
  
    v_intCurDec NUMBER := 2;
    
BEGIN
    
    Select bytCurrencyDec Into v_intCurDec from Currencys Where lngCurrencyID = lngCurrencyID_in;
    Return v_intCurDec;
   
EXCEPTION

   WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE ('error: ' || SQLERRM);
      Return v_intCurDec;
      
END GetCurrencyDec;

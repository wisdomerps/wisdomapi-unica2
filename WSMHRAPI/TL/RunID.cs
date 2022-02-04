using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Microsoft.VisualBasic;

namespace WSMHRAPI.TL
{
    public class RunID
    {
        public static  int RunMailLenght = 13;
        public static  int RunLenght = 9;
        public static  string RunFmt = " Right(replace(Convert(varchar(10),Getdate(),111),'/',''),5)   ";

        public static int GetRunNoID(string TableName, string FieldName , WSM.Conn.DB.DataBaseName DbName, string CmpCode)
        {
            string _Qry = "";
            string RunNo = "";
            string _RunFmt = "";
            int IndChar = 0;
            string CmpFmt = ""; //00

            string _subCmpCode = "";

            //_subCmpCode = CmpCode.Substring(CmpCode.Length - 2, 2);


            if (CmpCode != "")
            {
                CmpCode = CmpCode.Substring(CmpCode.Length - 2, 2);
                char[] charArr = CmpCode.ToCharArray();
                foreach (char c in charArr)
                {
                    Console.WriteLine(c);

                    IndChar = IndChar + 1;

                    CmpFmt = CmpFmt + ((int)c).ToString();



                    if (IndChar >= 2)
                    {
                        break;
                    }
                }


                if ((CmpFmt!= "") &&  (CmpFmt.Length == 4 ))
                    {
                   CmpFmt = CmpFmt.Substring(CmpFmt.Length - 3, 3);
                    _RunFmt = " Left(Right(replace(Convert(varchar(10),Getdate(),111),'/',''),6),2) +  Right('0000'+ Convert(varchar(4),(Convert(int," + CmpFmt + ") +  Convert(int,Right(replace(Convert(varchar(10),Getdate(),111),'/',''),4)))),3)   ";
         
                }


            }
            else
            {
                _RunFmt = RunFmt;
            }


            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

            _Qry = @" SELECT  ISNULL((";
            _Qry += Constants.vbCrLf + " SELECT TOP 1  Convert(varchar(" + RunLenght + ")," + FieldName + " +1)  AS FNRunNo ";
            _Qry += Constants.vbCrLf + " FROM  " + TableName + "  WITH(NOLOCK) ";
            _Qry += Constants.vbCrLf + " WHERE  LEN(" + FieldName + ") =" + RunLenght + "";
            _Qry += Constants.vbCrLf + " AND LEFT(" + FieldName + ",5)= " + _RunFmt;
            _Qry += Constants.vbCrLf + "  ORDER BY " + FieldName + "  DESC) ," + _RunFmt + " + '0001') AS FNRunNo";


            //_Qry = @" SELECT  ISNULL((";
            //_Qry += Constants.vbCrLf + " SELECT TOP 1  Convert(varchar(" + CmpFmt.Length + RunLenght + ")," + FieldName + " +1)  AS FNRunNo ";
            //_Qry += Constants.vbCrLf + " FROM  " + TableName + "  WITH(NOLOCK) ";
            //_Qry += Constants.vbCrLf + " WHERE  LEN(" + FieldName + ") =" + CmpFmt.Length +"+"+ RunLenght + "";
            //_Qry += Constants.vbCrLf + " AND LEFT(" + FieldName + ",6 + " + CmpFmt.Length  + ")='"+ CmpFmt  + "'+" + _RunFmt;
            //_Qry += Constants.vbCrLf + "  ORDER BY " + FieldName + "  DESC) ,'" + CmpFmt + "'+"+ _RunFmt + " + '0001') AS FNRunNo";



            RunNo = Cnn.GetField(_Qry, DbName, "0");


            return int.Parse(RunNo);
        }
    }
}
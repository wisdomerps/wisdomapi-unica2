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
        public static  int RunLenght = 10;
        public static  string RunFmt = " Right(replace(Convert(varchar(10),Getdate(),111),'/',''),6)   ";

        public static int GetRunNoID(string TableName, string FieldName , WSM.Conn.DB.DataBaseName DbName, string CmpCode)
        {
            string _Qry = "";
            string RunNo = "";
            string _RunFmt = "";
            string IndChar = "";
            string CmpFmt = "";

            string _subCmpCode = "";
            _subCmpCode = CmpCode.Substring(CmpCode.Length - 2, 2);


            //If CmpFmt<> "" And CmpFmt.Length = 4 Then
            //  CmpFmt = Right(CmpFmt, 3)
            //    _RunFmt = " Left(Right(replace(Convert(varchar(10),Getdate(),111),'/',''),6),2) +  Right('0000'+ Convert(varchar(4),(Convert(int," & CmpFmt & ") +  Convert(int,Right(replace(Convert(varchar(10),Getdate(),111),'/',''),4)))),4)   "
            //End If

            _RunFmt = RunFmt;

            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

            _Qry = @" SELECT  ISNULL((";
            _Qry += Constants.vbCrLf + " SELECT TOP 1  Convert(varchar(" + RunLenght + ")," + FieldName + " +1)  AS FNRunNo ";
            _Qry += Constants.vbCrLf + " FROM  " + TableName + "  WITH(NOLOCK) ";
            _Qry += Constants.vbCrLf + " WHERE  LEN(" + FieldName + ") =" + RunLenght + "";
            _Qry += Constants.vbCrLf + " AND LEFT(" + FieldName + ",6)= " + _RunFmt;
            _Qry += Constants.vbCrLf + "  ORDER BY " + FieldName + "  DESC) ," + _RunFmt + " + '0001') AS FNRunNo";

            RunNo = Cnn.GetField(_Qry, WSM.Conn.DB.DataBaseName.DB_HR, "0");


            return int.Parse(RunNo);
        }
    }
}
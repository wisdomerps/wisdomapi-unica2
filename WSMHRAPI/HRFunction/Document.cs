using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.HRFunction
{
    public class Document
    {
        public static string GetDocumentNo(String _DBName, String _TblName , String _DocType, bool _GetFotmat = false, String AddPrefix = "", String DocumentDate  = "")
        {
            string _no = "";
            string _Qrysql = "";
            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

            _Qrysql = " EXEC [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_SYSTEM) + "].dbo.SP_GEN_DOCUMENTNO '" + _DBName + "','" + _TblName + "','" + _DocType + "','" + _GetFotmat + "','" + AddPrefix + "','" + DocumentDate + "'";
            _no = Cnn.GetField(_Qrysql, WSM.Conn.DB.DataBaseName.DB_SYSTEM, "");

            return _no;
        }

    //    Public Shared Function GetDocumentNo(ByVal _DBName As String, ByVal _TblName As String, ByVal _DocType As String, Optional _GetFotmat As Boolean = False, Optional AddPrefix As String = "", Optional DocumentDate As String = "") As String

    //    Dim _Qrysql As String
    //    _Qrysql = " EXEC [" & HI.Conn.DB.GetDataBaseName(Conn.DB.DataBaseName.DB_SYSTEM) & "].dbo.SP_GEN_DOCUMENTNO '" & _DBName & "','" & _TblName & "','" & _DocType & "','" & IIf(_GetFotmat, "Y", "") & "','" & AddPrefix & "','" & HI.UL.ULDate.ConvertEnDB(DocumentDate) & "'"
    //    Return HI.Conn.SQLConn.GetField(_Qrysql, Conn.DB.DataBaseName.DB_SYSTEM, "")

    //End Function

    }
}
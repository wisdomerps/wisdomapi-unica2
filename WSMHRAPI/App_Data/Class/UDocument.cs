using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMERPAPI
{
    public static  class UDocument
    {
        public static string GetDocumentNo(string _DBName,string _TblName,string _DocType,bool _GetFotmat =false,string AddPrefix ="",string DocumentDate="") {
            string docno = "";
            string cmdstring = "";

            cmdstring = " EXEC [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_SYSTEM) + "].dbo.SP_GEN_DOCUMENTNO '" + _DBName + "','" + _TblName + "','" + _DocType + "','" + (_GetFotmat ? "Y" : "") + "','" + AddPrefix + "','" + UFuncs.ConvertEnDB(DocumentDate) + "'";

            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();
            docno = Cnn.GetField(cmdstring, WSM.Conn.DB.DataBaseName.DB_SYSTEM, "");

            return docno;
        }

    }
}
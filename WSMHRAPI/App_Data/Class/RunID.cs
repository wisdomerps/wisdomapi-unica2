

using System;

namespace WSMERPAPI
    {  static class Extensions
        {
            /// <summary>
            /// Get substring of specified number of characters on the right.
            /// </summary>
            public static string Right(this string value, int length)
            {
                return value.Substring(value.Length - length);
            }
        }
    public static class RunID
    {
        //    Private Shared RunMailLenght As Integer = 13;
        //Private Shared RunLenght As Integer = 10
        //Private Shared RunFmt As String = " Right(replace(Convert(varchar(10),Getdate(),111),'/',''),6)   "

      

        public static int GetRunNoID(String TableName, String FieldName, WSM.Conn.DB.DataBaseName DbName, int cmpID = 0) 
        {

            int RunLenght = 9;
            String RunFmt = " Right(replace(Convert(varchar(10),Getdate(),111),'/',''),5)   ";


            String _Qry  = "";
            String RunNo = "";
            String _RunFmt = "";
            int IndChar = 0;
            String CmpFmt = "";
            WSM.Conn.SQLConn Cnn = new WSM.Conn.SQLConn();

            String FTCmpCode = "";

            if (cmpID != 0)
            {
                _Qry = "SELECT FTCmpCode FROM [" + WSM.Conn.DB.GetDataBaseName(WSM.Conn.DB.DataBaseName.DB_HR) + "].[dbo].[TCNMCmp] WHERE FNHSysCmpId = " + cmpID;

                FTCmpCode = Cnn.GetField(_Qry, DbName, 0);

                char[] ch = new char[FTCmpCode.Length];

                FTCmpCode = FTCmpCode.Substring((FTCmpCode.Length - 2), (FTCmpCode.Length));

                // Copy character by character into array  
                for (int i = 0; i < FTCmpCode.Length; i++)
                {
                    ch[i] = FTCmpCode[i];
                }

                // Printing content of array  
                foreach (char c in ch)
                {

                    CmpFmt = CmpFmt + (Convert.ToInt32(c)).ToString();
                   
                }
                if (CmpFmt != "" && CmpFmt.Length == 4)
                {
                    CmpFmt = CmpFmt.Right(3);
                    _RunFmt = " Left(Right(replace(Convert(varchar(10),Getdate(),111),'/',''),6),2) +  Right('0000'+ Convert(varchar(4),(Convert(int," + CmpFmt + ") +  Convert(int,Right(replace(Convert(varchar(10),Getdate(),111),'/',''),4)))),3)   ";


                }
            }
            else
            {
                _RunFmt = RunFmt;
            }

            _RunFmt = " Left(Right(replace(Convert(varchar(10),Getdate(),111),'/',''),6),2) +  Right('0000'+ Convert(varchar(4),(Convert(int," + CmpFmt + ") +  Convert(int,Right(replace(Convert(varchar(10),Getdate(),111),'/',''),4)))),3)   ";

            _Qry = @" SELECT  ISNULL(( ";
            _Qry += "SELECT TOP 1  Convert(varchar(" + RunLenght + ")," + FieldName + " +1)  AS FNRunNo ";
            _Qry += " FROM  " + TableName + "  WITH(NOLOCK) ";
            _Qry += " WHERE  LEN(" + FieldName +  ") =" + RunLenght + "";
            _Qry += " AND LEFT(" + FieldName + ",5)= " + _RunFmt;
            _Qry += " ORDER BY " + FieldName + "  DESC) ," +  _RunFmt + " + '0001') AS FNRunNo";



           
            RunNo = Cnn.GetField(_Qry, DbName, 0);



            if (int.Parse(RunNo) > 0)
            {
                return int.Parse(RunNo);
            }
            else
            {
                return 0;
            }
           
        }
      
    }
}
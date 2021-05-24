using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Text;

namespace WSM.Conn
{
    public static class DB

    {

        public static string _ConnectString;


        public static string[] DBName = new string[30];

        public static string[] SystemDBName = { "DB_TEMPDB", "DB_SECURITY", "DB_HR", "DB_SYSTEM", "DB_MASTER", "DB_MER", "DB_PUR", "DB_INVEN", "DB_PROD", "DB_ACCOUNT", "DB_LANG", "DB_LOG", "DB_MAIL", "DB_HR_PAYROLL", "DB_MEDC", "DB_FG", "DB_PLANNING", "DB_DOC", "DB_FIXED", "DB_SAMPLE" };
        public enum DataBaseName : int
        {

            DB_TEMPDB = 0,
            DB_SECURITY = 1,
            DB_HR = 2,
            DB_SYSTEM = 3,
            DB_MASTER = 4,
            DB_MERCHAN = 5,
            DB_PUR = 6,
            DB_INVEN = 7,
            DB_PROD = 8,
            DB_ACCOUNT = 9,
            DB_LANG = 10,
            DB_LOG = 11,
            DB_MAIL = 12,
            DB_HR_PAYROLL = 13,
            DB_MEDC = 14,
            DB_FG = 15,
            DB_PLANNING = 16,
            DB_DOC = 17,
            DB_FIXED = 18,
            DB_SAMPLE = 19
        }

        public static string GetDataBaseName(DataBaseName DbName)
        {
            try
            {
                return DBName[(int)DbName];
            }
            catch
            {
                return "";
            }
        }

        public static string GetServerName()
        {
            try
            {
                return WSM.Conn.DB.ServerName;
            }
            catch
            {
                return "";
            }
        }

        public static string GetDataBaseName(string DbName)
        {
            try
            {
                int I = 0;
                foreach (string StrDBName in DBName)
                {

                    if (StrDBName.ToUpper().Contains(DbName.ToUpper()) == true)
                    {
                        return DBName[I];

                    }
                    I = I + 1;
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
        public static bool UsedDB(DataBaseName DbName)
        {
            try
            {
       
                BaseName = DBName[(int)DbName];
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool UsedDB(string DbName)
        {
            try
            {
                int I = 0;
                foreach (string StrDBName in DBName)
                {
                    if (StrDBName.ToUpper().Contains(DbName.ToUpper()) == true)

                    {
                      
                        BaseName = DBName[I];
                        return true;
                        break;
                    }
                    I = I + 1;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string CmpID { get; set; }
        public static string ServerName { get; set; }
        public static string UserName { get; set; }
        public static string UserPassword { get; set; }
        public static string BaseName { get; set; }
        public static string AppService { get; set; }
        public static string AppServicePath { get; set; }
        public static string AppServiceName { get; set; }

        public static string ConnectionString(DataBaseName DbName)
        {

            try
            {
                _ConnectString = "SERVER=" + WSM.Conn.DB.ServerName + ";UID=" + UserName + ";PWD=" + UserPassword + ";Initial Catalog=" + DBName[(int)DbName];
                return _ConnectString;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        #region " Function Decrypt FuncEncryptData"
        public static string FuncDecryptData(string DecryTxt, bool FirtsTime = true)
        {

            string txtDecry = "";
            try
            {
                char Buff1 = '\0';
                char Buff2 = '\0';
                string TxtBuff1 = "";
                string TxtBuff2 = "";
                int i = 0;
                int DecryCode = 0;


                if (!string.IsNullOrEmpty(Strings.Trim(DecryTxt)))
                {
                    DecryCode = Strings.Asc(Strings.Right(DecryTxt, 1)) - Strings.Asc(Strings.Mid(DecryTxt, 2, 1));

                    for (i = Strings.Len(DecryTxt) - 1; i >= 1; i += -1)
                    {
                        TxtBuff1 += Strings.Mid(DecryTxt, i, 1);
                    }

                    for (i = 1; i <= Strings.Len(TxtBuff1); i++)
                    {
                        Buff1 = Convert.ToChar(Strings.Mid(TxtBuff1, i, 1));
                        Buff2 = '\0';
                        Buff2 = Strings.Chr(Strings.Asc(Buff1) - DecryCode);
                        TxtBuff2 += Buff2;
                    }
                    txtDecry = TxtBuff2;
                }

                if ((FirtsTime))
                {
                    if (txtDecry.Length > 1)
                    {
                        txtDecry = FuncDecryptData(Strings.Right(txtDecry, txtDecry.Length - 1), false);
                    }
                    else
                    {
                        txtDecry = FuncDecryptData(txtDecry, false);
                    }

                }

            }
            catch (Exception ex)
            {
            }
            return txtDecry;
        }

        public static string FuncEncryptData(string EncryTxt, bool FirtsTime = true)
        {
            string txtEncry = "";
            try
            {
                int EncryCode = 0;
                char Buff1 = '\0';
                char Buff2 = '\0';
                string TxtBuff1 = "";
                string TxtBuff2 = "";
                int i = 0;
                VBMath.Randomize();
                EncryCode = Convert.ToInt32((9 * VBMath.Rnd()) + 1);


                for (i = 1; i <= Strings.Len(EncryTxt); i++)
                {
                    Buff1 = Convert.ToChar(Strings.Mid(EncryTxt, i, 1));
                    Buff2 = '\0';
                    Buff2 = Strings.Chr(Strings.Asc(Buff1) + EncryCode);
                    TxtBuff1 += Buff2;

                }

                for (i = Strings.Len(TxtBuff1); i >= 1; i += -1)
                {
                    TxtBuff2 += Strings.Mid(TxtBuff1, i, 1);
                }

                EncryCode = Strings.Asc(Strings.Mid(TxtBuff2, 2, 1)) + EncryCode;
                txtEncry = TxtBuff2 + Strings.Chr(EncryCode);

                if ((FirtsTime))
                {
                    if (txtEncry.Length > 1)
                    {
                        txtEncry = FuncEncryptData("H" + txtEncry, false);
                    }
                    else
                    {
                        txtEncry = FuncEncryptData(txtEncry, false);
                    }

                }

            }
            catch (Exception ex)
            {
            }
            return txtEncry;
        }
        #endregion

        #region " Function Decrypt FuncEncryptData Server"

        public static string FuncDecryptDataServer(string DecryTxt)
        {

            string txtDecry = "";
            try
            {
                char Buff1 = '\0';
                char Buff2 = '\0';
                string TxtBuff1 = "";
                string TxtBuff2 = "";
                int i = 0;
                int DecryCode = 0;
                //Microsoft.VisualBasic.Strings.As
                if (!string.IsNullOrEmpty(DecryTxt.Trim()))
                {
                    DecryCode = Strings.Asc(Strings.Right(DecryTxt, 1)) - Strings.Asc(Strings.Mid(DecryTxt, 2, 1));

                    for (i = DecryTxt.Length - 1; i >= 1; i += -1)
                    {
                        TxtBuff1 += Strings.Mid(DecryTxt, i, 1);
                    }

                    for (i = 1; i <= Strings.Len(TxtBuff1); i++)
                    {
                        Buff1 = Convert.ToChar(Strings.Mid(TxtBuff1, i, 1));
                        Buff2 = '\0';
                        Buff2 = Strings.Chr(Strings.Asc(Buff1) - DecryCode);
                        TxtBuff2 += Buff2;
                    }

                    txtDecry = TxtBuff2;
                    txtDecry = Strings.Right(txtDecry, txtDecry.Length - 1);
                    txtDecry = Strings.Left(txtDecry, txtDecry.Length - 1);


                    int txtlangth = 0;
                    int txtsplit1 = 0;

                    try
                    {
                        txtlangth = int.Parse((txtDecry.Split('|'))[0]);
                        txtsplit1 = int.Parse((txtDecry.Split('|'))[1]);
                        txtDecry = ((txtDecry.Split('|'))[2]);
                    }
                    catch { }


                    if (txtlangth > 0)
                    {
                        txtDecry = Strings.Right(txtDecry, txtlangth - txtsplit1) + Strings.Left(txtDecry, txtsplit1);

                    };

                }

            }
            catch (Exception ex)
            {
            }
            return txtDecry;
        }

        public static string FuncEncryptDataServer(string EncryTxt)
        {
            string txtEncry = "";
            try
            {
                int EncryCode = 0;
                char Buff1 = '\0';
                char Buff2 = '\0';
                string TxtBuff1 = "";
                string TxtBuff2 = "";
                int i = 0;
                int txtlangth = 0;
                int txtsplit1 = 0;

                txtlangth = EncryTxt.Length;
                txtsplit1 = txtlangth / 2;

                if (txtlangth <= 0)
                {
                    txtlangth = 0;
                    txtsplit1 = 0;
                };

                EncryTxt = txtlangth.ToString() + "|" + txtsplit1.ToString() + "|" + Strings.Right(EncryTxt, txtsplit1) + Strings.Left(EncryTxt, txtlangth - txtsplit1);

                VBMath.Randomize();
                EncryTxt = "L" + EncryTxt + "H";

                EncryCode = Convert.ToInt32((9 * VBMath.Rnd()) + 1);


                for (i = 1; i <= Strings.Len(EncryTxt); i++)
                {
                    Buff1 = Convert.ToChar(Strings.Mid(EncryTxt, i, 1));
                    Buff2 = '\0';
                    Buff2 = Strings.Chr(Strings.Asc(Buff1) + EncryCode);
                    TxtBuff1 += Buff2;

                }

                for (i = Strings.Len(TxtBuff1); i >= 1; i += -1)
                {
                    TxtBuff2 += Strings.Mid(TxtBuff1, i, 1);
                }

                EncryCode = Strings.Asc(Strings.Mid(TxtBuff2, 2, 1)) + EncryCode;
                txtEncry = TxtBuff2 + Strings.Chr(EncryCode);



            }
            catch (Exception ex)
            {
            }
            return txtEncry;
        }

        #endregion
    }
}

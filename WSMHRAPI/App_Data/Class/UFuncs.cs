using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Drawing;

namespace WSMERPAPI
{
    public static class UFuncs
    {

        public enum eLang : int
        {
            EN = 1,
            TH = 2,
            VT = 3,
            KM = 4,
            BM = 5,
            LAO = 6,
            CH = 7
        }
        public static eLang Language { get; set; }

        public const string FormatDateDB = "Convert(varchar(10),Getdate(),111)";
        public const string FormatTimeDB = "Convert(varchar(8),Getdate(),114)";
        public static string rpQuoted(string tmpStr)
        {

            string DataStr = tmpStr.Trim();

            if (!string.IsNullOrEmpty(tmpStr))
            {
                return Strings.Replace(DataStr, (Strings.Chr(39)).ToString(), (Strings.Chr(39)).ToString() + (Strings.Chr(39)).ToString());
            }
            else
            {
                return DataStr;
            }

        }

        public static string CheckDate(object Obj)
        {
            try
            {
                System.Globalization.CultureInfo _Culture = new System.Globalization.CultureInfo("en-US", true);
                _Culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                _Culture.DateTimeFormat.ShortTimePattern = "HH:mm:ss";

                System.Threading.Thread.CurrentThread.CurrentCulture = _Culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = _Culture;



                string _Date = "";
                _Date = Strings.Format(Convert.ToDateTime(Obj), "dd/MM/yyyy");

                return _Date;
            }
            catch //(Exception ex)
            {
                return "";
            }
        }
        public static string ConvertEnDB(object DataDate)
        {
            string strDate = "";

            try
            {
                strDate = CheckDate(DataDate);

                if ((Convert.ToInt32(Strings.Mid(strDate, 7, 4)) > 0) & (Convert.ToInt32(Strings.Mid(strDate, 7, 4)) > (2300)))
                {
                    strDate = (Strings.Mid(strDate, 1, 2)) + "/" + (Strings.Mid(strDate, 4, 2)) + "/" + (Convert.ToInt32(Strings.Mid(strDate, 7, 4)) - 543).ToString("0000");
                }
                else
                {
                    strDate = (Strings.Mid(strDate, 1, 2)) + "/" + (Strings.Mid(strDate, 4, 2)) + "/" + (Strings.Mid(strDate, 7, 4));
                }

                strDate = Strings.Mid(strDate, 7, 4) + "/" + Strings.Mid(strDate, 4, 2) + "/" + Strings.Mid(strDate, 1, 2);
            }
            catch //(Exception ex)
            {
                strDate = "";
            }


            return strDate;

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSM.Conn
{
    public class commandstring
    {
        private string cmdstring = "";

        public commandstring() {

            cmdstring = "";
        }

        public void clear() {
            cmdstring = "";
        }

        public string getcommand()
        {
           return  cmdstring ;
        }

        public void Add(string command)
        {
            cmdstring += Microsoft.VisualBasic.Constants.vbCrLf + " " + command;
        }

    }
}
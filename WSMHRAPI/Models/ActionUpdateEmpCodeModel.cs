﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class ActionUpdateEmpCodeModel
    {
        public bool Status { get; set; }
        public int FNHSysEmpID { get; set; }
        public string FTEmpCode { get; set; }

        public int StatusCode { get; set; }
        public string Messege { get; set; }

    }
}
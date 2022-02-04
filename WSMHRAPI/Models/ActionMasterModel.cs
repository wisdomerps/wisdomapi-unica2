using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class ActionMasterModel
    {
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string Messege { get; set; }
        public int FNHSysMasterID { get; set; }

    }
}
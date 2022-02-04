using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class MasterCompanyModel
    {
        public string username { get; set; }
        public int FNHSysCmpId { get; set; }

        public string FTCmpCode { get; set; }

        public string FTCmpNameTH { get; set; }
        public string FTCmpNameEN { get; set; }
        public string FTRemark { get; set; }

        public string FTStateActive { get; set; }
    }
}
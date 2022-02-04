using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class MasterCLeavelModel
    {
        public string username { get; set; }
        public int FNHSysCLevelId { get; set; }
        
        public string FTCLevelCode { get; set; }

        public string FTCLevelNameTH { get; set; }
        public string FTCLevelNameEN { get; set; }
        public string FTRemark { get; set; }

        public string FTStateActive { get; set; }

    }
}
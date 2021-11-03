using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class MasterSectModel
    {
        public string username { get; set; }
        public int FNHSysSectId { get; set; }
        public string FTSectCode { get; set; }
        public string FTSectNameTH { get; set; }
        public string FTSectNameEN { get; set; }
        public string FTRemark { get; set; }

        public int FNHSysCmpId { get; set; }

    }
}
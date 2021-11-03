using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class MasterUnitSectModel
    {
        public string username { get; set; }
        public int FNHSysUnitSectId { get; set; }

        public string FTUnitSectCode { get; set; }

        public string FTUnitSectNameTH { get; set; }
        public string FTUnitSectNameEN { get; set; }
        public string FTRemark { get; set; }

        public int FNHSysCmpId { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class MasterDivisionModel
    {
        public string username { get; set; }
        public int FNHSysDivisonId { get; set; }

        public string FTDivisonCode { get; set; }

        public string FTDivisonNameTH { get; set; }
        public string FTDivisonNameEN { get; set; }
        public string FTRemark { get; set; }

        public int FNHSysCmpId { get; set; }
        public string FTStateActive { get; set; }
    }
}
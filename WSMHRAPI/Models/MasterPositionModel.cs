using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class MasterPositionModel
    {
        public string username { get; set; }
        public int FNHSysPositId { get; set; }

        public string FTPositCode { get; set; }

        public string FTPositNameTH { get; set; }

        public string FTPositNameEN { get; set; }

        public string FTRemark { get; set; }

        public int FNHSysCmpId { get; set; }

        public int FNEmployeeFormatType { get; set; }

        public string FTStateActive { get; set; }

    }
}
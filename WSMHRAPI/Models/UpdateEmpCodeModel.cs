using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class UpdateEmpCodeModel
    {
        public string username { get; set; }
        public int FNHSysCmpId { get; set; }
        public int FNHSysEmpID { get; set; }
        public int FNHSysEmpTypeId { get; set; }
        
        public string FTEmpCode { get; set; }

        public string FDDateStart { get; set; }

    }
}
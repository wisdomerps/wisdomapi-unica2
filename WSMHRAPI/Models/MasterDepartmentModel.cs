using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class MasterDepartmentModel
    {
        public string username { get; set; }
        public int FNHSysDeptId { get; set; }

        public string FTDeptCode { get; set; }

        public string FTDeptDescTH { get; set; }
        public string FTDeptDescEN { get; set; }
        public string FTRemark { get; set; }

        public int FNHSysCmpId { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class LeaveModel
    {
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

    }
}
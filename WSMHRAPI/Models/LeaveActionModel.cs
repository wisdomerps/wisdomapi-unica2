using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class LeaveActionModel
    {
        public int EmployeeId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int LeaveTypeId { get; set; }
        
        public int ApproverId { get; set; }
        public string FTStateType { get; set; }
        public int ActionType { get; set; }
    }
}
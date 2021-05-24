using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class CreateLeaveRequestModel
    {
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
        public int WorkShiftId { get; set; }
        public int LeaveTypeId { get; set; }

        public int LeaveDayState { get; set; }
        
        public int LeaveMethod { get; set; }
        public int LeaveMinutes { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Remark { get; set; }

        public string FileBase64 { get; set; }
        public string ExtentionFile { get; set; }

    }
}
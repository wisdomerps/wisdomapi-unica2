using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSM
{
    public class RefreshNikeTokenResultJSON
    {
        public string access_token;
        public string token_type;
        public string expires_in;
        public string scope;
    }


    public class RefreshUserInfoJSON
    {
        public string username;
        public string password;
        public string cmpcode;
    }

    public class JSonHeaderDocument
    {
        public string HSysCmpId { get; set; }
        public string DocumentNo { get; set; }
        public string WHCode { get; set; }
        public string WHLocCode { get; set; }
        public string Note { get; set; }
        public string UserName { get; set; }
    }

    public class RefreshReportJSON
    {
        public bool ExportState;
        public string Message;
        public string Report;
    }

    public class UserDataInfo
    {
        public bool Authentication { get; set; }
        public bool Authenusername { get; set; }
        public bool Authenpassword { get; set; }
        public int EmpId { get; set; }
        public List<UserDataCmpInfo> CmpIdInfo { get; set; }
    }


    public class UserDataCmpInfo {
        public int CmpId { get; set; }
        public string CmpCode { get; set; }

    }


    public class UserRegisterInfo
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string EmpSurName { get; set; }
        public string EmpIdCard { get; set; }
        public string EmpPhone { get; set; }
        public string EmpBirthday { get; set; }

    }

    public class MasterUnisect
    {
        public int FNHSysUnitSectId { get; set; }
        public string FTUnitSectCode { get; set; }
        public string FTUnitSectNameTH { get; set; }
        public string FTUnitSectNameEN { get; set; }
        public string FTStateProd { get; set; }
        public string FTRemark { get; set; }
        public string FTStateActive { get; set; }
        public string FTStateCut { get; set; }
        public string FTStateSew { get; set; }

        public string FTUserName { get; set; }
        public string FNIncentiveType { get; set; }
        public string FNHSysEmpID { get; set; }
        public string FNHSysUnitSectIdTo { get; set; }
        public string FTStateStockFabric { get; set; }
        public string FTStateStockAccessory { get; set; }
        public string FTStateCutAuto { get; set; }

        public string FTStateHeatTransfer { get; set; }
        public string FTStateEmpPrint { get; set; }
        public string FTStatePadPrint { get; set; }
        public string FTStateLaser { get; set; }
        public string FTStateEmbroidery { get; set; }
        public string FTStateQC { get; set; }

        public string FTStateMachanic { get; set; }
        public string FNHSysIncenFormulaId { get; set; }
        public string FTStateRelease { get; set; }
        public string FTStateSampleRoom { get; set; }
        public string FNHSysCmpId { get; set; }
        public string FNSeq { get; set; }
    }

}
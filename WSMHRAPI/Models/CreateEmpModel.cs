using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSMHRAPI.Models
{
    public class CreateEmpModel  //class main
    {
        public string username { get; set; }
        public int EmployeeId { get; set; }
        public int FNHSysCmpId { get; set; }
        public int FNHSysEmpTypeId { get; set; }
        public int FNHSysPreNameId { get; set; }
        public string FTEmpCode { get; set; }
        public string FTEmpNameTH { get; set; }
        public string FTEmpSurnameTH { get; set; }
        public string FTEmpNicknameTH { get; set; }
        public string FTEmpNameEN { get; set; }
        public string FTEmpSurnameEN { get; set; }
        public string FTEmpNicknameEN { get; set; }

        public int FNEmpSex { get; set; }
        public string FDDateStart { get; set; }
        
        public string FCWeight { get; set; }
        public string FCHeight { get; set; }
        public string FDBirthDate { get; set; }
        public string FTEmpIdNo { get; set; }
        public string FTEmpIdNoBy { get; set; }

        public string FDDateIdNoAssign { get; set; }
        public string FDDateIdNoEnd { get; set; }

        public string FTAddrNo { get; set; }
        public string FTAddrMoo { get; set; }
        public string FTAddrSoi { get; set; }
        public string FTAddrRoad { get; set; }
        public string FTAddrTumbol { get; set; }
        public string FTAddrAmphur { get; set; }
        public string FTAddrProvince { get; set; }
        public string FTAddrPostCode { get; set; }
        public string FTAddrTel { get; set; }

        public string FTAddrNo1 { get; set; }
        public string FTAddrMoo1 { get; set; }
        public string FTAddrSoi1 { get; set; }
        public string FTAddrRoad1 { get; set; }
        public string FTAddrTumbol1 { get; set; }
        public string FTAddrAmphur1 { get; set; }
        public string FTAddrProvince1 { get; set; }
        public string FTAddrPostCode1 { get; set; }
        public string FTAddrTel1 { get; set; }

        public string FTFatherName { get; set; }
        public string FTFatherCareer { get; set; }
        public string FNFatherLife { get; set; }

        public string FTMotherName { get; set; }
        public string FTMotherCareer { get; set; }
        public string FNMotherLife { get; set; }

        public string FTTaxNo { get; set; }
        public string FTSocialNo { get; set; }

        public int FNHSysNationalityId { get; set; }
        public int FNHSysRaceId { get; set; }
        public int FNHSysReligionId { get; set; }
        public int FNHSysBldId { get; set; }
      
        public int FNMilitary { get; set; }



        public string FNMaritalStatus { get; set; }
        public string FTMateName { get; set; }
        public string FTMateCareer { get; set; }
        public string FTMateAddrWork { get; set; }

        public int FNHSysShiftID { get; set; }

        public int FNHSysPositId { get; set; }
        public int FNHSysDivisonId { get; set; }
        public int FNHSysDeptId { get; set; }
        public int FNHSysSectId { get; set; }
        public int FNHSysUnitSectId { get; set; }

        public int FNHSysPositIdOrg { get; set; }
        public int FNHSysCLevelIdOrg { get; set; }
        public int FNHSysCountryIdOrg { get; set; }
        public int FNHSysCmpIdOrg { get; set; }
        public int FNHSysDivisonIdOrg { get; set; }
        public int FNHSysDeptIdOrg { get; set; }
        public int FNHSysSectIdOrg { get; set; }
        public int FNHSysUnitSectIdOrg { get; set; }



        public CreateEmpEducation createEmpEducation { get;set;}  // 1:1 
        public List<CreateEmpEducation> createEmpEducations { get; set; } // 1:M


        public CreateEmpChild CreateEmpChild { get; set; }  // 1:1 
        public List<CreateEmpChild> CreateEmpChilds { get; set; } // 1:M

    }

    public class CreateEmpEducation   //class c
    {
        public int SeqId { get; set; }
        public int EmployeeId { get; set; }
        public int FNHSysCourseId { get; set; }

    }
    public class CreateEmpChild  //class c
    {
        public int EmployeeId { get; set; }
        public int FNSeqNo { get; set; }
        public string FTChildName { get; set; }

        public string FDChildBirthDate { get; set; }
        public string FTChildSex { get; set; }
        public string FTStudySta { get; set; }

        public string FTStateNotDisTax { get; set; }
    }

    }
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ASS2_20240802.Models
{
    public class DoctorPatientMapping
    {
        public int Id { get; set; } // 主键
        public string DoctorId { get; set; } // 医生ID
        public string UserId { get; set; } // 病人ID

        public virtual ASS2_20240802.Models.ApplicationUser User { get; set; }
    }
}




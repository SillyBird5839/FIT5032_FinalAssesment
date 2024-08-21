namespace ASS2_20240802.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    public partial class HealthCareAppointments
    {
        public int Id { get; set; }

        public int PatientID { get; set; }

        public int DoctorID { get; set; }

        [Required]
        public string Date { get; set; }
    }

}

namespace ASS2_20240802.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserActionLogs1 : DbMigration
    {
        public override void Up()
        {/*
            CreateTable(
                "dbo.HealthCareAppointments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                        DoctorID = c.Int(nullable: false),
                        Date = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            */
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HealthCareAppointments");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using ASS2_20240802.Models;


namespace ASS2_20240802.Controllers
{
    public class HealthCareAppointmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HealthCareAppointments
        public ActionResult Index()
        {
            return View(db.HealthCareAppointments.ToList());
        }

        // GET: HealthCareAppointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HealthCareAppointments healthCareAppointments = db.HealthCareAppointments.Find(id);
            if (healthCareAppointments == null)
            {
                return HttpNotFound();
            }
            return View(healthCareAppointments);
        }

        // GET: HealthCareAppointments/Create
        public ActionResult Create()
        {
            return View();
        }


        /*
        // POST: HealthCareAppointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PatientID,DoctorID,Date")] HealthCareAppointments healthCareAppointments)
        {
            if (ModelState.IsValid)
            {
                db.HealthCareAppointments.Add(healthCareAppointments);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(healthCareAppointments);
        }
        */




        [HttpPost]
        public String CreateAppointment()
        {
            var appointment = new HealthCareAppointments
            {
                PatientID = int.Parse(Request.Form["PatientID"]),
                DoctorID = int.Parse(Request.Form["DoctorID"]),
                Date = Request.Form["AppointmentDate"]
            };

            var vx = Request.Files["Attachment"].ContentLength;


            // Store the attachment in local storage.
            var Str1 = Request.Files[0].FileName.Split('.');
            var FileType = Str1[Str1.Length - 1];
            var FilePath =
                Server.MapPath("~/Uploads/") +
                string.Format(@"{0}", Guid.NewGuid()) +
                "." + FileType;
            Request.Files[0].SaveAs(FilePath);


            if (ModelState.IsValid)
            {
                // Add the appointment into the database.
                db.HealthCareAppointments.Add(appointment);
                db.SaveChanges();

                // Send confirmation email.
                var mail = new MailMessage();
                mail.To.Add(new MailAddress(Request.Form["EmailAddress"]));
                mail.From = new MailAddress("@outlook.com");

                mail.Subject = "Appointment Confirmation";
                mail.Body =
                    "You booked an appointment.\n" +
                    "Patient ID： " + Request.Form["PatientID"] + "\n" +
                    "Doctor ID： " + Request.Form["DoctorID"] + "\n" +
                    "Date： " + Request.Form["AppointmentDate"] + "\n";

                mail.IsBodyHtml = false;

                var attachment = new System.Net.Mail.Attachment(FilePath);
                mail.Attachments.Add(attachment);

                // Add smtp server.
                var smtp = new SmtpClient();
                smtp.Host = "smtp-mail.outlook.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential
                    ("@outlook.com", "");
                smtp.Send(mail);
                return "Success";
            }
            return "Database Unavailable.";

        }



        // GET: HealthCareAppointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HealthCareAppointments healthCareAppointments = db.HealthCareAppointments.Find(id);
            if (healthCareAppointments == null)
            {
                return HttpNotFound();
            }
            return View(healthCareAppointments);
        }

        // POST: HealthCareAppointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PatientID,DoctorID,Date")] HealthCareAppointments healthCareAppointments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(healthCareAppointments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(healthCareAppointments);
        }

        // GET: HealthCareAppointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HealthCareAppointments healthCareAppointments = db.HealthCareAppointments.Find(id);
            if (healthCareAppointments == null)
            {
                return HttpNotFound();
            }
            return View(healthCareAppointments);
        }

        // POST: HealthCareAppointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HealthCareAppointments healthCareAppointments = db.HealthCareAppointments.Find(id);
            db.HealthCareAppointments.Remove(healthCareAppointments);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASS2_20240802.Models;
using Microsoft.AspNet.Identity;

namespace ASS2_20240802.Controllers
{
    public class DoctorUserMedicationsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // User Action Log
        private void LogUserAction(string action)
        {
            if (!User.IsInRole("Admin"))
            {
                var log = new UserActionLog
                {
                    UserId = User.Identity.GetUserId(),
                    ActionName = " DoctorUserMedicationsController: " + action,
                    ActionTime = DateTime.Now
                };
                db.UserActionLogs.Add(log);
                db.SaveChanges();
            }
        }

        // GET: DoctorUserMedications
        [Authorize]
        public ActionResult Index(string userId = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Index");
            }
            var medicationsQuery = db.DoctorUserMedications.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                // Filter for a specific user if userId is provided (from MyPatients view)
                medicationsQuery = medicationsQuery.Where(m => m.UserId == userId);
            }

            if (User.IsInRole("Admin"))
            {
                var allMedications = medicationsQuery.Select(m => new MedicationViewModel
                {
                    Id = m.Id,
                    UserId = m.UserId, // Displaying UserId for Admins
                    UserEmail = db.Users.FirstOrDefault(u => u.Id == m.UserId).Email,
                    PrescribingDoctorEmail = db.Users.FirstOrDefault(u => u.Id == m.PrescribingDoctorUserId).Email,
                    MedicationName = m.MedicationName,
                    MedicationDescription = m.MedicationDescription,
                    DosageInstructions = m.DosageInstructions,
                    CanEdit = true // Admins can edit all entries
                }).ToList();
                return View(allMedications);
            }
            else if (User.IsInRole("Doctor"))
            {
                var doctorId = User.Identity.GetUserId();
                var viewableMedications = medicationsQuery.ToList().Select(m => new MedicationViewModel
                {
                    Id = m.Id,
                    UserEmail = db.Users.FirstOrDefault(u => u.Id == m.UserId).Email,
                    PrescribingDoctorEmail = db.Users.Where(u => u.Id == m.PrescribingDoctorUserId).Select(u => u.Email).FirstOrDefault() ?? "Self-diagnosis",
                    MedicationName = m.MedicationName,
                    MedicationDescription = m.MedicationDescription,
                    DosageInstructions = m.DosageInstructions,
                    CanEdit = m.PrescribingDoctorUserId == doctorId
                });

                ViewBag.UserId = userId;
                return View(viewableMedications);
            }
            else if (User.IsInRole("User"))
            {
                var PatientId = User.Identity.GetUserId();
                var medicationsForUser = db.DoctorUserMedications
                    .Where(m => m.UserId == PatientId)
                    .Select(m => new MedicationViewModel
                    {
                        Id = m.Id,
                        UserEmail = db.Users.Where(u => u.Id == m.UserId).Select(u => u.Email).FirstOrDefault(),
                        PrescribingDoctorEmail = db.Users.Where(u => u.Id == m.PrescribingDoctorUserId).Select(u => u.Email).FirstOrDefault(),
                        MedicationName = m.MedicationName,
                        MedicationDescription = m.MedicationDescription,
                        DosageInstructions = m.DosageInstructions,
                        CanEdit = m.PrescribingDoctorUserId == null  // 允许用户编辑开药医生为 null 的条目
                    }).ToList();

                return View(medicationsForUser);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }





        // GET: DoctorUserMedications/Details/5
        public ActionResult Details(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Details");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorUserMedication doctorUserMedication = db.DoctorUserMedications.Find(id);
            if (doctorUserMedication == null)
            {
                return HttpNotFound();
            }
            return View(doctorUserMedication);
        }

        // GET: DoctorUserMedications/Create
        public ActionResult Create(string userId = null)
        {
            if (!string.IsNullOrEmpty(userId) && User.IsInRole("Doctor"))
            {
                ViewBag.UserId = userId;  // 将病人ID传递给视图
            }
            return View();
        }

        // POST: DoctorUserMedications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MedicationName,MedicationDescription,DosageInstructions")] DoctorUserMedication doctorUserMedication, string userId)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Create");
            }
            if (ModelState.IsValid)
            {
                if (User.IsInRole("Doctor"))
                {
                    doctorUserMedication.PrescribingDoctorUserId = User.Identity.GetUserId();
                    doctorUserMedication.UserId = userId;  // 使用从GET方法传递的病人ID
                }
                else if (User.IsInRole("User"))
                {
                    doctorUserMedication.PrescribingDoctorUserId = null;  // 用户为自己开药
                    doctorUserMedication.UserId = User.Identity.GetUserId();  // 当前用户的ID
                }
                else if (User.IsInRole("Admin"))
                {
                    // Admins can edit all fields, including setting the doctor and user IDs directly
                }

                try
                {
                    db.DoctorUserMedications.Add(doctorUserMedication);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the medication: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", "Model state is invalid");
            }

            return View(doctorUserMedication);
        }





        // GET: DoctorUserMedications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorUserMedication doctorUserMedication = db.DoctorUserMedications.Find(id);
            if (doctorUserMedication == null)
            {
                return HttpNotFound();
            }
            return View(doctorUserMedication);
        }

        // POST: DoctorUserMedications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,PrescribingDoctorUserId,MedicationName,MedicationDescription,DosageInstructions")] DoctorUserMedication doctorUserMedication)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Edit");
            }
            if (ModelState.IsValid)
            {
                db.Entry(doctorUserMedication).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctorUserMedication);
        }

        // GET: DoctorUserMedications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorUserMedication doctorUserMedication = db.DoctorUserMedications.Find(id);
            if (doctorUserMedication == null)
            {
                return HttpNotFound();
            }
            return View(doctorUserMedication);
        }

        // POST: DoctorUserMedications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Delete");
            }
            DoctorUserMedication doctorUserMedication = db.DoctorUserMedications.Find(id);
            db.DoctorUserMedications.Remove(doctorUserMedication);
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

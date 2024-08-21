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
    public class DoctorInformationsController : Controller
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
                    ActionName = " DoctorInformationsController: " + action,
                    ActionTime = DateTime.Now
                };
                db.UserActionLogs.Add(log);
                db.SaveChanges();
            }
        }

        // GET: DoctorInformations
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Index");
            }
            return View(db.DoctorInformations.ToList());
        }

        public ActionResult MyIndex()
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("MyIndex");
            }
            string currentUserId = User.Identity.GetUserId();
            var doctorInformation = db.DoctorInformations.FirstOrDefault(d => d.DoctorId == currentUserId);

            if (doctorInformation == null)
            {
                return HttpNotFound("Your information is not available.");
            }

            return View(doctorInformation);
        }

        // GET: DoctorInformations/Details/5
        public ActionResult Details(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Details");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorInformation doctorInformation = db.DoctorInformations.Find(id);
            if (doctorInformation == null)
            {
                return HttpNotFound();
            }
            return View(doctorInformation);
        }

        // GET: DoctorInformations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DoctorInformations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DoctorId,Name,Major,Description,PhoneNumber")] DoctorInformation doctorInformation)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Create");
            }
            if (ModelState.IsValid)
            {
                db.DoctorInformations.Add(doctorInformation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(doctorInformation);
        }

        // GET: DoctorInformations/EditDoctorInformation/5
        [Authorize(Roles = "Doctor,Admin")] // 确保访问权限
        public ActionResult EditDoctorInformation(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!User.IsInRole("Admin") && User.Identity.GetUserId() != id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden); // 不允许非管理员编辑其他医生信息
            }
            DoctorInformation doctorInformation = db.DoctorInformations.Find(id);
            if (doctorInformation == null)
            {
                return HttpNotFound();
            }
            return View(doctorInformation);
        }

        // POST: DoctorInformations/EditDoctorInformation/5
        [HttpPost]
        [Authorize(Roles = "Doctor,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditDoctorInformation([Bind(Include = "DoctorId,Name,Major,Description,PhoneNumber")] DoctorInformation doctorInformation)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Edit");
            }
            if (!User.IsInRole("Admin") && User.Identity.GetUserId() != doctorInformation.DoctorId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden); // 不允许非管理员提交其他医生信息
            }
            if (ModelState.IsValid)
            {
                db.Entry(doctorInformation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyIndex");
            }
            return View(doctorInformation);
        }



        // GET: DoctorInformations/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorInformation doctorInformation = db.DoctorInformations.Find(id);
            if (doctorInformation == null)
            {
                return HttpNotFound();
            }
            return View(doctorInformation);
        }

        // POST: DoctorInformations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Delete");
            }
            DoctorInformation doctorInformation = db.DoctorInformations.Find(id);
            db.DoctorInformations.Remove(doctorInformation);
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

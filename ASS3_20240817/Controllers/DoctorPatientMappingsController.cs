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
using Microsoft.AspNet.Identity.Owin;

namespace ASS2_20240802.Controllers
{
    public class DoctorPatientMappingsController : Controller
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
                    ActionName = "DoctorPatientMappingsController: " + action,
                    ActionTime = DateTime.Now
                };
                db.UserActionLogs.Add(log);
                db.SaveChanges();
            }
        }

        // GET: DoctorPatientMappings
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Index");
            }
            var mappings = db.DoctorPatientMappings.ToList();
            return View(mappings);
        }


        // GET: DoctorPatientMappings/MyPatients
        public ActionResult MyPatients()
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("MyPatients");
            }
            var doctorId = User.Identity.GetUserId(); // 获取当前登录医生的ID
            var mappings = db.DoctorPatientMappings
                             .Where(m => m.DoctorId == doctorId) // 筛选出医生ID匹配的条目
                             .Include(m => m.User) // 确保加载相关的用户信息
                             .ToList();
            return View(mappings);
        }

        // 在类的开始添加角色限制
        [Authorize(Roles = "Doctor")]
        // 确保每个操作都进行了相应的权限验证
        public ActionResult Details(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Details");
            }
            var mapping = db.DoctorPatientMappings.Find(id);
            if (mapping == null || mapping.DoctorId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(mapping);
        }


        // GET: DoctorPatientMappings/Create
        [Authorize(Roles = "Doctor")]
        public ActionResult Create()
        {
            // Directly select users without filtering by role
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: DoctorPatientMappings/Create
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string userEmail)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Create");
            }
            var user = db.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "No user found with the given email.");
                return View();
            }

            var doctorId = User.Identity.GetUserId();
            var existingMapping = db.DoctorPatientMappings.FirstOrDefault(m => m.DoctorId == doctorId && m.UserId == user.Id);
            if (existingMapping != null)
            {
                ModelState.AddModelError("", "This patient is already mapped to you.");
                return View();
            }

            DoctorPatientMapping doctorPatientMapping = new DoctorPatientMapping
            {
                DoctorId = doctorId,
                UserId = user.Id
            };

            db.DoctorPatientMappings.Add(doctorPatientMapping);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }


        // GET: DoctorPatientMappings/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorPatientMapping doctorPatientMapping = db.DoctorPatientMappings.Find(id);
            if (doctorPatientMapping == null)
            {
                return HttpNotFound();
            }
            return View(doctorPatientMapping);
        }

        // POST: DoctorPatientMappings/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DoctorId,UserId")] DoctorPatientMapping doctorPatientMapping)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Edit");
            }
            if (ModelState.IsValid)
            {
                db.Entry(doctorPatientMapping).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(doctorPatientMapping);
        }


        // GET: DoctorPatientMappings/Delete/5
        [Authorize(Roles = "Doctor")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorPatientMapping doctorPatientMapping = db.DoctorPatientMappings.Find(id);
            if (doctorPatientMapping == null)
            {
                return HttpNotFound();
            }
            // Ensure the logged-in doctor can only delete their own patient mappings
            if (doctorPatientMapping.DoctorId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(doctorPatientMapping);
        }

        // POST: DoctorPatientMappings/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Doctor")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Delete");
            }
            DoctorPatientMapping doctorPatientMapping = db.DoctorPatientMappings.Find(id);
            // Double-check the doctor ID on delete to ensure security
            if (doctorPatientMapping.DoctorId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            db.DoctorPatientMappings.Remove(doctorPatientMapping);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }


        public ActionResult MyDoctors()
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("MyDoctors");
            }
            string currentUserId = User.Identity.GetUserId();
            var doctorIds = db.DoctorPatientMappings
                              .Where(m => m.UserId == currentUserId)
                              .Select(m => m.DoctorId)
                              .ToList();

            var doctors = db.Users
                            .Where(u => doctorIds.Contains(u.Id))
                            .Join(db.DoctorInformations,
                                  user => user.Id,
                                  doctorInfo => doctorInfo.DoctorId,
                                  (user, doctorInfo) => new DoctorViewModel
                                  {
                                      Email = user.Email,
                                      Name = doctorInfo.Name,
                                      Major = doctorInfo.Major,
                                      PhoneNumber = doctorInfo.PhoneNumber
                                  }).ToList();

            return View(doctors);
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

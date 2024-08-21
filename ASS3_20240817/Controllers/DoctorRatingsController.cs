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
    public class DoctorRatingsController : Controller
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
                    ActionName = "DoctorRatingsController: " + action,
                    ActionTime = DateTime.Now
                };
                db.UserActionLogs.Add(log);
                db.SaveChanges();
            }
        }


        // GET: DoctorRatings
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Index");
            }
            var doctors = db.DoctorInformations.ToList();
            var ratings = db.DoctorRatings.GroupBy(r => r.DoctorId)
                                          .Select(g => new { DoctorId = g.Key, AverageScore = g.Average(r => r.Score) })
                                          .ToList();

            var viewModel = doctors.Select(d => new DoctorRatingViewModel
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                Major = d.Major,
                Description = d.Description,
                PhoneNumber = d.PhoneNumber,
                AverageScore = ratings.FirstOrDefault(r => r.DoctorId == d.DoctorId)?.AverageScore
            }).ToList();
            return View(viewModel);
        }


        // GET: DoctorRatings/Details/5
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
            DoctorRating doctorRating = db.DoctorRatings.Find(id);
            if (doctorRating == null)
            {
                return HttpNotFound();
            }
            LogUserAction("_Details");
            return View(doctorRating);
        }

        // GET: DoctorRatings/Create
        public ActionResult Create(string doctorId) // 假设从界面传入了医生的ID
        {

            var model = new DoctorRating
            {
                DoctorId = doctorId,
                UserId = User.Identity.GetUserId(), // 获取当前登录用户的ID
                CreatedDate = DateTime.Now // 设置当前时间为创建时间
            };
            return View(model);
        }

        // POST: DoctorRatings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DoctorId,Score")] DoctorRating doctorRating)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Create");
            }
            if (ModelState.IsValid)
            {
                // 获取当前用户ID
                string currentUserId = User.Identity.GetUserId();

                // 查找并删除该用户对该医生的所有旧评分
                var existingRatings = db.DoctorRatings.Where(r => r.UserId == currentUserId && r.DoctorId == doctorRating.DoctorId).ToList();
                foreach (var rating in existingRatings)
                {
                    db.DoctorRatings.Remove(rating);
                }

                // 创建新的评分记录
                doctorRating.UserId = currentUserId; // 设置用户ID
                doctorRating.CreatedDate = DateTime.Now; // 设置创建日期

                db.DoctorRatings.Add(doctorRating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(doctorRating);
        }



        // GET: DoctorRatings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorRating doctorRating = db.DoctorRatings.Find(id);
            if (doctorRating == null)
            {
                return HttpNotFound();
            }
            return View(doctorRating);
        }

        // POST: DoctorRatings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RatingId,DoctorId,UserId,Score,CreatedDate")] DoctorRating doctorRating)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Edit");
            }
            if (ModelState.IsValid)
            {
                db.Entry(doctorRating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctorRating);
        }

        // GET: DoctorRatings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorRating doctorRating = db.DoctorRatings.Find(id);
            if (doctorRating == null)
            {
                return HttpNotFound();
            }
            return View(doctorRating);
        }

        // POST: DoctorRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                LogUserAction("Delete");
            }
            DoctorRating doctorRating = db.DoctorRatings.Find(id);
            db.DoctorRatings.Remove(doctorRating);
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

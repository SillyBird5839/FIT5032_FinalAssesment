using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASS2_20240802.Models;
using Microsoft.AspNet.Identity;
using ClosedXML.Excel;
using System.IO;

namespace ASS2_20240802.Controllers
{
    public class UserActionLogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: UserActionLogs
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.UserActionLogs.ToList());
        }

        // GET: UserActionLogs/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserActionLog userActionLog = db.UserActionLogs.Find(id);
            if (userActionLog == null)
            {
                return HttpNotFound();
            }
            return View(userActionLog);
        }

        // GET: UserActionLogs/Create
        public ActionResult Create()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: UserActionLogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,ActionName,ActionTime")] UserActionLog userActionLog)
        {
            if (!User.IsInRole("Admin"))
            {
                // 为非管理员用户自动填充日志
                userActionLog.UserId = User.Identity.GetUserId();
                userActionLog.ActionName = "Auto-Logged Action";
                userActionLog.ActionTime = DateTime.Now;

                db.UserActionLogs.Add(userActionLog);
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                db.UserActionLogs.Add(userActionLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userActionLog);
        }

        // GET: UserActionLogs/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserActionLog userActionLog = db.UserActionLogs.Find(id);
            if (userActionLog == null)
            {
                return HttpNotFound();
            }
            return View(userActionLog);
        }

        // POST: UserActionLogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,UserId,ActionName,ActionTime")] UserActionLog userActionLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userActionLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userActionLog);
        }

        // GET: UserActionLogs/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserActionLog userActionLog = db.UserActionLogs.Find(id);
            if (userActionLog == null)
            {
                return HttpNotFound();
            }
            return View(userActionLog);
        }

        // POST: UserActionLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            UserActionLog userActionLog = db.UserActionLogs.Find(id);
            db.UserActionLogs.Remove(userActionLog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        public ActionResult ExportToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("User Actions");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "UserID";
                worksheet.Cell(currentRow, 2).Value = "Action Name";
                worksheet.Cell(currentRow, 3).Value = "Action Time";

                foreach (var log in db.UserActionLogs.ToList())
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = log.UserId;
                    worksheet.Cell(currentRow, 2).Value = log.ActionName;
                    worksheet.Cell(currentRow, 3).Value = log.ActionTime;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UserActionLogs.xlsx");
                }
            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult ActionNameStatistics()
        {
            var actionCounts = db.UserActionLogs
                                 .GroupBy(log => log.ActionName)
                                 .Select(group => new ActionCountViewModel
                                 {
                                     ActionName = group.Key,
                                     Count = group.Count()
                                 }).ToList();

            return View(actionCounts);  // Pass the list of ActionCountViewModel to the view
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

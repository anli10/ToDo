﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ToDo.Models;

namespace Lab10IdentityNew.Controllers
{
    [Authorize(Roles = "User,Editor,Administrator")]

    public class ActivitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Activities

        public ActionResult Index()
        {
            if (User.IsInRole("Administrator"))
            {
                var activities = db.Activities.Include(a => a.Project);
                return View(activities.ToList());
            }
            else if (User.IsInRole("Editor") || User.IsInRole("User"))
            {
                var userId = User.Identity.GetUserId();
                var teamsId = db.Teams.Where(t => t.ApplicationUsers.Select(i => i.Id).Contains(userId)).Select(t => t.Id);
                var projectsId = from proj in db.Projects
                               orderby proj.Team.Id
                               where teamsId.Contains(proj.Team.Id)
                               select proj.Id;
                var activities = from activity in db.Activities
                                 where projectsId.Contains(activity.ProjectId)
                                 select activity;
                return View(activities.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        // GET: Activities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: Activities/Create
        public ActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");

            var userId = User.Identity.GetUserId();
            var teamId = db.Teams.Where(t => t.ApplicationUsers.Select(i => i.Id).Contains(userId)).Select(t => t.Id).First();
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.Teams.FirstOrDefault().Id == teamId), "Id", "UserName");

            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,StartDate,EndDate,ProjectId,UserId,Status")] Activity activity)
        {
            var userId = User.Identity.GetUserId();
            var teamId = db.Teams.Where(t => t.ApplicationUsers.Select(i => i.Id).Contains(userId)).Select(t => t.Id).First();
            if (ModelState.IsValid)
            {
                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.Teams.FirstOrDefault().Id == teamId), "Id", "UserName", activity.UserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", activity.ProjectId);
            return View(activity);
        }

        // GET: Activities/Edit/5
        public ActionResult Edit(int? id)
        {
            var userId = User.Identity.GetUserId();
            var teamId = db.Teams.Where(t => t.ApplicationUsers.Select(i => i.Id).Contains(userId)).Select(t => t.Id).First();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", activity.ProjectId);
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.Teams.FirstOrDefault().Id == teamId), "Id", "UserName", activity.UserId);
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,StartDate,EndDate,ProjectId,UserId,Status")] Activity activity)
        {
            var userId = User.Identity.GetUserId();
            var teamId = db.Teams.Where(t => t.ApplicationUsers.Select(i => i.Id).Contains(userId)).Select(t => t.Id).First();
            if (ModelState.IsValid)
            {
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", activity.ProjectId);
            ViewBag.UserId = new SelectList(db.Users.Where(u => u.Teams.FirstOrDefault().Id == teamId), "Id", "UserName", activity.UserId);

            return View(activity);
        }

        // GET: Activities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Activity activity = db.Activities.Find(id);
            db.Activities.Remove(activity);
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

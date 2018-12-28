using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ToDo.Models;

namespace ToDo.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var teamsId = db.Teams.Where(t => t.ApplicationUsers.Select(i => i.Id).Contains(userId)).Select(t => t.Id);
            var projects = from proj in db.Projects
                           orderby proj.Team.Id
                           where teamsId.Contains(proj.Team.Id)
                           select proj;
            return View(projects.ToList());
           
        }
        // GET: Projects/Details/5
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User.Identity.GetUserId();
            var teamsId = db.Teams.Where(t => t.ApplicationUsers.Select(i => i.Id).Contains(userId)).Select(t => t.Id);
            var projects = from proj in db.Projects
                           orderby proj.Team.Id
                           where teamsId.Contains(proj.Team.Id)
                           select proj;

            Project project = db.Projects.Find(id);
            if (project == null || !projects.Select(p => p.Id).Contains(project.Id))
            {
                return HttpNotFound();
            }
         
         return View(project);
 
        }

        // GET: Projects/Create
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.TeamId = new SelectList(db.Teams.Where(t => t.ApplicationUsers.Select(i => i.Id).Contains(userId)), "Id", "Name");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User,Editor,Administrator")]

        public ActionResult Create([Bind(Include = "Id,Name,Description,TeamId,EditorId")] Project project)
        {
            var userId = User.Identity.GetUserId();
            var teamsId = db.Teams.Where(t => t.ApplicationUsers.Select(i => i.Id).Contains(userId)).Select(t => t.Id);
            var projects = from proj in db.Projects
                           orderby proj.Team.Id
                           where teamsId.Contains(proj.Team.Id)
                           select proj;
            ViewBag.TeamId = teamsId;
            project.EditorId = userId;

            ApplicationUser user = db.Users.Find(userId);
            var userRole = user.Roles.FirstOrDefault();

            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new
                RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new
                UserStore<ApplicationUser>(context));

            if (TryUpdateModel(user))
            {
                
                UserManager.RemoveFromRole(userId, "User");
                UserManager.AddToRole(userId, "Editor");
            }

            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "User,Editor,Administrator")]

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = db.Projects.Find(id);

            var userId = User.Identity.GetUserId();
            var teamsId = db.Teams.Where(t => t.ApplicationUsers.Select(i => i.Id).Contains(userId)).Select(t => t.Id);
            var projects = from proj in db.Projects
                           orderby proj.Team.Id
                           where teamsId.Contains(proj.Team.Id)
                           select proj;

            ViewBag.TeamId = new SelectList(db.Teams.Where(t => t.ApplicationUsers.Select(i => i.Id).Contains(userId)), "Id", "Name");


            if (project == null || !projects.Select(p => p.Id).Contains(project.Id))
            {
                return HttpNotFound();
            }
            
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User,Editor,Administrator")]

        public ActionResult Edit([Bind(Include = "Id,Name,Description,TeamId")] Project project)
        {
            var userId = User.Identity.GetUserId();
            var teamId = db.Teams.Where(t => t.ApplicationUsers.Select(i => i.Id).Contains(userId)).Select(t => t.Id).First();
            ViewBag.TeamId = teamId;
            project.TeamId = teamId;
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "User,Editor,Administrator")]
        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            var userId = User.Identity.GetUserId();
            var teamId = db.Teams.Where(t => t.ApplicationUsers.Select(i => i.Id).Contains(userId)).Select(t => t.Id).First();
            ViewBag.TeamId = teamId;
            project.TeamId = teamId;
            var projects = db.Projects.Include(p => p.Team).Where(p => p.Team.Id == teamId);
            if (project == null || !projects.Select(p => p.Id).Contains(project.Id))
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "User,Editor,Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            //db.SaveChanges();
            ApplicationUser user = db.Users.Find(userId);
            var userRole = user.Roles.FirstOrDefault();

            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new
                RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new
                UserStore<ApplicationUser>(context));


            UserManager.RemoveFromRole(userId, "Editor");
            UserManager.AddToRole(userId, "User");

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

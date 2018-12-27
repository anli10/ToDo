using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ToDo.Models;
using ToDo.ViewModel;


namespace ToDo.Controllers
{
    public class TeamsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Teams
        public ActionResult Index()
        {
            return View(db.Teams.ToList());
        }

        // GET: Teams/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // GET: Teams/Create
        public ActionResult Create()
        {
            var teamViewMode = new TeamViewMode();

            var allUsers = db.Users.ToList();

            ViewBag.AllUsers = allUsers.Select(x => new SelectListItem
            {
                Text = x.UserName,
                Value = x.Id
            });

            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TeamViewMode teamViewMode)
        {
            if (ModelState.IsValid)
            {
                var teamToAdd = db.Teams.Include(i => i.ApplicationUsers).First();

                if (TryUpdateModel(teamToAdd, "Team", new string[] { "Id", "Name" }))
                {
                    var updatedUsers = new HashSet<string>(teamViewMode.SelectedUsers);

                    foreach (ApplicationUser applicationUser in db.Users)
                    {
                        if (!updatedUsers.Contains(applicationUser.Id))
                        {
                            teamToAdd.ApplicationUsers.Remove(applicationUser);
                        }
                        else
                        {
                            teamToAdd.ApplicationUsers.Add((applicationUser));
                        }
                    }
                }
                db.Teams.Add(teamToAdd);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teamViewMode);
        }

        // GET: Teams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var teamViewMode = new TeamViewMode
            {
                Team = db.Teams.Include(i => i.ApplicationUsers).First(i => i.Id == id)
            };

            if (teamViewMode.Team == null)
                return HttpNotFound();
            var allUsers = db.Users.ToList();

            teamViewMode.AllUsers = allUsers.Select(x => new SelectListItem
            {
                Text = x.UserName,
                Value = x.Id
            });

            return View(teamViewMode);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TeamViewMode teamViewMode)
        {

            if (teamViewMode == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (ModelState.IsValid)
            {

                var teamToUpdate = db.Teams.Include(i => i.ApplicationUsers).First(i => i.Id == teamViewMode.Team.Id);

                if (TryUpdateModel(teamToUpdate, "Team", new string[] { "Name", "Id" }))
                {
                    var newUsers = db.Users.Where(
                        m => teamViewMode.SelectedUsers.Contains(m.Id)).ToList();
                    var updatedUser = new HashSet<string>(teamViewMode.SelectedUsers);
                    foreach (ApplicationUser applicationUser in db.Users)
                    {
                        if (!updatedUser.Contains(applicationUser.Id))
                        {
                            teamToUpdate.ApplicationUsers.Remove(applicationUser);
                        }
                        else
                        {
                            teamToUpdate.ApplicationUsers.Add((applicationUser));
                        }
                    }

                    db.Entry(teamToUpdate).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(teamViewMode);
        }

        // GET: Teams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Team team = db.Teams.Find(id);
            db.Teams.Remove(team);
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

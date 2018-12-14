using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Test1.Models;
using Microsoft.AspNet.Identity;

namespace Test1.Controllers
{
    public class GradesController : Controller
    {
        private Student_Enrollment_Entities db = new Student_Enrollment_Entities();

        // GET: Grades
        public ActionResult Index()
        {
            //var currentUser = User.Identity.GetUserName();
            var grades = db.Grades.Include(g => g.Course).Include(g => g.Student).Include(g => g.Term);
            return View(grades.ToList());
        }

        // GET: Grades/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grade grade = db.Grades.Find(id);
            if (grade == null)
            {
                return HttpNotFound();
            }
            return View(grade);
        }

        // GET: Grades/Create
        public ActionResult Create()
        {
            ViewBag.Course_ID = new SelectList(db.Courses, "Course_ID", "Course_ID");
            ViewBag.Student_email = new SelectList(db.Students, "Student_email", "Student_email");
            ViewBag.Term_ID = new SelectList(db.Terms, "Term_ID", "Term_ID");
            return View();
        }

        // POST: Grades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Term_ID,Course_ID,Student_email,Grade1")] Grade grade)
        {
            if (ModelState.IsValid)
            {
                db.Grades.Add(grade);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Course_ID = new SelectList(db.Courses, "Course_ID", "Course_ID", grade.Course_ID);
            ViewBag.Student_email = new SelectList(db.Students, "Student_email", "Student_email", grade.Student_email);
            ViewBag.Term_ID = new SelectList(db.Terms, "Term_ID", "Term_ID", grade.Term_ID);
            return View(grade);
        }

        // GET: Grades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grade grade = db.Grades.Find(id);
            if (grade == null)
            {
                return HttpNotFound();
            }
            ViewBag.Course_ID = new SelectList(db.Courses, "Course_ID", "Course_ID", grade.Course_ID);
            ViewBag.Student_email = new SelectList(db.Students, "Student_email", "Student_email", grade.Student_email);
            ViewBag.Term_ID = new SelectList(db.Terms, "Term_ID", "Term_ID", grade.Term_ID);
            return View(grade);
        }

        // POST: Grades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Term_ID,Course_ID,Student_email,Grade1")] Grade grade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grade).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Course_ID = new SelectList(db.Courses, "Course_ID", "Course_ID", grade.Course_ID);
            ViewBag.Student_email = new SelectList(db.Students, "Student_email", "Student_email", grade.Student_email);
            ViewBag.Term_ID = new SelectList(db.Terms, "Term_ID", "Term_ID", grade.Term_ID);
            return View(grade);
        }

        // GET: Grades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grade grade = db.Grades.Find(id);
            if (grade == null)
            {
                return HttpNotFound();
            }
            return View(grade);
        }

        // POST: Grades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Grade grade = db.Grades.Find(id);
            db.Grades.Remove(grade);
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

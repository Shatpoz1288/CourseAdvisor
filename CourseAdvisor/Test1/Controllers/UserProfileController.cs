using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Test1.Models;
using Microsoft.AspNet.Identity;
using System.Data;

namespace Test1.Controllers
{
    public class UserProfileController : Controller
    {

        // GET: UserProfile
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string YourDropDown)
        {
            var studentID = User.Identity.GetUserName();
            Student_Enrollment_Entities student_EnrollmentEntities = new Student_Enrollment_Entities();

            if (student_EnrollmentEntities.Students.Find(studentID) == null)
            {
                var newStudent = new Student { Student_email = studentID };
                student_EnrollmentEntities.Students.Add(newStudent);
                student_EnrollmentEntities.SaveChanges();
            }

            string selected = Request.Form["YourDropDown"];

            if (selected == "COEN")
            {
                Sequential COEN = student_EnrollmentEntities.Sequentials.FirstOrDefault(x => x.SequentialID == 100);
                student_EnrollmentEntities.Students.FirstOrDefault(x => x.Student_email == studentID).Sequentials.Add(COEN);
            }
            else if (selected == "ELEN")
            {
                Sequential ELEN = student_EnrollmentEntities.Sequentials.FirstOrDefault(x => x.SequentialID == 101);
                student_EnrollmentEntities.Students.FirstOrDefault(x => x.Student_email == studentID).Sequentials.Add(ELEN);
            }
            student_EnrollmentEntities.SaveChanges();
            return View();
        }

        public ActionResult Curriculum()
        {
            return View();
        }


        public ActionResult Remaining()
        {
            string currentStudent = User.Identity.GetUserName();
            DataTable dt = new DataTable();
            dt.Columns.Add("CourseID");
            dt.Columns.Add("Course Name");
            dt.Columns.Add("Credits");
            dt.Columns.Add("Sequential ID");
            dt.Columns.Add("Sequential Name");
            dt.Columns.Add("Term");
            DataRow row = null;

            Student_Enrollment_Entities db = new Student_Enrollment_Entities();
            //list of course id in Grades ONLY
            var gradeToList = (from g in db.Grades
                              where g.Student_email == currentStudent
                              select new { g.Course_ID}).ToList();

            //Junction table SequentialToStudent
            var query = (from s in db.Sequentials
                        where s.Students.Any(c=>c.Student_email==currentStudent)
                        select new { currentStudent, s.SequentialID, s.Sequential_Name});


            //Join SequentialToStudent to ProgramToCourses on SequentialID
            var query1 = from a in query
                         from b in db.Program_Course
                         where a.SequentialID == b.SequentialID
                         select new { currentStudent, a.SequentialID, a.Sequential_Name, b.Course_ID, b.TermID};

            //Outer join with Grades (list of courses only)
            var query2 = (from a in query
                          from b in query1
                          where a.SequentialID == b.SequentialID
                          select new { b.Course_ID }).ToList().Except(gradeToList);

            //Query2 with Query1 to get info for student/courses, but only on remaining classes (no course name/credits tho)
            var query3 = from a in query2
                         from c in query1
                         where a.Course_ID == c.Course_ID
                         orderby c.TermID
                         select new { c.Course_ID, c.SequentialID, c.Sequential_Name, c.TermID};

            //Query3 with Courses to get additional info 
            var query4 = from a in query3
                         from c in db.Courses
                         where a.Course_ID == c.Course_ID
                         orderby a.TermID
                         select new { a.Course_ID, c.Course_name, c.Credits, a.SequentialID, a.Sequential_Name, a.TermID };


            foreach (var rowObj in query4)
            {
                row = dt.NewRow();
                dt.Rows.Add(rowObj.Course_ID, rowObj.Course_name, rowObj.Credits, rowObj.SequentialID, rowObj.Sequential_Name, rowObj.TermID);
            }

            return View(dt);
        }

        public ActionResult Suggested()
        {
            string currentStudent = User.Identity.GetUserName();
            DataTable dt = new DataTable();
            dt.Columns.Add("CourseID");
            dt.Columns.Add("Course Name");
            dt.Columns.Add("Credits");
            //dt.Columns.Add("Sequential ID");
            //dt.Columns.Add("Sequential Name");
            //dt.Columns.Add("Term");
            DataRow row = null;

            int totalCredit;

            Student_Enrollment_Entities db = new Student_Enrollment_Entities();
            //list of course id in Grades ONLY
            var gradeToList = (from g in db.Grades
                               where g.Student_email == currentStudent
                               select new { g.Course_ID }).ToList();

            //list of course id in Grade, then of only credit value, and sum them
            var courseFromGrade = from a in db.Grades
                                  where a.Student_email == currentStudent
                                  select new { a.Course_ID };

            var creditCount = from a in courseFromGrade
                              from c in db.Courses
                              where a.Course_ID == c.Course_ID
                              select new { c.Credits };

            if (creditCount.Count() == 0)
            {
                totalCredit = 0;
            }
            else
            {
                totalCredit = creditCount.Sum(x => x.Credits);
            }

            //Junction table SequentialToStudent
            var query = (from s in db.Sequentials
                         where s.Students.Any(c => c.Student_email == currentStudent)
                         select new { currentStudent, s.SequentialID, s.Sequential_Name });


            //Join SequentialToStudent to ProgramToCourses on SequentialID
            var query1 = from a in query
                         from b in db.Program_Course
                         where a.SequentialID == b.SequentialID
                         select new { currentStudent, a.SequentialID, a.Sequential_Name, b.Course_ID, b.TermID };

            //Outer join with Grades (list of courses only)
            var query2 = (from a in query
                          from b in query1
                          where a.SequentialID == b.SequentialID
                          select new { b.Course_ID }).ToList().Except(gradeToList);

            //Query2 with Query1 to get info for student/courses, but only on remaining classes (no course name/credits tho)
            var query3 = from a in query2
                         from c in query1
                         where a.Course_ID == c.Course_ID
                         orderby c.TermID
                         select new { c.Course_ID, c.SequentialID, c.Sequential_Name, c.TermID };


            if (totalCredit == 0 && totalCredit <14)
            {
                //Query3 with Courses to get additional info 
                var query4 = from a in query3
                             from c in db.Courses
                             where a.Course_ID == c.Course_ID & a.TermID==1
                             orderby a.TermID
                             select new { a.Course_ID, c.Course_name, c.Credits };
                foreach (var rowObj in query4)
                {
                    row = dt.NewRow();
                    dt.Rows.Add(rowObj.Course_ID, rowObj.Course_name, rowObj.Credits);
                }
            }
            else if (totalCredit < 17)
            {
                //Query3 with Courses to get additional info 
                var query4 = from a in query3
                             from c in db.Courses
                             where a.Course_ID == c.Course_ID & a.TermID == 2
                             orderby a.TermID
                             select new { a.Course_ID, c.Course_name, c.Credits };
                foreach (var rowObj in query4)
                {
                    row = dt.NewRow();
                    dt.Rows.Add(rowObj.Course_ID, rowObj.Course_name, rowObj.Credits);
                }
            }
            else if (totalCredit < 53)
            {
                //Query3 with Courses to get additional info 
                var query4 = from a in query3
                             from c in db.Courses
                             where a.Course_ID == c.Course_ID & a.TermID == 3
                             orderby a.TermID
                             select new { a.Course_ID, c.Course_name, c.Credits };
                foreach (var rowObj in query4)
                {
                    row = dt.NewRow();
                    dt.Rows.Add(rowObj.Course_ID, rowObj.Course_name, rowObj.Credits);
                }
            }
            else if (totalCredit < 71)
            {
                //Query3 with Courses to get additional info 
                var query4 = from a in query3
                             from c in db.Courses
                             where a.Course_ID == c.Course_ID & a.TermID == 4
                             orderby a.TermID
                             select new { a.Course_ID, c.Course_name, c.Credits };
                foreach (var rowObj in query4)
                {
                    row = dt.NewRow();
                    dt.Rows.Add(rowObj.Course_ID, rowObj.Course_name, rowObj.Credits);
                }
            }
            else if (totalCredit < 89)
            {
                //Query3 with Courses to get additional info 
                var query4 = from a in query3
                             from c in db.Courses
                             where a.Course_ID == c.Course_ID & a.TermID == 5
                             orderby a.TermID
                             select new { a.Course_ID, c.Course_name, c.Credits };
                foreach (var rowObj in query4)
                {
                    row = dt.NewRow();
                    dt.Rows.Add(rowObj.Course_ID, rowObj.Course_name, rowObj.Credits);
                }
            }
            else if (totalCredit < 107)
            {
                //Query3 with Courses to get additional info 
                var query4 = from a in query3
                             from c in db.Courses
                             where a.Course_ID == c.Course_ID & a.TermID == 6
                             orderby a.TermID
                             select new { a.Course_ID, c.Course_name, c.Credits };
                foreach (var rowObj in query4)
                {
                    row = dt.NewRow();
                    dt.Rows.Add(rowObj.Course_ID, rowObj.Course_name, rowObj.Credits);
                }
            }
            else if (totalCredit < 125)
            {
                //Query3 with Courses to get additional info 
                var query4 = from a in query3
                             from c in db.Courses
                             where a.Course_ID == c.Course_ID & a.TermID == 7
                             orderby a.TermID
                             select new { a.Course_ID, c.Course_name, c.Credits };
                foreach (var rowObj in query4)
                {
                    row = dt.NewRow();
                    dt.Rows.Add(rowObj.Course_ID, rowObj.Course_name, rowObj.Credits);
                }
            }
            else if (totalCredit < 143)
            {
                //Query3 with Courses to get additional info 
                var query4 = from a in query3
                             from c in db.Courses
                             where a.Course_ID == c.Course_ID & a.TermID == 8
                             orderby a.TermID
                             select new { a.Course_ID, c.Course_name, c.Credits };
                foreach (var rowObj in query4)
                {
                    row = dt.NewRow();
                    dt.Rows.Add(rowObj.Course_ID, rowObj.Course_name, rowObj.Credits);
                }
            }

            /*foreach (var rowObj in query4)
            {
                row = dt.NewRow();
                dt.Rows.Add(rowObj.Course_ID, rowObj.Course_name, rowObj.Credits, rowObj.SequentialID, rowObj.Sequential_Name, rowObj.TermID);
            }*/

            return View(dt);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DiarySchema.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace DiarySchema.Controllers
{
    public class HomeController : Controller
    {
        DiaryContext db = new DiaryContext();
        public RedirectResult Index()
        {
            
           string redirectString = "/Account/Login";
            bool isStudent = HttpContext.User.IsInRole("Student");
            bool isLecturer = HttpContext.User.IsInRole("Lecturer");
            bool isAdmin = HttpContext.User.IsInRole("Admin");
            var todaySemester = GetTodaySemester();
            string userNAme = HttpContext.User.Identity.Name;
            string year = todaySemester.Year;
            string semesterNumber = todaySemester.Number;
            string lecturerSubject = GetFirstSubjectForLecturer(HttpContext.User.Identity.Name);
            ///string lecturerGroup = GetFirstGroupForLecturer(HttpContext.User.Identity.Name);
            //Переадрисация в зависимости от роли
            if (isStudent)
            {
                redirectString = "/Student/Index/"+ year + "/"+ semesterNumber;
            }
            if (isLecturer)
                redirectString = "/Lecturer/Index/1/2";
            if (isAdmin)
                redirectString = "/Student/Index";//Доделать!
            return Redirect(redirectString);
            /* var subjectNames = from p in db.Subjects
                                join c in db.TeachersGroupsSubjects on p.Id equals c.SubjectId
                                join g in db.Groups on c.GroupId equals g.Id
                                join s in db.Students on g.Id equals s.GroupId
                                where s.Id == 7
                                select new {Id = p.Id, Name = p.Name, Departament = p.Departament  };
             var names = new List<string>();
             foreach(var b in subjectNames)
             {
                 names.Add(b.Name);
             }
             ViewBag.SujectNames = names;
             //список оценок

            /* var subjectNames = from p in db.Subjects
           */
        }
        private string GetFirstSubjectForLecturer(string userName)
        {
            return "fdsfs";
        }
        public ActionResult Error()
        {
            

            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        private Semester GetTodaySemester()
        {
            DiaryConnection db = new DiaryConnection();
            //Надо дописать с обращением к базе данных
            var toDay = DateTime.Today;
            var semester = db.Semester.Where(p => p.BeginningDate < toDay && p.EndDate > toDay).First(); ;
            return semester;
        }
    }
}
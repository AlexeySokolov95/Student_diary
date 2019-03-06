using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DiarySchema.Models;
using System.Collections;

namespace DiarySchema.Controllers
{
    public class StudentController : Controller
    {
        DiaryConnection dbContext = new DiaryConnection();
        public ActionResult Index(string year, string semesterNumber)
        {
            //переместить всю логику в StudentJournalViewModel?
            //будущие параметры для методы
            string userName = HttpContext.User.Identity.Name;
            var student = dbContext.Students.Where(p => p.UserName == userName).First();
            //var semester = dbContext.Semester.Where(p => p.Year == year && p.Number == semesterNumber).First();

            var semester = dbContext.Semester.Where(p => p.Year == "2016|2017" && p.Number == "1").First();
           // var semester = Semester.Today();
            var viewModel = new StudentJournalViewModel();
            //db.Users.Where(p => p.UserName == userName).ToList();
            //var student = (from p in db.Students where p.UserName == userName select new { Id = p.Id }).ToList();//db.Students.Where(p => p.Id == 7).ToList();
            // stdId = student[0].Id;
            //Заполнения модели представления
            //Получение название предметов для журнала
            viewModel.GetNamesOfSubject(dbContext, student.Id, semester);
            //Получение данных для заполнения области таблицы с оценками
            viewModel.GetSubjectGrades(dbContext, student.Id, semester);
            //Для строки с названиями месяцев
            viewModel.GetMounthNames();
            //С количеством дней в месяцах для для colspan.
            viewModel.GetDaysMounth();
            //Для отображение строки где день месяца + день недели.
            viewModel.GetDaysSemester();
            //Для отображение пропусков ввида:”3 пропуска из 24”
            viewModel.GetMissed();
            //Для формирование таблиц с итоговыми оценками
            viewModel.GetAllFinalGrades(dbContext, student.Id, semester);
            //полное имя студента 
            //viewModel.Student = student;


            //получения даты начало поступления
            var StartDate = dbContext.Groups.Where(p => p.Id == student.GroupId).First(); //student.GroupId;
            int YearStart =  StartDate.YearOfAdmission;                   
            List<string> SemesterLearning=new List<string>();
            var  date1 = DateTime.Now;           
            for(int i=0; i<4;i++)
            {
                if (date1.Year >= (int)(YearStart + 1))
                {
                    SemesterLearning.Add(YearStart + "|" + (int)(YearStart+1));
                    //SemesterLearning.Add(YearStart + "|" + YearStart1+"/"+2);
                    YearStart++;                    
                }
            }
            ViewBag.SemesterLearning1= SemesterLearning;
            ViewBag.Year = year;
            ViewBag.Semester = semesterNumber;

            viewModel.GetRefToSemesters(dbContext, student.Id);

            return View(viewModel);
        }
        private Semester GetTodaySemester()
        {
            //Надо дописать с обращением к базе данных
            var toDay = DateTime.Today;
            var semester = dbContext.Semester.Where(p => p.BeginningDate > toDay && p.EndDate < toDay).First(); ;
            return semester;
        }
    }
}
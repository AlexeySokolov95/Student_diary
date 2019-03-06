using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DiarySchema.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DiarySchema.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Students()
        {
            List<Students> students;
            DiaryConnection db = new DiaryConnection();
            students = db.Students.ToList();
            return View(students);
        }
        [HttpPost]
        public ActionResult StudentsSearch(string number, string fullName, string groupNumber)
        {
            IEnumerable<Students> foundStudents;
            List<Students> students;
            using (DiaryConnection db = new DiaryConnection())
            {
                students = db.Students.ToList();
            }
            foundStudents = students;
            if (number != "")
                foundStudents = foundStudents.Where(p => p.Number.Contains(number)).ToList();// == number);
            if (fullName != "")
                foundStudents = foundStudents.Where(p => p.FullName.Contains(fullName)).ToList();// == fullName);
            if (groupNumber != "")
                foundStudents = foundStudents.Where(p => p.GroupId == Convert.ToInt32(groupNumber));

            return PartialView(foundStudents);


            //List<Students> foundStudents;
            //DiaryConnection db = new DiaryConnection();
            //{
            //    if (number == "" && fullName == "" && groupNumber == "")
            //        foundStudents = db.Students.ToList();
            //    else if (fullName == "" && groupNumber == "")
            //        foundStudents = db.Students.Where(p => p.Number == number).ToList();
            //    else if (number == "" && groupNumber == "")
            //        foundStudents = db.Students.Where(p => p.FullName.Contains(fullName)).ToList();
            //    else if (number == "" && fullName == "")
            //    {
            //        var students = db.Students.ToList();
            //        foundStudents = students.Where(p => p.Groups1.GetGroupNumber(Semester.Today()) == Convert.ToInt32(groupNumber)).ToList();
            //    }
            //    else if (groupNumber == null)
            //        foundStudents = db.Students.Where(p => p.Number == number && p.FullName.Contains(fullName)).ToList();
            //    else if (number == "")
            //        foundStudents = db.Students.Where(p => p.FullName.Contains(fullName)).ToList().
            //            Where(p => p.Groups1.GetGroupNumber(Semester.Today()) == Convert.ToInt32(groupNumber)).ToList();
            //    else if (fullName == "")
            //        foundStudents = db.Students.Where(p => p.Number == number).ToList().
            //            Where(p => p.Groups1.GetGroupNumber(Semester.Today()) == Convert.ToInt32(groupNumber)).ToList();
            //    else
            //    {
            //        foundStudents = db.Students.Where(p => p.Number == number && p.FullName.Contains(fullName)).ToList().
            //            Where(p => p.Groups1.GetGroupNumber(Semester.Today()) == Convert.ToInt32(groupNumber)).ToList();
            //    }
            //}
            //return PartialView(foundStudents);
        }
        public ActionResult AddStudents()
        {
            List<Groups> groups = new List<Models.Groups>();
            using (DiaryConnection db = new DiaryConnection())
            {
                groups = db.Groups.ToList();
                foreach(var b in groups)
                {
                    b.Number = b.GetGroupNumber(Semester.Today());
                    ViewBag.Groups = new SelectList(groups, "Id", "Number");
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddStudents(Students student)
        {
            if (ModelState.IsValid)
            {
                using (DiaryConnection db = new DiaryConnection())
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                }
                return RedirectToAction("Students");
            }
            ViewBag.ErrorMessage = "Неверный ввод данных";
            List<Groups> groups = new List<Models.Groups>();
            using (DiaryConnection db = new DiaryConnection())
            {
                groups = db.Groups.ToList();
                foreach (var b in groups)
                {
                    b.Number = b.GetGroupNumber(Semester.Today());
                    ViewBag.Groups = new SelectList(groups, "Id", "Number",student.Groups1);
                }
            }
            return View(student);
        }
        [HttpGet]
        public ActionResult EditStudents(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Students student;
            DiaryConnection db = new DiaryConnection();
            student = db.Students.Find(id);
            if (student != null)
            {
                List<Groups>groups = db.Groups.ToList();
                foreach (var b in groups)
                {
                    b.Number = b.GetGroupNumber(Semester.Today());
                    ViewBag.Groups = new SelectList(groups, "Id", "Number", student.Groups1);
                }
                db.Dispose();
                return View(student);
            }
            db.Dispose();
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult EditStudents([Bind(Include = "Id,FullName,Number,YearOfBirth,GroupId,Email")]Students student)
        {
            using (DiaryConnection db = new DiaryConnection())
            {

                if (ModelState.IsValid)
                {
                    db.Entry(student).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    List<Groups> groups = db.Groups.ToList();
                    foreach (var b in groups)
                    {
                        b.Number = b.GetGroupNumber(Semester.Today()); 
                    }
                    ViewBag.Groups = new SelectList(groups, "Id", "Number");
                    db.Dispose();
                    ViewBag.ErrorMessage = "Неверный ввод данных";
                    View(student);
                }
            }
            return RedirectToAction("Students");
        }
        [HttpGet]
        public ActionResult DeleteStudent(int id)
        {
            Students student;
            using (DiaryConnection db = new DiaryConnection())
                student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = id;
            return View(student);
        }

        [HttpPost, ActionName("DeleteStudent")]
        public ActionResult DeleteStudentConfirmed(int id)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                Students student = db.Students.Find(id);
                if (student == null)
                {
                    return HttpNotFound();
                }
                db.Students.Remove(student);
                db.SaveChanges();
            }
            return RedirectToAction("Students");
        }
        public ActionResult Groups()
        {
            List<Groups> groups;
            DiaryConnection db = new DiaryConnection();
            groups = db.Groups.ToList();
            return View(groups);
            //return View();
        }
        public ActionResult GroupsSearch(int? number, int? yearOfAdmission, string faculty, string degree, string formOfStudy)
        {
            IEnumerable<Groups> foundGroups;
            List<Groups> groups;
            using (DiaryConnection db = new DiaryConnection())
            {
                groups = db.Groups.ToList();
            }
            foundGroups = groups;
            if (number != null)
                foundGroups = foundGroups.Where(p => p.Number == number);//GetGroupNumber(Semester.Today()) == number);
            if (yearOfAdmission != null)
                foundGroups = foundGroups.Where(p => p.YearOfAdmission == yearOfAdmission);
            if (faculty != "")
                foundGroups = foundGroups.Where(p => p.Faculty.Contains(faculty)).ToList();// == faculty);
            if (degree != "")
                foundGroups = foundGroups.Where(p => p.Degree.Contains(degree)).ToList();// == degree);
            if (formOfStudy != "")
                foundGroups = foundGroups.Where(p => p.FormOfStudy.Contains(formOfStudy)).ToList();// == formOfStudy);

            return PartialView(foundGroups);
        }
        [HttpGet]
        public ActionResult AddGroups()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddGroups([Bind(Include = "Id,Number,YearOfAdmission,Faculty,Degree,FormOfStudy,MonitorId")]Groups groups)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                if (ModelState.IsValid)
                {
                    db.Groups.Add(groups);
                    db.SaveChanges();
                    return RedirectToAction("Groups");
                }
            }
            ViewBag.ErrorMessage = "Неверный ввод данных";
            return View(groups);
        }
        [HttpGet]
        public ActionResult EditGroups(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Groups group;
            using (DiaryConnection db = new DiaryConnection())
            {
                group = db.Groups.Find(id);
                if (group != null)
                {
                    var students = db.Students.Where(p => p.GroupId == group.Id).ToList();
                    ViewBag.Students = new SelectList(students, "Id", "FullName",group.Students);
                    db.Dispose();
                    return View(group);
                }
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult EditGroups([Bind(Include = "Id,Number,YearOfAdmission,Faculty,Degree,FormOfStudy,MonitorId")]Groups group)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(group).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var students = db.Students.Where(p => p.GroupId == group.Id);
                    ViewBag.Students = new SelectList(students, "Id", "FullName", group.Students);
                    db.Dispose();
                    return View(group);
                }
            }
            return RedirectToAction("Groups");
        }
        [HttpGet]
        public ActionResult DeleteGroups(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Groups group;
            using (DiaryConnection db = new DiaryConnection())
                group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = id;
            return View(group);
        }

        [HttpPost, ActionName("DeleteGroups")]
        public ActionResult DeleteGroupConfirmed(int id)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                Groups group = db.Groups.Find(id);
                if (group == null)
                {
                    return HttpNotFound();
                }
                db.Groups.Remove(group);
                db.SaveChanges();
            }
            return RedirectToAction("Groups");
        }
        public ActionResult Lecturers()
        {
            List<Teachers> teachers;
            DiaryConnection db = new DiaryConnection();
            teachers = db.Teachers.ToList();
            return View(teachers);
            //return View();
        }
        [HttpPost]
        public ActionResult LecturersSearch(string fullName, string departament)
        {
            IEnumerable<Teachers> foundLecturers;
            List<Teachers> teachers;
            using (DiaryConnection db = new DiaryConnection())
            {
                teachers = db.Teachers.ToList();
            }
            foundLecturers = teachers;
            if (fullName != "")
                foundLecturers = foundLecturers.Where(p => p.FullName.Contains(fullName)).ToList();// == number);
            if (departament != "")
                foundLecturers = foundLecturers.Where(p => p.Departament.Contains(departament)).ToList();// == fullName);

            return PartialView(foundLecturers);
            //List<Teachers> foundLecturers = null;
            //using (DiaryConnection db = new DiaryConnection())
            //{
            //    if (fullName != "" && departament == "")
            //        foundLecturers = db.Teachers.Where(p => p.FullName.Contains(fullName)).ToList();
            //    if (departament != "" && fullName == "")
            //        foundLecturers = db.Teachers.Where(p => p.Departament == departament).ToList();
            //    if (departament != "" && fullName != "")
            //        foundLecturers = db.Teachers.Where(p => p.Departament == departament && p.FullName.Contains(fullName)).ToList();
            //}
            //return PartialView(foundLecturers);
        }
        [HttpGet]
        public ActionResult AddLecturers()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddLecturers([Bind(Include = "Id,FullName,Departament")]Teachers lecturer)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                if (ModelState.IsValid)
                {
                    db.Teachers.Add(lecturer);
                    db.SaveChanges();
                    return RedirectToAction("AddLecturers");
                }
            }
            return View(lecturer);
        }
        [HttpGet]
        public ActionResult EditLecturers(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Teachers lecturer;
            using (DiaryConnection db = new DiaryConnection())
                lecturer = db.Teachers.Find(id);
            if (lecturer != null)
            {
                return View(lecturer);
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult EditLecturers([Bind(Include = "Id,FullName,Departament")]Teachers lecturer)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(lecturer).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Lecturers");
                }          
            }
            return View(lecturer);           
        }
        public ActionResult ShowStudents(int? id)
        {
            Groups group;
            List<Students> students;
            if (id == null)
                return HttpNotFound();
            using (DiaryConnection db = new DiaryConnection())
            {
                group = db.Groups.Find(id);
                if (group == null)
                    return HttpNotFound();
                students = group.Students1.ToList();
            }
            return View(students);
        }
        [HttpGet]
        public ActionResult DeleteLecturer(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Teachers lecturer;
            using (DiaryConnection db = new DiaryConnection())
                lecturer = db.Teachers.Find(id);
            if (lecturer == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = id;
            return View(lecturer);
        }

        [HttpPost, ActionName("DeleteLecturer")]
        public ActionResult DeleteLecturerConfirmed(int id)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                Teachers lecturer = db.Teachers.Find(id);
                if (lecturer == null)
                {
                    return HttpNotFound();
                }
                db.Teachers.Remove(lecturer);
                db.SaveChanges();
            }
            return RedirectToAction("Lecturers");
        }
        public ActionResult LecturersJournal(int? id)
        {
            if (id == null)
                return HttpNotFound();
            Teachers Lecturer;
            LecturerViewModel viewModel = new LecturerViewModel();
            using (DiaryConnection db = new DiaryConnection())
            {
                //Находим семестер, группу и предмет для запроса.
                //Вначале находим самый близкий к сегодняшней дате семестер
                var TGS = db.TeachersGroupsSubjects.Where(p => p.TeacherId == id);
                List<Semester> founsSemesters = new List<Semester>();
                //Находим все семестры в которые преподавал преподаватель
                foreach (var item in TGS)
                {
                    var tables = item.TableOfGrades;
                    foreach (var table in tables)
                    {
                        founsSemesters.Add(table.Semester);
                    }
                }
                founsSemesters = founsSemesters.Distinct().ToList();
                //Находим самый близкий к сегодняшней дате семестер
                Semester youngestSemester = founsSemesters.First();
                foreach (var semester in founsSemesters)
                {
                    if (semester.BeginningDate > youngestSemester.BeginningDate)
                        youngestSemester = youngestSemester;
                }
                //Получаем первую попавшиеся группу и предмет для найденного семестра.
                Groups group = youngestSemester.TableOfGrades.First().TeachersGroupsSubjects.Groups;
                Subjects subject = youngestSemester.TableOfGrades.First().TeachersGroupsSubjects.Subjects;
                //Заполняем модель представления.
                viewModel.GetAllFinalGrades(db, group.Id, id.Value, subject.Id, youngestSemester);
                //Заполнения модели представления
                //Получение название предметов для журнала
                viewModel.GetNamesOfStudents(db, group.Id, id.Value, subject.Id, youngestSemester);
                //Получение данных для заполнения области таблицы с оценками
                viewModel.GetStudentsGrades(db, group.Id, id.Value, subject.Id, youngestSemester);
                //Для строки с названиями месяцев
                viewModel.GetMounthNames();
                //С количеством дней в месяцах для для colspan.
                viewModel.GetDaysMounth();
                //Для отображение строки где день месяца + день недели.
                viewModel.GetDaysSemester();
                //Для формирование таблиц с итоговыми оценками
                viewModel.GetAllFinalGrades(db, group.Id, id.Value, subject.Id, youngestSemester);
            }
            return View(viewModel);
        }
        [HttpGet]
        public ActionResult Subjects()
        {
            List<Subjects> subjects;
            DiaryConnection db = new DiaryConnection();
            subjects = db.Subjects.ToList();
            return View(subjects);
            //return View();
        }
        [HttpPost]
        public ActionResult SubjectsSearch(string name, string departament)
        {
            IEnumerable<Subjects> foundSubjects;
            List<Subjects> subjects;
            using (DiaryConnection db = new DiaryConnection())
            {
                subjects = db.Subjects.ToList();
            }
            foundSubjects = subjects;
            if (name != "")
                foundSubjects = foundSubjects.Where(p => p.Name.Contains(name)).ToList();// == number);
            if (departament != "")
                foundSubjects = foundSubjects.Where(p => p.Departament.Contains(departament)).ToList();// == fullName);
            
            return PartialView(foundSubjects);

            //List<Subjects> foundSubjects = null;
            //using (DiaryConnection db = new DiaryConnection())
            //{
            //    if (name != "" && departament == "")
            //        foundSubjects = db.Subjects.Where(p => p.Name.Contains(name)).ToList();
            //    if (departament != "" && name == "")
            //        foundSubjects = db.Subjects.Where(p => p.Departament == departament).ToList();
            //    if (departament != "" && name != "")
            //        foundSubjects = db.Subjects.Where(p => p.Departament == departament && p.Name.Contains(name)).ToList();
            //}
            //return PartialView(foundSubjects);
        }
        [HttpGet]
        public ActionResult AddSubjects()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddSubjects([Bind(Include = "Id,Name,Departament")]Subjects subject)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                if (ModelState.IsValid)
                {
                    db.Subjects.Add(subject);
                    db.SaveChanges();
                    return RedirectToAction("Subjects");
                }
            }
            return View(subject);
        }
        [HttpGet]
        public ActionResult EditSubjects(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Subjects subject;
            using (DiaryConnection db = new DiaryConnection())
                subject = db.Subjects.Find(id);
            if (subject != null)
            {
                return View(subject);
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult EditSubjects([Bind(Include = "Id,Name,Departament")]Subjects subject)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(subject).State = EntityState.Modified;
                    db.SaveChanges();
                    //RedirectToAction("Subjects");
                }
            }
            return RedirectToAction("Subjects");//View(subject);
        }
        [HttpGet]
        public ActionResult DeleteSubjects(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Subjects subject;
            using (DiaryConnection db = new DiaryConnection())
                subject = db.Subjects.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = id;
            return View(subject);
        }

        [HttpPost, ActionName("DeleteSubjects")]
        public ActionResult DeleteSubjectsConfirmed(int id)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                Subjects subject = db.Subjects.Find(id);
                if (subject == null)
                {
                    return HttpNotFound();
                }
                db.Subjects.Remove(subject);
                db.SaveChanges();
            }
            return RedirectToAction("Subjects");
        }
        [HttpGet]
        public ActionResult Semesters()
        {
            List<Semester> semester;
            DiaryConnection db = new DiaryConnection();
            semester = db.Semester.ToList();
            return View(semester);
        }
        [HttpPost]
        public ActionResult SemestersSearch(string year, string number)
        {
            //List<Semester> foundSemesters = null;
            //using (DiaryConnection db = new DiaryConnection())
            //{
            //    if (year != "" && number == "")
            //        foundSemesters = db.Semester.Where(p => p.Year == year).ToList();
            //    if (number != "" && year == "")
            //        foundSemesters = db.Semester.Where(p => p.Number == number).ToList();
            //    if (year != "" && number != "")
            //        foundSemesters = db.Semester.Where(p => p.Number == number && p.Year == year).ToList();

            //}
            //return PartialView(foundSemesters);
            IEnumerable<Semester> foundSemesters;
            List<Semester> semester;
            using (DiaryConnection db = new DiaryConnection())
            {
                semester = db.Semester.ToList();
            }
            foundSemesters = semester;
            if (number != "")
                foundSemesters = foundSemesters.Where(p => p.Number == number);
            if (year != "")
                foundSemesters = foundSemesters.Where(p => p.Year == year);
            return PartialView(foundSemesters);
        }
        [HttpGet]
        public ActionResult AddSemester()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddSemester([Bind(Include = "Id,BeginningDate,EndDate,Number,Year")]Semester semester)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                if (ModelState.IsValid)
                {
                    db.Semester.Add(semester);
                    db.SaveChanges();
                    return RedirectToAction("Semesters");
                }
            }
            return View(semester);
        }
        [HttpGet]
        public ActionResult EditSemester(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Semester semester;
            using (DiaryConnection db = new DiaryConnection())
                semester = db.Semester.Find(id);
            if (semester != null)
            {
                return View(semester);
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult EditSemester([Bind(Include = "Id,BeginningDate,EndDate,Number,Year")]Semester semester)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(semester).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Semesters");
                }
            }
            return View(semester);
        }
        [HttpGet]
        public ActionResult DeleteSemester(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Semester semester;
            using (DiaryConnection db = new DiaryConnection())
                semester = db.Semester.Find(id);
            if (semester == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = id;
            return View(semester);
        }

        [HttpPost, ActionName("DeleteSemester")]
        public ActionResult DeleteSemesterConfirmed(int id)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                Semester semester = db.Semester.Find(id);
                if (semester == null)
                {
                    return HttpNotFound();
                }
                db.Semester.Remove(semester);
                db.SaveChanges();
            }
            return RedirectToAction("Semesters");
        }
        [HttpGet]
        public ActionResult LecturerGroupSubject()
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                var groups = db.Groups.ToList();
                foreach (var b in groups)
                {
                    b.Number = b.GetGroupNumber(Semester.Today());                   
                }
                ViewBag.Groups = new SelectList(groups, "Id", "Number");
                ViewBag.Lecturers = new SelectList(db.Teachers.ToList(), "Id", "FullName");
                ViewBag.Subjects = new SelectList(db.Subjects.ToList(), "Id", "Name");
            }
            return View();
        }
        [HttpPost]
        public ActionResult LecturerGroupSubjectSearch(int? groupId, int? subjectId, int? teacherId)
        {
            List<TeachersGroupsSubjects> foundLGS = null;
            DiaryConnection db = new DiaryConnection();

                foundLGS = db.TeachersGroupsSubjects.ToList();
                if (groupId != null)
                    foundLGS = foundLGS.Where(p => p.GroupId == groupId).ToList();
                if (subjectId != null)
                    foundLGS = foundLGS.Where(p => p.SubjectId == subjectId).ToList();
                if(teacherId != null)
                    foundLGS = foundLGS.Where(p => p.TeacherId == teacherId).ToList();
            if (groupId == null && subjectId == null && teacherId == null)
                foundLGS = null;      
            return PartialView(foundLGS);
        }
        [HttpGet]
        public ActionResult AddLecturerGroupSubject()
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                var groups = db.Groups.ToList();
                foreach (var b in groups)
                {
                    b.Number = b.GetGroupNumber(Semester.Today());
                }
                ViewBag.Groups = new SelectList(groups, "Id", "Number");
                ViewBag.Lecturers = new SelectList(db.Teachers.ToList(), "Id", "FullName");
                ViewBag.Subjects = new SelectList(db.Subjects.ToList(), "Id", "Name");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddLecturerGroupSubject([Bind(Include = "Id,GroupId,TeacherId,SubjectId")]TeachersGroupsSubjects lgs)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                if (ModelState.IsValid)
                {
                    db.TeachersGroupsSubjects.Add(lgs);
                    db.SaveChanges();
                    return RedirectToAction("LecturerGroupSubject");
                }
                var groups = db.Groups.ToList();
                foreach (var b in groups)
                {
                    b.Number = b.GetGroupNumber(Semester.Today());
                }
                ViewBag.Groups = new SelectList(groups, "Id", "Number");
                ViewBag.Lecturers = new SelectList(db.Teachers.ToList(), "Id", "FullName");
                ViewBag.Subjects = new SelectList(db.Subjects.ToList(), "Id", "Name");
            }
            return View(lgs);
        }
        [HttpGet]
        public ActionResult EditLecturerGroupSubject(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            TeachersGroupsSubjects lgs;
            DiaryConnection db = new DiaryConnection();
            lgs = db.TeachersGroupsSubjects.Find(id);
            if (lgs != null)
            {
                var groups = db.Groups.ToList();
                foreach (var b in groups)
                {
                    b.Number = b.GetGroupNumber(Semester.Today());
                }
                ViewBag.Groups = new SelectList(groups, "Id", "Number",lgs.Groups);
                ViewBag.Lecturers = new SelectList(db.Teachers.ToList(), "Id", "FullName",lgs.Teachers);
                ViewBag.Subjects = new SelectList(db.Subjects.ToList(), "Id", "Name",lgs.Subjects);
                db.Dispose();
                return View(lgs);
            }
            db.Dispose();
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult EditLecturerGroupSubject([Bind(Include = "Id,GroupId,TeacherId,SubjectId")]TeachersGroupsSubjects lgs)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                if (ModelState.IsValid)
                {
                    db.Entry(lgs).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Semesters");
                }
                else
                {
                    var groups = db.Groups.ToList();
                    foreach (var b in groups)
                    {
                        b.Number = b.GetGroupNumber(Semester.Today());
                    }
                    ViewBag.Groups = new SelectList(groups, "Id", "Number", lgs.Groups);
                    ViewBag.Lecturers = new SelectList(db.Teachers.ToList(), "Id", "FullName", lgs.Teachers);
                    ViewBag.Subjects = new SelectList(db.Subjects.ToList(), "Id", "Name", lgs.Subjects);
                }
            }
            return View(lgs);
        }
        [HttpGet]
        public ActionResult DeleteLecturerGroupSubject(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            TeachersGroupsSubjects lgs;
            DiaryConnection db = new DiaryConnection();
            lgs = db.TeachersGroupsSubjects.Find(id);
            if (lgs == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = id;
            return View(lgs);
        }

        [HttpPost, ActionName("DeleteLecturerGroupSubject")]
        public ActionResult DeleteLecturerGroupSubjectConfirmed(int id)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                TeachersGroupsSubjects lgs = db.TeachersGroupsSubjects.Find(id);
                if (lgs == null)
                {
                    return HttpNotFound();
                }
                db.TeachersGroupsSubjects.Remove(lgs);
                db.SaveChanges();
            }
            return RedirectToAction("LecturerGroupSubject");
        }
        public int GetActualGroupNumber(Groups group, int year)
        {
            if (year > group.YearOfAdmission)
            {
                while (year != group.YearOfAdmission)
                {
                    group.Number= group.Number +100;
                    group.YearOfAdmission++;
                }
            }
            return group.Number;
        }


        //регистрация
        public ActionResult ApplicationsForRegistration()
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                var groups = db.Groups.ToList();
                foreach (var b in groups)
                {
                    b.Number = b.GetGroupNumber(Semester.Today());
                }
                Groups nullGroup = new Groups();
                ViewBag.Groups = new SelectList(groups, "Id", "Number", new Groups());
            }
            return View();
        }
        public ActionResult ApplicationsForRegistrationSearch(string firstName, string secondName, string middleName, string number, int? group)
        {
            List<ApplicationsForRegistration> appl = null;
            DiaryConnection db = new DiaryConnection();
            if (group == null)
                appl = db.ApplicationsForRegistration.Where(p => p.FirstName.Contains(firstName) && p.SecondName.Contains(secondName) && p.CardNumber.Contains(number) && p.MiddleName.Contains(middleName) && p.CardNumber.Contains(number)).ToList();
            else
                appl = db.ApplicationsForRegistration.Where(p => p.FirstName.Contains(firstName) && p.SecondName.Contains(secondName) && p.CardNumber.Contains(number) && p.MiddleName.Contains(middleName) && p.CardNumber.Contains(number) && p.GroupId == group).ToList();
            return View(appl);
        }
        public ActionResult DeleteApplicationsForRegistration(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            ApplicationsForRegistration application;
            DiaryConnection db = new DiaryConnection();
            application = db.ApplicationsForRegistration.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = id;
            return View(application);
        }
        [HttpPost, ActionName("DeleteApplicationsForRegistration")]
        public ActionResult DeleteApplicationsForRegistrationConfirmed(int id)
        {
            using (DiaryConnection db = new DiaryConnection())
            {
                ApplicationsForRegistration application = db.ApplicationsForRegistration.Find(id);
                if (application == null)
                {
                    return HttpNotFound();
                }
                db.ApplicationsForRegistration.Remove(application);
                db.SaveChanges();
            }
            return RedirectToAction("ApplicationsForRegistration");
        }
        [HttpGet]
        public ActionResult ConfirmStudentRegistration(int? id)
        {
            if (id == null)
                return HttpNotFound();
            ApplicationsForRegistration application;
            DiaryConnection db = new DiaryConnection();
            application = db.ApplicationsForRegistration.Find(id);
            ViewBag.FoundStudents = db.Students.Where(p => (p.FullName.Contains(application.FirstName) && p.FullName.Contains(application.SecondName)) || p.Number == application.CardNumber).ToList();
            return View(application);
        }
        [HttpPost]
        public ActionResult StudentsSearchForApplication(string number, string fullName, string groupNumber, int applicationId)
        {
            List<Students> foundStudents;
            using (DiaryConnection db = new DiaryConnection())
            {
                if (number == "" && fullName == "" && groupNumber == "")
                    foundStudents = db.Students.ToList();
                else if (fullName == "" && groupNumber == "")
                    foundStudents = db.Students.Where(p => p.Number == number).ToList();
                else if (number == "" && groupNumber == "")
                    foundStudents = db.Students.Where(p => p.FullName.Contains(fullName)).ToList();
                else if (number == "" && fullName == "")
                {
                    var students = db.Students.ToList();
                    foundStudents = students.Where(p => p.Groups1.GetGroupNumber(Semester.Today()) == Convert.ToInt32(groupNumber)).ToList();
                }
                else if (groupNumber == null)
                    foundStudents = db.Students.Where(p => p.Number == number && p.FullName.Contains(fullName)).ToList();
                else if (number == "")
                    foundStudents = db.Students.Where(p => p.FullName.Contains(fullName)).ToList().
                        Where(p => p.Groups1.GetGroupNumber(Semester.Today()) == Convert.ToInt32(groupNumber)).ToList();
                else if (fullName == "")
                    foundStudents = db.Students.Where(p => p.Number == number).ToList().
                        Where(p => p.Groups1.GetGroupNumber(Semester.Today()) == Convert.ToInt32(groupNumber)).ToList();
                else
                {
                    foundStudents = db.Students.Where(p => p.Number == number && p.FullName.Contains(fullName)).ToList().
                        Where(p => p.Groups1.GetGroupNumber(Semester.Today()) == Convert.ToInt32(groupNumber)).ToList();
                }
                foundStudents.RemoveAll(p => p.UserName != null);
            }
            ViewBag.ApplicationId = applicationId;
            return PartialView(foundStudents);
        }
        public ActionResult CompletionStudentRegistration(int? applicationId, int? studentId)
        {
            if (applicationId == null || studentId == null)
                return HttpNotFound();
            DiaryConnection db = new DiaryConnection();
            var application = db.ApplicationsForRegistration.Find(applicationId);
            ViewBag.FoundStudents = db.Students.Where(p => (p.FullName.Contains(application.FirstName) && p.FullName.Contains(application.SecondName)) || p.Number == application.CardNumber);
            var student = db.Students.Find(studentId);
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var studentAccount = new ApplicationUser { Email = application.Email, UserName = application.Login };
            var result = userManager.Create(studentAccount, application.Password);
            if (result.Succeeded)
            {
                userManager.AddToRole(studentAccount.Id, "student");
                student.UserName = application.Login;
                db.ApplicationsForRegistration.Remove(application);
                db.SaveChanges();
            }
            return RedirectToAction("ApplicationsForRegistration");
        }
    }
}
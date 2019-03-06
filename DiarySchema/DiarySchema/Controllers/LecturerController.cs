using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web.Mvc;
using DiarySchema.Models;
using System.Data.Entity.Core;

namespace DiarySchema.Controllers
{
    public class LecturerController : Controller
    {
        DiaryConnection db = new DiaryConnection();

        public ActionResult GroupSubjectTest()
        {
            string userName = HttpContext.User.Identity.Name;
            var lecturerId = db.Teachers.Where(p => p.UserName == userName).First().Id;
            var toDaySemester = Semester.Today();
            var findGroups = (from p in db.Groups
                              join b in db.TeachersGroupsSubjects on p.Id equals b.GroupId
                              join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
                              join d in db.Semester on c.SemesterId equals d.Id
                              where b.TeacherId == lecturerId && d.Number == toDaySemester.Number &&
                              d.Year == toDaySemester.Year
                              select new
                              {
                                  Id = p.Id,
                                  Number = p.Number,
                                  YearOfAdmission = p.YearOfAdmission,
                                  Faculty = p.Faculty,
                                  Degree = p.Degree,
                                  MonitorId = p.MonitorId
                              }).ToList();
            List<Groups> groups = new List<Groups>();
            foreach (var group in findGroups)
            {
                if (groups.Find(p => p.Id == group.Id) == null)
                    groups.Add(new Groups
                    {
                        Id = group.Id,
                        Number = group.Number,
                        YearOfAdmission = group.YearOfAdmission,
                        Faculty = group.Faculty,
                        Degree = group.Degree,
                        MonitorId = group.MonitorId
                    });
            }

            foreach (var b in groups)
            {
                b.Number = b.GetGroupNumber(Semester.Today());

            }
            groups.OrderBy(p => p.Number);
            ViewBag.Groups = new SelectList(groups, "Id", "Number", 1);
            var firstGroup = groups.First();
            var subjects = (from p in db.Subjects
                            join b in db.TeachersGroupsSubjects on p.Id equals b.SubjectId
                            join f in db.Groups on b.GroupId equals f.Id
                            join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
                            join d in db.Semester on c.SemesterId equals d.Id
                            where b.TeacherId == lecturerId && d.Number == toDaySemester.Number && d.Year == toDaySemester.Year && f.Id == firstGroup.Id
                            select new
                            {
                                Name = p.Name,
                                Id = p.Id
                            }).ToList();
            subjects = subjects.Distinct().ToList();
            subjects.OrderBy(p => p.Name);
            SelectList subject = new SelectList(subjects, "Id", "Name");
            ViewBag.Subject = subject;
            return View();
        }
        
        public ActionResult GetItems(int id)
        {
            var subjects = (from p in db.Subjects
                            join b in db.TeachersGroupsSubjects on p.Id equals b.SubjectId
                            where b.GroupId == id
                            select new
                            {
                                Name = p.Name,
                                Id = p.Id
                            });
            ViewBag.Subject = subjects.Distinct().ToList();
            SelectList subject = new SelectList(subjects, "Id", "Name");
            ViewBag.Subject = subject;
            return View();            
        }
        // GET: Lecturer
        //DiaryConnection db = new DiaryConnection();
        public ActionResult Index(int? group, int? subject)
        {
            string userName = HttpContext.User.Identity.Name;
            var lecturerId = db.Teachers.Where(p => p.UserName == userName).First().Id;
            var toDaySemester = Semester.Today();
            var findGroups = (from p in db.Groups
                              join b in db.TeachersGroupsSubjects on p.Id equals b.GroupId
                              join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
                              join d in db.Semester on c.SemesterId equals d.Id
                              where b.TeacherId == lecturerId && d.Number == toDaySemester.Number &&
                              d.Year == toDaySemester.Year
                              select new
                              {
                                  Id = p.Id,
                                  Number = p.Number,
                                  YearOfAdmission = p.YearOfAdmission,
                                  Faculty = p.Faculty,
                                  Degree = p.Degree,
                                  MonitorId = p.MonitorId
                              }).ToList();
            List<Groups> groups = new List<Groups>();
            foreach (var group1 in findGroups)
            {
                if (groups.Find(p => p.Id == group1.Id) == null)
                    groups.Add(new Groups
                    {
                        Id = group1.Id,
                        Number = group1.Number,
                        YearOfAdmission = group1.YearOfAdmission,
                        Faculty = group1.Faculty,
                        Degree = group1.Degree,
                        MonitorId = group1.MonitorId
                    });
            }

            foreach (var b in groups)
            {
                b.Number = b.GetGroupNumber(Semester.Today());

            }
            groups.OrderBy(p => p.Number);
            int firstGroup;
            if (group == null)
                firstGroup = groups.FirstOrDefault().Id;
            else
                firstGroup = group.Value;
            ViewBag.Groups = new SelectList(groups, "Id", "Number", groups.First(p => p.Id == firstGroup));

            var subjects = (from p in db.Subjects
                            join b in db.TeachersGroupsSubjects on p.Id equals b.SubjectId
                            join f in db.Groups on b.GroupId equals f.Id
                            join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
                            join d in db.Semester on c.SemesterId equals d.Id
                            where b.TeacherId == lecturerId && d.Number == toDaySemester.Number && d.Year == toDaySemester.Year && f.Id == firstGroup
                            select new
                            {
                                Name = p.Name,
                                Id = p.Id
                            }).ToList();
            subjects = subjects.Distinct().ToList();
            subjects.OrderBy(p => p.Name);
            if (subject == null)
                subject = subjects.First().Id;
            ViewBag.Subject = new SelectList(subjects, "Id", "Name", subjects.First(p => p.Id == subject));
            LecturerViewModel viewModel = new LecturerViewModel();
            var toDaySemester1 = GetTodaySemester();
            int lecId = db.Teachers.Where(p => p.UserName == userName).First().Id;
            viewModel.GetAllFinalGrades(db, firstGroup, lecId, subject.Value, toDaySemester1);
            //Заполнения модели представления 
            //Получение название предметов для журнала 
            viewModel.GetNamesOfStudents(db, firstGroup, lecId, subject.Value, toDaySemester1);
            //Получение данных для заполнения области таблицы с оценками 
            viewModel.GetStudentsGrades(db, firstGroup, lecId, subject.Value, toDaySemester1);
            //Для строки с названиями месяцев 
            viewModel.GetMounthNames();
            //С количеством дней в месяцах для для colspan. 
            viewModel.GetDaysMounth();
            //Для отображение строки где день месяца + день недели. 
            viewModel.GetDaysSemester();
            //Для формирование таблиц с итоговыми оценками 
            viewModel.GetAllFinalGrades(db, firstGroup, lecId, subject.Value, toDaySemester1);
            viewModel.Subject = db.Subjects.Find(subject);
            viewModel.Group = db.Groups.Find(firstGroup);
            return View(viewModel);
        }
        //public ActionResult Index(int group, int subject)
        //{
        //    string userName = HttpContext.User.Identity.Name;
        //    var lecturerId = db.Teachers.Where(p => p.UserName == userName).First().Id;
        //    var toDaySemester = Semester.Today();
        //    var findGroups = (from p in db.Groups
        //                      join b in db.TeachersGroupsSubjects on p.Id equals b.GroupId
        //                      join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
        //                      join d in db.Semester on c.SemesterId equals d.Id
        //                      where b.TeacherId == lecturerId && d.Number == toDaySemester.Number &&
        //                      d.Year == toDaySemester.Year
        //                      select new
        //                      {
        //                          Id = p.Id,
        //                          Number = p.Number,
        //                          YearOfAdmission = p.YearOfAdmission,
        //                          Faculty = p.Faculty,
        //                          Degree = p.Degree,
        //                          MonitorId = p.MonitorId
        //                      }).ToList();
        //    List<Groups> groups = new List<Groups>();
        //    foreach (var group1 in findGroups)
        //    {
        //        if (groups.Find(p => p.Id == group1.Id) == null)
        //            groups.Add(new Groups
        //            {
        //                Id = group1.Id,
        //                Number = group1.Number,
        //                YearOfAdmission = group1.YearOfAdmission,
        //                Faculty = group1.Faculty,
        //                Degree = group1.Degree,
        //                MonitorId = group1.MonitorId
        //            });
        //    }

        //    foreach (var b in groups)
        //    {
        //        b.Number = b.GetGroupNumber(Semester.Today());

        //    }
        //    groups.OrderBy(p => p.Number);
        //    ViewBag.Groups = new SelectList(groups, "Id", "Number", 1);            
        //    var firstGroup = groups.First();
        //    var subjects = (from p in db.Subjects
        //                    join b in db.TeachersGroupsSubjects on p.Id equals b.SubjectId
        //                    join f in db.Groups on b.GroupId equals f.Id
        //                    join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
        //                    join d in db.Semester on c.SemesterId equals d.Id
        //                    where b.TeacherId == lecturerId && d.Number == toDaySemester.Number && d.Year == toDaySemester.Year && f.Id == firstGroup.Id
        //                    select new
        //                    {
        //                        Name = p.Name,
        //                        Id = p.Id
        //                    }).ToList();
        //    subjects = subjects.Distinct().ToList();
        //    subjects.OrderBy(p => p.Name);
        //    SelectList subject1 = new SelectList(subjects, "Id", "Name");
        //    ViewBag.Subject = subject1;



        //    LecturerViewModel viewModel = new LecturerViewModel();
        //    var toDaySemester1 = GetTodaySemester();            
        //    int lecId = db.Teachers.Where(p => p.UserName == userName).First().Id;
        //    viewModel.GetAllFinalGrades(db, group, lecId, subject, toDaySemester1);
        //    //Заполнения модели представления
        //    //Получение название предметов для журнала
        //    viewModel.GetNamesOfStudents(db, group, lecId, subject, toDaySemester1);
        //    //Получение данных для заполнения области таблицы с оценками
        //    viewModel.GetStudentsGrades(db, group, lecId, subject, toDaySemester1);
        //    //Для строки с названиями месяцев
        //    viewModel.GetMounthNames();
        //    //С количеством дней в месяцах для для colspan.
        //    viewModel.GetDaysMounth();
        //    //Для отображение строки где день месяца + день недели.
        //    viewModel.GetDaysSemester();
        //    //Для формирование таблиц с итоговыми оценками
        //    viewModel.GetAllFinalGrades(db, group, lecId, subject, toDaySemester1);
        //    viewModel.Subject = db.Subjects.Find(subject);
        //    viewModel.Group = db.Groups.Find(group);
        //    return View(viewModel);
        //}

        //public ActionResult GetJournal(int group, int subject)
        //{
        //    LecturerViewModel viewModel = new LecturerViewModel();
        //    var toDaySemester1 = GetTodaySemester();
        //    int lecId = 1;
        //    viewModel.GetAllFinalGrades(db, group, lecId, subject, toDaySemester1);
        //    //Заполнения модели представления
        //    //Получение название предметов для журнала
        //    viewModel.GetNamesOfStudents(db, group, lecId, subject, toDaySemester1);
        //    //Получение данных для заполнения области таблицы с оценками
        //    viewModel.GetStudentsGrades(db, group, lecId, subject, toDaySemester1);
        //    //Для строки с названиями месяцев
        //    viewModel.GetMounthNames();
        //    //С количеством дней в месяцах для для colspan.
        //    viewModel.GetDaysMounth();
        //    //Для отображение строки где день месяца + день недели.
        //    viewModel.GetDaysSemester();
        //    //Для формирование таблиц с итоговыми оценками
        //    viewModel.GetAllFinalGrades(db, group, lecId, subject, toDaySemester1);
        //    viewModel.Subject = db.Subjects.Find(subject);
        //    viewModel.Group = db.Groups.Find(group);
        //    return View(viewModel);
        //}


        [HttpPost]
        public ActionResult Index(LecturerViewModel viewModel, string action)
        {
            DiaryConnection db = new DiaryConnection();
            db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            //action - параметр отправляемый из кнопки "сохранить". В зависимости от него обновляеться нужная часть модели. 
            if (action == "Journal")
            {
                for (int i = 0; i < viewModel.ListOfGrades.Count; i++)
                {
                    for (int j = 0; j < viewModel.ListOfGrades[i].Count; j++)
                    {
                        //Обновляем отправленные данные таблицы 
                        var foundTableEntry = viewModel.ListOfGrades[i][j];
                        var tableEntry = db.TableEntry.Where(p => p.Id == foundTableEntry.Id).First();
                        tableEntry.Value = foundTableEntry.Value;

                    }
                }
            }
            if (action == "PassFailTest")
            {
                foreach (var b in viewModel.PassFailTestGrades)
                {
                    //Обновляем отправленные данные таблицы 
                    var tableEntry = db.TableEntry.Where(p => p.Id == b.Id).First();
                    tableEntry.Value = b.Value;
                }
            }
            if (action == "PracticalWork")
            {
                foreach (var b in viewModel.PracticalWorkGrades)
                {
                    var tableEntry = db.TableEntry.Where(p => p.Id == b.Id).First();
                    tableEntry.Value = b.Value;
                }
            }
            if (action == "CoursePaper")
            {
                foreach (var b in viewModel.CoursePaperGrades)
                {
                    var tableEntry = db.TableEntry.Where(p => p.Id == b.Id).First();
                    tableEntry.Value = b.Value;
                }
            }
            if (action == "Exam")
            {
                foreach (var b in viewModel.ExamGrades)
                {
                    var tableEntry = db.TableEntry.Where(p => p.Id == b.Id).First();
                    tableEntry.Value = b.Value;
                }
            }
            db.SaveChanges();
            return Redirect("/Lecturer/Index" + "/" + viewModel.Group.Id + "/" + viewModel.Subject.Id);
        }

        public ActionResult CreateJournal(int groupId, int subjectId)
        {
            ViewBag.GroupId = groupId;
            ViewBag.SubjectId = subjectId;
            return View();
        }

        [HttpPost]
        public ActionResult CreateJournal(List<bool> numerator, List<bool> denominator, int groupId, int subjectId)
        {
            string userName = HttpContext.User.Identity.Name;
            DiaryConnection db = new DiaryConnection();
            var lecId = db.Teachers.Where(p => p.UserName == userName).First().Id;
            var tgs = db.TeachersGroupsSubjects.Where(p => p.TeacherId == lecId &&
            p.GroupId == groupId && p.SubjectId == subjectId).First();
            var semester = Semester.Today();
            if (db.TableOfGrades.Any(p => p.TeachersGroupsSubjectId == tgs.Id && p.TypeOfKnowledgeControl == "Journal"))
                return Redirect("/Lecturer/Index" + "/" + groupId + "/" + subjectId);
            //var tgs = (from p in db.TeachersGroupsSubject where p.TeacherId == lecId /*&& p.GroupId == group1 && p.SubjectId == subject*/ select p).ToList();// db.TeachersGroupsSubject.Where(p => p.TeacherId == lecId && p.GroupId ==group &&p.SubjectId == subject).ToList().First(); 
            //Получаем все дни по расписанию в течении семестра. 
            var allDays = GetAllDaysAreOnTimetable(numerator, denominator, semester);
            //Получаем всех студентов у данной группы 
            var allStudents = (from p in db.Students
                               where p.GroupId == groupId
                               select new
                               { Id = p.Id }).ToList();/*db.Students.Where(p => p.GroupId == group1)*/
                                                       //Добавляем новую таблицу с оценками(TGSId должен быть параметром) 
            var newTable = new TableOfGrades
            {
                TeachersGroupsSubjectId = tgs.Id,
                TypeOfKnowledgeControl = "Journal",
                SemesterId = semester.Id
            };
            db.TableOfGrades.Add(newTable);
            db.SaveChanges();

            foreach (var day in allDays)
            {
                foreach (var std in allStudents)
                {
                    if (!db.TableEntry.Any(p => (p.StudentId == std.Id) && (p.Date == day) && (p.TableOfGradeId == newTable.Id)))
                        db.TableEntry.Add(new TableEntry { StudentId = std.Id, Date = day, TableOfGradeId = newTable.Id, Value = "" });
                }
            }
            db.SaveChanges();
            return Redirect("/Lecturer/Index" + "/" + groupId + "/" + subjectId);
        }


        public ActionResult CreateExamTable(int groupId, int subjectId)
        {
            ViewBag.GroupId = groupId;
            ViewBag.SubjectId = subjectId;
            return View();
        }


        [HttpPost]
        public ActionResult CreateExamTable(DateTime examDate, int groupId, int subjectId)
        {
            var semester = GetTodaySemester();
            string userName = HttpContext.User.Identity.Name;
            var lecId = db.Teachers.Where(p => p.UserName == userName).First().Id;
            var tgs = db.TeachersGroupsSubjects.Where(p => p.TeacherId == lecId &&
            p.GroupId == groupId && p.SubjectId == subjectId).First();
            if (db.TableOfGrades.Any(p => p.TeachersGroupsSubjectId == tgs.Id && p.TypeOfKnowledgeControl == "Exam"))
                return Redirect("/Lecturer/Index" + "/" + groupId + "/" + subjectId);
            //var semester = db.Semester.Where(p => p.Year == "2016/2017" && p.Number == "2").First(); 
            //var tgs = (from p in db.TeachersGroupsSubject where p.TeacherId == lecId /*&& p.GroupId == group1 && p.SubjectId == subject*/ select p).ToList();// db.TeachersGroupsSubject.Where(p => p.TeacherId == lecId && p.GroupId ==group &&p.SubjectId == subject).ToList().First(); 
            //Получаем всех студентов у данной группы 
            var allStudents = (from p in db.Students
                               where p.GroupId == groupId
                               select new
                               { Id = p.Id }).ToList();/*db.Students.Where(p => p.GroupId == group1)*/
                                                       //Добавляем новую таблицу с оценками(TGSId должен быть параметром) 
            db.TableOfGrades.Add(new TableOfGrades
            {
                TeachersGroupsSubjectId = tgs.Id,
                TypeOfKnowledgeControl = "Exam",
                SemesterId = semester.Id
            });
            db.SaveChanges();
            var table = db.TableOfGrades.Where(p => p.TeachersGroupsSubjectId == tgs.Id &&
            p.SemesterId == semester.Id && p.TypeOfKnowledgeControl == "Exam").First();
            foreach (var std in allStudents)
            {
                if (!db.TableEntry.Any(p => (p.StudentId == std.Id) && (p.Date == examDate) && (p.TableOfGradeId == table.Id)))
                    db.TableEntry.Add(new TableEntry { StudentId = std.Id, Date = examDate, TableOfGradeId = table.Id, Value = "" });

            }
            //Нужна валидация данных по дате как минимум 
            db.SaveChanges();
            return Redirect("/Lecturer/Index" + "/" + groupId + "/" + subjectId);
        }


        public ActionResult CreatePassFailTestTable(int groupId, int subjectId)
        {
            var semester = GetTodaySemester();
            string userName = HttpContext.User.Identity.Name;
            var lecId = db.Teachers.Where(p => p.UserName == userName).First().Id;
            string semesterNumber = "2";
            string year = "2016/2017";

            var tgs = db.TeachersGroupsSubjects.Where(p => p.TeacherId == lecId &&
            p.GroupId == groupId && p.SubjectId == subjectId).First();
            if (db.TableOfGrades.Any(p => p.TeachersGroupsSubjectId == tgs.Id && p.TypeOfKnowledgeControl == "PassFailTest"))
                return Redirect("/Lecturer/Index" + "/" + groupId + "/" + subjectId);
            //var semester = db.Semester.Where(p => p.Year == "2016/2017" && p.Number == "2").First(); 
            //var tgs = (from p in db.TeachersGroupsSubject where p.TeacherId == lecId /*&& p.GroupId == group1 && p.SubjectId == subject*/ select p).ToList();// db.TeachersGroupsSubject.Where(p => p.TeacherId == lecId && p.GroupId ==group &&p.SubjectId == subject).ToList().First(); 
            //Получаем всех студентов у данной группы 
            var allStudents = (from p in db.Students
                               where p.GroupId == groupId
                               select new
                               { Id = p.Id }).ToList();/*db.Students.Where(p => p.GroupId == group1)*/
                                                       //Добавляем новую таблицу с оценками(TGSId должен быть параметром) 
            db.TableOfGrades.Add(new TableOfGrades
            {
                TeachersGroupsSubjectId = tgs.Id,
                TypeOfKnowledgeControl = "PassFailTest",
                SemesterId = semester.Id
            });
            db.SaveChanges();
            var table = db.TableOfGrades.Where(p => p.TeachersGroupsSubjectId == tgs.Id &&
            p.SemesterId == semester.Id && p.TypeOfKnowledgeControl == "PassFailTest").First();
            foreach (var std in allStudents)
            {
                if (!db.TableEntry.Any(p => (p.StudentId == std.Id) && (p.TableOfGradeId == table.Id)))
                    db.TableEntry.Add(new TableEntry { StudentId = std.Id, TableOfGradeId = table.Id, Value = "" });

            }

            db.SaveChanges();
            return Redirect("/Lecturer/Index" + "/" + groupId + "/" + subjectId);
        }


        public ActionResult CreateCoursePaperTable(int groupId, int subjectId)
        {
            var semester = GetTodaySemester();
            string userName = HttpContext.User.Identity.Name;
            var lecId = db.Teachers.Where(p => p.UserName == userName).First().Id;
            var tgs = db.TeachersGroupsSubjects.Where(p => p.TeacherId == lecId &&
            p.GroupId == groupId && p.SubjectId == subjectId).First();
            if (db.TableOfGrades.Any(p => p.TeachersGroupsSubjectId == tgs.Id && p.TypeOfKnowledgeControl == "CoursePaper"))
                return Redirect("/Lecturer/Index" + "/" + groupId + "/" + subjectId);
            //var semester = db.Semester.Where(p => p.Year == "2016/2017" && p.Number == "2").First(); 
            //var tgs = (from p in db.TeachersGroupsSubject where p.TeacherId == lecId /*&& p.GroupId == group1 && p.SubjectId == subject*/ select p).ToList();// db.TeachersGroupsSubject.Where(p => p.TeacherId == lecId && p.GroupId ==group &&p.SubjectId == subject).ToList().First(); 
            //Получаем всех студентов у данной группы 
            var allStudents = (from p in db.Students
                               where p.GroupId == groupId
                               select new
                               { Id = p.Id }).ToList();/*db.Students.Where(p => p.GroupId == group1)*/
                                                       //Добавляем новую таблицу с оценками(TGSId должен быть параметром) 
            db.TableOfGrades.Add(new TableOfGrades
            {
                TeachersGroupsSubjectId = tgs.Id,
                TypeOfKnowledgeControl = "CoursePaper",
                SemesterId = semester.Id
            });
            db.SaveChanges();
            var table = db.TableOfGrades.Where(p => p.TeachersGroupsSubjectId == tgs.Id &&
            p.SemesterId == semester.Id && p.TypeOfKnowledgeControl == "CoursePaper").First();
            foreach (var std in allStudents)
            {
                if (!db.TableEntry.Any(p => (p.StudentId == std.Id) && (p.TableOfGradeId == table.Id)))
                    db.TableEntry.Add(new TableEntry { StudentId = std.Id, TableOfGradeId = table.Id, Value = "" });

            }
            db.SaveChanges();
            return Redirect("/Lecturer/Index" + "/" + groupId + "/" + subjectId);
        }

        public ActionResult CreatePracticalWorkTable(int groupId, int subjectId)
        {
            var semester = GetTodaySemester();
            string userName = HttpContext.User.Identity.Name;
            var lecId = db.Teachers.Where(p => p.UserName == userName).First().Id;
            var tgs = db.TeachersGroupsSubjects.Where(p => p.TeacherId == lecId &&
            p.GroupId == groupId && p.SubjectId == subjectId).First();
            if (db.TableOfGrades.Any(p => p.TeachersGroupsSubjectId == tgs.Id && p.TypeOfKnowledgeControl == "PracticalWork"))
                return Redirect("/Lecturer/Index" + "/" + groupId + "/" + subjectId);

            //var semester = db.Semester.Where(p => p.Year == "2016/2017" && p.Number == "2").First(); 
            //var tgs = (from p in db.TeachersGroupsSubject where p.TeacherId == lecId /*&& p.GroupId == group1 && p.SubjectId == subject*/ select p).ToList();// db.TeachersGroupsSubject.Where(p => p.TeacherId == lecId && p.GroupId ==group &&p.SubjectId == subject).ToList().First(); 
            //Получаем всех студентов у данной группы 
            var allStudents = (from p in db.Students
                               where p.GroupId == groupId
                               select new
                               { Id = p.Id }).ToList();/*db.Students.Where(p => p.GroupId == group1)*/
                                                       //Добавляем новую таблицу с оценками(TGSId должен быть параметром) 
            db.TableOfGrades.Add(new TableOfGrades
            {
                TeachersGroupsSubjectId = tgs.Id,
                TypeOfKnowledgeControl = "PracticalWork",
                SemesterId = semester.Id
            });
            db.SaveChanges();
            var table = db.TableOfGrades.Where(p => p.TeachersGroupsSubjectId == tgs.Id &&
            p.SemesterId == semester.Id && p.TypeOfKnowledgeControl == "PracticalWork").First();
            foreach (var std in allStudents)
            {
                if (!db.TableEntry.Any(p => (p.StudentId == std.Id) && (p.TableOfGradeId == table.Id)))
                    db.TableEntry.Add(new TableEntry { StudentId = std.Id, TableOfGradeId = table.Id, Value = "" });

            }

            db.SaveChanges();
            return Redirect("/Lecturer/Index" + "/" + groupId + "/" + subjectId);
        }

        private List<DateTime> GetAllDaysAreOnTimetable(List<bool> numerator, List<bool> denominator, Semester semester)
        {
            for (int i = 0; i < denominator.Count; i++)
            {
                if (denominator[i] == true)
                    denominator.RemoveAt(i + 1);

            }
            for (int i = 0; i < numerator.Count; i++)
            {
                if (numerator[i] == true)
                    numerator.RemoveAt(i + 1);
            }
            List<DateTime> allDays = new List<DateTime>();
            DateTime startSemestr = semester.BeginningDate;
            DateTime endSemestr = semester.EndDate;
            bool isNumerator = true;
            Dictionary<DayOfWeek, int> dayNumberInTheWeek = new Dictionary<DayOfWeek, int>(6);
            dayNumberInTheWeek.Add(DayOfWeek.Monday,0);
            dayNumberInTheWeek.Add(DayOfWeek.Tuesday, 1);
            dayNumberInTheWeek.Add(DayOfWeek.Wednesday, 2);
            dayNumberInTheWeek.Add(DayOfWeek.Thursday, 3);
            dayNumberInTheWeek.Add(DayOfWeek.Friday, 4);
            dayNumberInTheWeek.Add(DayOfWeek.Saturday, 5);
            dayNumberInTheWeek.Add(DayOfWeek.Sunday, 6);
            while (startSemestr <= endSemestr)
            {
                DateTime day;
                if (isNumerator)
                {
                    if (numerator[dayNumberInTheWeek[startSemestr.DayOfWeek]])
                    {
                        day = startSemestr;
                        allDays.Add(day);
                    }
                }
                else
                {
                    if (denominator[dayNumberInTheWeek[startSemestr.DayOfWeek]])
                    {
                        day = startSemestr;
                        allDays.Add(day);
                    }
                }
                startSemestr = startSemestr.AddDays(1);
            }
            return allDays;


        }
        private Semester GetTodaySemester()
        {
            DiaryConnection db = new DiaryConnection();
            //Надо дописать с обращением к базе данных
            var toDay = DateTime.Today;
            var semester = db.Semester.Where(p => p.BeginningDate < toDay && p.EndDate > toDay).First();
            return semester;
        }
        private string GetTodayYear()
        {
            //Надо дописать с обращением к базе данных
            return "2016/2017";
        }


        //вставлено через вк(внизу)
        public ActionResult SelectGroupAndSubject()
        {
            string userName = HttpContext.User.Identity.Name;
            var groups = db.Groups.ToList();
            foreach (var b in groups)
            {
                b.Number = b.GetGroupNumber(Semester.Today());
            }
            ViewBag.TGS = db.TeachersGroupsSubjects.Where(p => p.Teachers.UserName == userName).ToList();
            ViewBag.Lecturer = db.Teachers.Where(p => p.UserName == userName).First();
            ViewBag.Groups = new SelectList(groups, "Id", "Number");
            ViewBag.Subjects = new SelectList(db.Subjects.ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult SelectGroupAndSubject([Bind(Include = "Id,GroupId,TeacherId,SubjectId")]TeachersGroupsSubjects lgs, string isNewSubject, string newSubjectName, string newSubjectDepartament)
        {
            if (isNewSubject == "true")
            {
                if (db.Subjects.Where(p => p.Name == newSubjectName && p.Departament == newSubjectDepartament).ToList().Count == 0)
                {
                    db.Subjects.Add(new Subjects { Name = newSubjectName, Departament = newSubjectDepartament });
                    db.SaveChanges();
                }
                lgs.SubjectId = db.Subjects.Where(p => p.Name == newSubjectName && p.Departament == newSubjectDepartament).First().Id;
            }
            if (ModelState.IsValid)
            {
                db.TeachersGroupsSubjects.Add(lgs);
                db.SaveChanges();
                return Redirect("/Lecturer/Index" + "/" + lgs.GroupId + "/" + lgs.SubjectId);
            }
            string userName = HttpContext.User.Identity.Name;
            var lecId = db.Teachers.Where(p => p.UserName == userName).First().Id;
            var groups = db.Groups.ToList();
            foreach (var b in groups)
            {
                b.Number = b.GetGroupNumber(Semester.Today());
            }
            ViewBag.Lecturer = lecId;
            ViewBag.Groups = new SelectList(groups, "Id", "Number");
            ViewBag.Subjects = new SelectList(db.Subjects.ToList(), "Id", "Name");
            return View();
        }


        public ActionResult Archive(int? group, int? subject, string year, string semesterNumber)
        {            
            string userName = HttpContext.User.Identity.Name;
            var lecturerId = db.Teachers.Where(p => p.UserName == userName).First().Id;
            var findSemesters = (from p in db.Semester
                                 join c in db.TableOfGrades on p.Id equals c.SemesterId
                                 join g in db.TeachersGroupsSubjects on c.TeachersGroupsSubjectId equals g.Id
                                 join f in db.Teachers on g.TeacherId equals f.Id

                                 where f.Id == lecturerId
                                 select new
                                 {
                                     Id = p.Id,
                                     BeginningDate = p.BeginningDate,
                                     EndDate = p.EndDate,
                                     Number = p.Number,
                                     Year = p.Year
                                 }).Distinct();
            List<Semester> semesters = new List<Semester>();
            foreach (var item in findSemesters)
            {
                semesters.Add(new Semester { Id = item.Id, BeginningDate = item.BeginningDate, EndDate = item.EndDate, Number = item.Number, Year = item.Year });
            }
            Semester semester;
            if (year == null || semesterNumber == null)
                semester = semesters.FirstOrDefault();
            else
                semester = db.Semester.FirstOrDefault(p => p.Year == year && p.Number == semesterNumber);
            var findGroups = (from p in db.Groups
                              join b in db.TeachersGroupsSubjects on p.Id equals b.GroupId
                              join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
                              join d in db.Semester on c.SemesterId equals d.Id
                              where b.TeacherId == lecturerId && d.Number == semester.Number &&
                              d.Year == semester.Year
                              select new
                              {
                                  Id = p.Id,
                                  Number = p.Number,
                                  YearOfAdmission = p.YearOfAdmission,
                                  Faculty = p.Faculty,
                                  Degree = p.Degree,
                                  MonitorId = p.MonitorId
                              }).ToList();
            List<Groups> groups = new List<Groups>();
            foreach (var group1 in findGroups)
            {
                if (groups.Find(p => p.Id == group1.Id) == null)
                    groups.Add(new Groups
                    {
                        Id = group1.Id,
                        Number = group1.Number,
                        YearOfAdmission = group1.YearOfAdmission,
                        Faculty = group1.Faculty,
                        Degree = group1.Degree,
                        MonitorId = group1.MonitorId
                    });
            }

            foreach (var b in groups)
            {
                b.Number = b.GetGroupNumber(Semester.Today());

            }
            groups.OrderBy(p => p.Number);
            if (group == null)
                group = db.Groups.First().Id;
            var firstGroup = groups.First();
            var subjects = (from p in db.Subjects
                            join b in db.TeachersGroupsSubjects on p.Id equals b.SubjectId
                            join f in db.Groups on b.GroupId equals f.Id
                            join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
                            join d in db.Semester on c.SemesterId equals d.Id
                            where b.TeacherId == lecturerId && d.Number == semester.Number && d.Year == semester.Year && f.Id == firstGroup.Id
                            select new
                            {
                                Name = p.Name,
                                Id = p.Id
                            }).ToList();
            subjects = subjects.Distinct().ToList();
            subjects.OrderBy(p => p.Name);
            if (subject == null)
                subject = db.Subjects.First().Id;
            //SelectList для симестров надо делать вручную. У меня на представление есть пример. 
            ViewBag.Semesters = semesters;
            ViewBag.Groups = new SelectList(groups, "Id", "Number", groups.FirstOrDefault(p => p.Id == group));
            ViewBag.Subjects = new SelectList(subjects, "Id", "Name", subjects.FirstOrDefault(p => p.Id == subject));

            LecturerViewModel viewModel = new LecturerViewModel();
            viewModel.GetAllFinalGrades(db, group.Value, lecturerId, subject.Value, semester);
            //Заполнения модели представления 
            //Получение название предметов для журнала 
            viewModel.GetNamesOfStudents(db, group.Value, lecturerId, subject.Value, semester);
            //Получение данных для заполнения области таблицы с оценками 
            viewModel.GetStudentsGrades(db, group.Value, lecturerId, subject.Value, semester);
            //Для строки с названиями месяцев 
            viewModel.GetMounthNames();
            //С количеством дней в месяцах для для colspan. 
            viewModel.GetDaysMounth();
            //Для отображение строки где день месяца + день недели. 
            viewModel.GetDaysSemester();
            //Для формирование таблиц с итоговыми оценками 
            viewModel.GetAllFinalGrades(db, group.Value, lecturerId, subject.Value, semester);
            viewModel.Subject = db.Subjects.Find(subject);
            viewModel.Group = db.Groups.Find(group);
            return  View(viewModel);
        }


        //public ActionResult GetSubjext(int? semesterId, int? groupId)
        //{
        //    string userName = HttpContext.User.Identity.Name;
        //    var lecturerId = db.Teachers.Where(p => p.UserName == userName).First().Id;
        //    var subjects = (from p in db.Subjects
        //                    join b in db.TeachersGroupsSubjects on p.Id equals b.SubjectId
        //                    join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
        //                    join d in db.Semester on c.SemesterId equals d.Id
        //                   where b.TeacherId == lecturerId && d.Id == semesterId
        //                   //where b.GroupId == groupId && d.Id == semesterId
        //                   select new
        //                    {
        //                        Name = p.Name,
        //                        Id = p.Id
        //                    });
        //    ViewBag.Subject = subjects.Distinct().ToList();
        //    SelectList subject = new SelectList(subjects, "Id", "Name");
        //    ViewBag.Subject = subject;            
        //    return View();
        //}
       


        public ActionResult GetJournal(int groupId, int subjectId, int? semesterId)
        {
            if (semesterId == null)
                semesterId = Semester.Today().Id;
            Semester semester = db.Semester.First(p => p.Id == semesterId);
            LecturerViewModel viewModel = new LecturerViewModel();
            string userName = HttpContext.User.Identity.Name;
            var lecId = db.Teachers.Where(p => p.UserName == userName).First().Id;
            viewModel.GetAllFinalGrades(db, groupId, lecId, subjectId, semester);
            //Заполнения модели представления 
            //Получение название предметов для журнала 
            viewModel.GetNamesOfStudents(db, groupId, lecId, subjectId, semester);
            //Получение данных для заполнения области таблицы с оценками 
            viewModel.GetStudentsGrades(db, groupId, lecId, subjectId, semester);
            //Для строки с названиями месяцев 
            viewModel.GetMounthNames();
            //С количеством дней в месяцах для для colspan. 
            viewModel.GetDaysMounth();
            //Для отображение строки где день месяца + день недели. 
            viewModel.GetDaysSemester();
            //Для формирование таблиц с итоговыми оценками 
            viewModel.GetAllFinalGrades(db, groupId, lecId, subjectId, semester);
            viewModel.Subject = db.Subjects.Find(subjectId);
            viewModel.Group = db.Groups.Find(groupId);
            return View(viewModel);
        }
        public ActionResult GetGroups(int? id)
        {           
            string userName = HttpContext.User.Identity.Name;
            var lecturerId = db.Teachers.Where(p => p.UserName == userName).First().Id;
            var semester = db.Semester.Find(id);
            var findGroups = (from p in db.Groups
                              join b in db.TeachersGroupsSubjects on p.Id equals b.GroupId
                              join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
                              join d in db.Semester on c.SemesterId equals d.Id
                              where b.TeacherId == lecturerId && d.Number == semester.Number &&
                              d.Year == semester.Year
                              select new
                              {
                                  Id = p.Id,
                                  Number = p.Number,
                                  YearOfAdmission = p.YearOfAdmission,
                                  Faculty = p.Faculty,
                                  Degree = p.Degree,
                                  MonitorId = p.MonitorId
                              }).ToList();
            List<Groups> groups = new List<Groups>();
            foreach (var group1 in findGroups)
            {
                if (groups.Find(p => p.Id == group1.Id) == null)
                    groups.Add(new Groups
                    {
                        Id = group1.Id,
                        Number = group1.Number,
                        YearOfAdmission = group1.YearOfAdmission,
                        Faculty = group1.Faculty,
                        Degree = group1.Degree,
                        MonitorId = group1.MonitorId
                    });
            }

            foreach (var b in groups)
            {
                b.Number = b.GetGroupNumber(Semester.Today());

            }
            groups.OrderBy(p => p.Number);
            ViewBag.Groups = new SelectList(groups, "Id", "Number");
            return View();
        
        //string userName = HttpContext.User.Identity.Name;
        //var lecturerId = db.Teachers.Where(p => p.UserName == userName).First().Id;
        //if (id == null)
        //{
        //    ViewBag.IsIndex = true;
        //    id = Semester.Today().Id;
        //}
        //else
        //    ViewBag.IsIndex = false;
        //var findSemesters = (from p in db.Semester
        //                     join c in db.TableOfGrades on p.Id equals c.SemesterId
        //                     join g in db.TeachersGroupsSubjects on c.TeachersGroupsSubjectId equals g.Id
        //                     join f in db.Teachers on g.TeacherId equals f.Id

        //                     where f.Id == lecturerId
        //                     select new
        //                     {
        //                         Id = p.Id,
        //                         BeginningDate = p.BeginningDate,
        //                         EndDate = p.EndDate,
        //                         Number = p.Number,
        //                         Year = p.Year
        //                     }).Distinct();
        //List<Semester> semesters = new List<Semester>();
        //foreach (var item in findSemesters)
        //{
        //    semesters.Add(new Semester { Id = item.Id, BeginningDate = item.BeginningDate, EndDate = item.EndDate, Number = item.Number, Year = item.Year });
        //}
        //ViewBag.Semesters = semesters;
        //return View();
    }
        //public ActionResult GetSubjeсt( int groupId, int? semesterId)
        //{
        //    if (semesterId == null)
        //        semesterId = Semester.Today().Id;
        //    string userName = HttpContext.User.Identity.Name;
        //    var lecturerId = db.Teachers.Where(p => p.UserName == userName).First().Id;
        //    var subjects = (from p in db.Subjects
        //                    join b in db.TeachersGroupsSubjects on p.Id equals b.SubjectId
        //                    join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
        //                    join d in db.Semester on c.SemesterId equals d.Id
        //                    where b.TeacherId == lecturerId &&
        //                    b.GroupId == groupId && d.Id == semesterId
        //                    select new
        //                    {
        //                        Name = p.Name,
        //                        Id = p.Id
        //                    });
        //    ViewBag.Subject = subjects.Distinct().OrderBy(p => p.Name).ToList();
        //    SelectList subject = new SelectList(subjects, "Id", "Name");
        //    ViewBag.SubjectCount = subject.Count();
        //    ViewBag.Subject = subject;            
        //    return View();
        //}
        public ActionResult GetSubjeсt(int groupId, int? semesterId)
        {
            if (semesterId == null)
            {
                ViewBag.IsIndex = true;
                semesterId = Semester.Today().Id;
            }
            else
                ViewBag.IsIndex = false;
            string userName = HttpContext.User.Identity.Name;
            var lecturerId = db.Teachers.Where(p => p.UserName == userName).First().Id;
            var subjects = (from p in db.Subjects
                            join b in db.TeachersGroupsSubjects on p.Id equals b.SubjectId
                            join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
                            join d in db.Semester on c.SemesterId equals d.Id
                            where b.TeacherId == lecturerId &&
                            b.GroupId == groupId && d.Id == semesterId
                            select new
                            {
                                Name = p.Name,
                                Id = p.Id
                            });
            subjects = subjects.Distinct().OrderBy(p => p.Name);
            ViewBag.Subject = new SelectList(subjects, "Id", "Name");            
            ViewBag.SubjectCount = subjects.Count();
            return View();
        }
        //public ActionResult GetGroups(int id)
        //{
        //    string userName = HttpContext.User.Identity.Name;
        //    var lecturerId = db.Teachers.Where(p => p.UserName == userName).First().Id;
        //    var findGroups = (from p in db.Groups
        //                      join b in db.TeachersGroupsSubjects on p.Id equals b.GroupId
        //                      join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
        //                      join d in db.Semester on c.SemesterId equals d.Id
        //                      where b.TeacherId == lecturerId && d.Id == id
        //                      select new
        //                      {
        //                          Id = p.Id,
        //                          Number = p.Number,
        //                          YearOfAdmission = p.YearOfAdmission,
        //                          Faculty = p.Faculty,
        //                          Degree = p.Degree,
        //                          MonitorId = p.MonitorId
        //                      }).ToList();
        //    List<Groups> groups = new List<Groups>();
        //    foreach (var group1 in findGroups)
        //    {
        //        if (groups.Find(p => p.Id == group1.Id) == null)
        //            groups.Add(new Groups
        //            {
        //                Id = group1.Id,
        //                Number = group1.Number,
        //                YearOfAdmission = group1.YearOfAdmission,
        //                Faculty = group1.Faculty,
        //                Degree = group1.Degree,
        //                MonitorId = group1.MonitorId
        //            });
        //    }

        //    foreach (var b in groups)
        //    {
        //        b.Number = b.GetGroupNumber(Semester.Today());

        //    }
        //    groups.OrderBy(p => p.Number);


        //    ViewBag.Groups = new SelectList(groups, "Id", "Number");
        //    return View();


        //}
        public ActionResult GetJornalArchive(int semesterId, int numberId, int sabjectId)
        {

            return View();
        }
    }
}
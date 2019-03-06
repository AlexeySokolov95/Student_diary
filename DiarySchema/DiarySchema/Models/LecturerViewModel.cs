using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiarySchema.Models
{
    public class LecturerViewModel
    {
        public Subjects Subject { get; set; }
        public Groups Group { get; set; }

        //список имён студентов
        public List<String> NamesOfStudents { get; set; }
        //список оценок для студентов.
        public List<List<String>> StudentsGrades { get; set; }
        //список названий месяца+год.
        public List<String> MonthNames { get; set; }
        //список кол-ва дней в каждом месяце отдельно.Для colspan.
        public List<int> DaysMonth { get; set; }
        //список для отображение строки где день месяца + день недели.
        public List<String> DaysSemester { get; set; }
        //список доступных предметов у данной группы у этого преподователя
        public List<ViewSubjects> Subjects { get; set; }
        //список доступных групп у этого преподавателя
        public List<Groups> Groups { get; set; }
        //для таблицы с оценками за экзамен
        public List<TableEntry> ExamGrades { get; set; }
        //для таблицы с оценками за зачёт
        public List<TableEntry> PassFailTestGrades { get; set; }
        //для таблицы с оценками за курсовые работы
        public List<TableEntry> PracticalWorkGrades { get; set; }
        //для таблицы с оценками за практику
        public List<TableEntry> CoursePaperGrades { get; set; }
        //список для хранения оценок журнала по предметам
        public List<List<TableEntry>> ListOfGrades { get; set; }
        public void GetAllFinalGrades(DiaryConnection db, int GroupId, int lecturerId, int SubjectId, Semester semester)
        {
            this.ExamGrades = GetFinalGrades(db, GroupId, SubjectId, lecturerId, "Exam", semester);
            this.PassFailTestGrades = GetFinalGrades(db, GroupId, SubjectId, lecturerId,  "PassFailTest", semester);
            this.PracticalWorkGrades = GetFinalGrades(db, GroupId, SubjectId, lecturerId,  "PracticalWork", semester);
            this.CoursePaperGrades = GetFinalGrades(db, GroupId, SubjectId, lecturerId, "CoursePaper", semester);
        }
        private List<TableEntry> GetFinalGrades(DiaryConnection db, int GroupId, int SubjectId, int lecturerId, string typeOfGrades, Semester semester)
        {
            var finalGrades = new List<TableEntry>();
            var fG = (from p in db.TableEntry
                      join b in db.Students on p.StudentId equals b.Id
                      join c in db.TableOfGrades on p.TableOfGradeId equals c.Id
                      join d in db.Semester on c.SemesterId equals d.Id
                      join g in db.TeachersGroupsSubjects on c.TeachersGroupsSubjectId equals g.Id
                      where g.GroupId == GroupId && g.TeacherId == lecturerId &&
                            g.SubjectId == SubjectId && d.Number == semester.Number &&
                            d.Year == semester.Year && c.TypeOfKnowledgeControl == typeOfGrades
                      select new
                      {
                          TableOfGradeId = p.TableOfGradeId,
                          Id = p.Id,
                          Date = p.Date,
                          Value = p.Value,
                          StudentId = p.StudentId
                      }).ToList(); // обёрнуто в ToList для избежания проблем с DataReader.
            foreach (var p in fG)
            {
                finalGrades.Add(new TableEntry {
                    TableOfGradeId = p.TableOfGradeId,
                    Id = p.Id,
                    Date = p.Date,
                    Value = p.Value,
                    StudentId = p.StudentId
                });
            }
            return finalGrades;
        }
        public List<ViewSubjects> GetSubjectsForLecturer(DiaryConnection db, int lecturerId, Semester semester)
        {
            var subjects = new List<ViewSubjects>();
            var fG = (from p in db.Subjects
                      join b in db.TeachersGroupsSubjects on p.Id equals b.SubjectId
                      join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
                      join d in db.Semester on c.SemesterId equals d.Id
                      where b.TeacherId == lecturerId && c.Semester == semester && 
                            d.Year == semester.Year
                      select new
                      {
                          Name = p.Name,
                          Id = p.Id
                      }).ToList(); // обёрнуто в ToList для избежания проблем с DataReader.
            foreach (var b in fG)
            {
                subjects.Add(new ViewSubjects(b.Id, b.Name));
            }
            return subjects;
        }
        public List<Groups> GetGroupsForLecturer(DiaryConnection db, int lecturerId, Semester semester)
        {
            //var groups = new List<Groups>();
            var groups = (from p in db.Groups
                      join b in db.TeachersGroupsSubjects on p.Id equals b.SubjectId
                      join c in db.TableOfGrades on b.Id equals c.TeachersGroupsSubjectId
                      join d in db.Semester on c.SemesterId equals d.Id
                      where b.TeacherId == lecturerId && d.Number == semester.Number && 
                            d.Year == semester.Year
                      select new Groups
                      {
                          Id = p.Id,
                          Number = p.Number,
                          YearOfAdmission = p.YearOfAdmission,
                          Faculty = p.Faculty,
                          Degree = p.Degree,
                          MonitorId = p.MonitorId
                      }).ToList(); // обёрнуто в ToList для избежания проблем с DataReader.
            /*foreach (var b in fG)
            {
                groups.Add(new Groups(b.Id, b.Number, b.YearOfAdmission, b.Faculty, b.Degree, b.StudentId));
            }*/
            //Вычисление номера группы в текущем году.
            /*foreach (var b in groups)
            {
                b.Number = DiarySchema.Groups.GetActualGroupNumber(b, DateTime.Today.Year);
            }*/
            return groups;
        }
        public void GetNamesOfStudents(DiaryConnection db, int GroupId, int lecturerId, int SubjectId, Semester semester)
        {
            //достаём из бд имена студентов по пред за выбранный семестр для журнала
            var namesOfStudents = new List<String>();
            var sN = db.Students.Where(p => p.GroupId == GroupId).Select(p => new { Name = p.FullName }).ToList();
            foreach (var b in sN)
            {
                namesOfStudents.Add(b.Name);
            }
            this.NamesOfStudents = namesOfStudents;
        }
        public void GetStudentsGrades(DiaryConnection db, int GroupId, int lecturerId, int SubjectId, Semester semester)
        {
            //извлекаем id студентов.
            //фактически повторный запрос
            var idStudents = (from p in db.Students
                              join b in db.Groups on p.GroupId equals b.Id
                              join c in db.TeachersGroupsSubjects on b.Id equals c.GroupId
                              join d in db.TableOfGrades on c.Id equals d.TeachersGroupsSubjectId
                              join j in db.Semester on d.SemesterId equals j.Id
                              where c.TeacherId == lecturerId && c.GroupId == GroupId &&
                                    c.SubjectId == SubjectId && j.Number == semester.Number &&
                                    j.Year == semester.Year && d.TypeOfKnowledgeControl == "Journal"
                              select new { Id = p.Id }).ToList();
            //достаём из бд список оценок к найденым студентам
            var subGr = new List<List<TableEntry>>();
            //достаём оценки отдельно для каждого студента
            foreach (var b in idStudents)
            {
                var listFfGrades = (from p in db.TableEntry      //db.TableEntrys.Join(db.TableOfGrades, p => p.TableOfGradeId ,c => c.Id,(p,c) => ).Join
                                    //join k in db.Students on p.StudentId equals k.Id
                                    join c in db.TableOfGrades on p.TableOfGradeId  equals c.Id
                                    join d in db.Semester on c.SemesterId equals d.Id
                                    join g in db.TeachersGroupsSubjects on c.TeachersGroupsSubjectId equals g.Id
                                    //join s in db.Subjects on g.SubjectId equals s.Id
                                    where p.StudentId == b.Id && g.TeacherId == lecturerId && g.GroupId == GroupId &&
                                    g.SubjectId == SubjectId && d.Number == semester.Number && d.Year == semester.Year
                                    && c.TypeOfKnowledgeControl == "Journal"
                                    select new
                                    {
                                        Value = p.Value,
                                        Date = p.Date,
                                        Id = p.Id,
                                        TableOfGradeId  = p.TableOfGradeId ,
                                        StudentId = p.StudentId
                                    }).ToList(); // обёрнуто в ToList для избежания проблем с DataReader.

                // из списка анонимного типа в список TableEntry
                var list1 = new List<TableEntry>(); //вложенный список
                foreach (var c in listFfGrades)
                {
                    list1.Add(new TableEntry {Id = c.Id,
                        StudentId = c.StudentId,
                        Date = c.Date,
                        TableOfGradeId  = c.TableOfGradeId ,
                        Value = c.Value });
                }
                subGr.Add(list1);
            }
            this.ListOfGrades = subGr;
            //пока у нас учтены только те дни в которых стоят оценки, надо заполнить пустые клетки в таблице
            List<List<string>> studentsGrades = new List<List<string>>();
            foreach (var b in subGr)
            {
                DateTime startSemestr = new DateTime(2017, 2, 10);
                DateTime endSemestr = new DateTime(2017, 6, 10);
                DateTime day = new DateTime(2017, 2, 10);
                //оценки за 1 предмет
                List<string> subjGrades = new List<string>();
                while (day <= endSemestr)
                {
                    var toDayTableEntry = b.Find(x => x.Date == day);
                    if (toDayTableEntry != null)
                        subjGrades.Add((string)toDayTableEntry.Value);
                    else
                        subjGrades.Add("");
                    day = day.AddDays(1);
                }
                studentsGrades.Add(subjGrades);
            }
            this.StudentsGrades = studentsGrades;
        }
        public void GetMounthNames()
        {
            var monthNames = new List<string>();
            DateTime startSemestr1 = new DateTime(2017, 2, 10);
            DateTime endSemestr1 = new DateTime(2017, 6, 10);
            string[] mounth = new string[12] { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
            while (startSemestr1 <= endSemestr1)
            {
                monthNames.Add(mounth[startSemestr1.Month - 1] + " " + startSemestr1.Year);
                startSemestr1 = startSemestr1.AddMonths(1);
            }
            this.MonthNames = monthNames;
        }
        public void GetDaysMounth()
        {
            if (this.ListOfGrades.Count > 0)
            {
                List<int> daysMonth = new List<int>();
                this.ListOfGrades[0].OrderBy(p => p.Date);
                int start = this.ListOfGrades[0][0].Date.Value.Month;
                int colspan = 0;
                for (int i = 0; i < this.ListOfGrades[0].Count; i++)
                {
                    colspan++;
                    if (this.ListOfGrades[0][i].Date.Value.Month > start)
                    {
                        start++;
                        daysMonth.Add(colspan);
                        colspan = 0;
                    }
                    if (i + 1 == this.ListOfGrades[0].Count)
                        daysMonth.Add(colspan);
                }
                this.DaysMonth = daysMonth;
            }
        }
        public void GetDaysSemester()
        {
            if (this.ListOfGrades.Count > 0)
            {
                var daysSemester = new List<string>();
                var dayOfWeek = new Dictionary<string, string>();
                dayOfWeek.Add("Monday", "Пн");
                dayOfWeek.Add("Tuesday", "Вт");
                dayOfWeek.Add("Wednesday", "Ср");
                dayOfWeek.Add("Thursday", "Чт");
                dayOfWeek.Add("Friday", "Пт");
                dayOfWeek.Add("Saturday", "Сб");
                dayOfWeek.Add("Sunday", "Вс");
                foreach (var item in this.ListOfGrades[0])
                    daysSemester.Add(item.Date.Value.Day + " " + dayOfWeek[item.Date.Value.DayOfWeek.ToString()]);
                this.DaysSemester = daysSemester;
            }
        }
    }
    
   /* public class ViewGroups
    {
        //Номер группы в тещем году
        public int Number { get; set; }
        public int Id { get; set; }
        public ViewGroups(int Id, int number)
        {
            this.Number = number;
            this.Id = Id;
        }
    }*/
    public class ViewSubjects
    {
        //Номер группы в тещем году
        public string Name { get; set; }
        public int Id { get; set; }
        public ViewSubjects(int id, string name)
        {
            this.Name = name;
            this.Id = Id;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace DiarySchema.Models
{
    public class StudentJournalViewModel
    {
        //список названий предметов. 
        public List<String> NamesOfSubject { get; set; }
        //список оценок для n предмета. 
        public List<List<string>> SubjectsGrades { get; set; }
    //список названий месяца+год. 
    public List<String> MonthNames { get; set; }
    //список кол-ва дней в каждом месяце отдельно.Для colspan. 
    public List<int> DaysMonth { get; set; }
    //список для отображение строки где день месяца + день недели. 
    public List<String> DaysSemester { get; set; }
    //список строк для отображение пропусков ввида:”3 пропуска из 24” 
    public List<String> Missed { get; set; }
    //для таблицы с оценками за экзамен 
    public List<FinalGrade> ExamGrades { get; set; }
    //для таблицы с оценками за зачёт 
    public List<FinalGrade> PassFailTestGrades { get; set; }
    //для таблицы с оценками за курсовые работы 
    public List<FinalGrade> PracticalWorkGrades { get; set; }
    //для таблицы с оценками за практику 
    public List<FinalGrade> CoursePaperGrades { get; set; }
    //список для хранения оценок журнала по предметам 
    private List<List<TableEntry>> ListOfGrades { get; set; }

//для формирования сылок с оценками за разные семестры 
public List<List<Semester>> OrderedSemesters { get; set; } 
private List<List<TableEntry>> ListOfGradeыs { get; set; } 
//метод для нахождения итоговых оценок 
public void GetAllFinalGrades(DiaryConnection db, int StudentId, Semester semester)
{
    this.ExamGrades = GetFinalGrades(db, StudentId, "Exam", semester);
    this.PassFailTestGrades = GetFinalGrades(db, StudentId, "PassFailTest", semester);
    this.PracticalWorkGrades = GetFinalGrades(db, StudentId, "PracticalWork", semester);
    this.CoursePaperGrades = GetFinalGrades(db, StudentId, "CoursePaper", semester);
}
private List<FinalGrade> GetFinalGrades(DiaryConnection db, int StudentId, string typeOfGrades, Semester semester)
{
    var finalGrades = new List<FinalGrade>();
    var fG = (from p in db.TableEntry
              join c in db.TableOfGrades on p.TableOfGradeId equals c.Id
              join d in db.Semester on c.SemesterId equals d.Id
              join g in db.TeachersGroupsSubjects on c.TeachersGroupsSubjectId equals g.Id
              join s in db.Subjects on g.SubjectId equals s.Id
              where p.StudentId == StudentId && d.Number == semester.Number && d.Year == semester.Year && c.TypeOfKnowledgeControl == typeOfGrades
              select new
              {
                  Value = p.Value,
                  Subject = s.Name
              }).ToList(); // обёрнуто в ToList для избежания проблем с DataReader. 
    foreach (var b in fG)
    {
        finalGrades.Add(new FinalGrade(b.Value, b.Subject));
    }
    return finalGrades;
}
public void GetRefToSemesters(DiaryConnection db, int studentId)
{
    List < List < Semester>> orderedSemesters = new List<List<Semester>>();
    List<string> years = new List<string>();
    var semesters = (from p in db.Semester
                     join c in db.TableOfGrades on p.Id equals c.SemesterId
                     join g in db.TeachersGroupsSubjects on c.TeachersGroupsSubjectId equals g.Id
                     join f in db.Groups on g.GroupId equals f.Id
                     join h in db.Students on f.Id equals h.GroupId
                     where h.Id == studentId
                     select new
                     {
                         Id = p.Id,
                         BeginningDate = p.BeginningDate,
                         EndDate = p.EndDate,
                         Number = p.Number,
                         Year = p.Year
                     }).Distinct();
    List<Semester> findSemesters = new List<Semester>();
    foreach (var item in semesters)
    {
        findSemesters.Add(new Semester { Id = item.Id, BeginningDate = item.BeginningDate, EndDate = item.EndDate, Number = item.Number, Year = item.Year });
        if (!years.Contains(item.Year))
            years.Add(item.Year);
    }
    foreach (var item in years)
        orderedSemesters.Add(findSemesters.Where(p => p.Year == item).ToList());
    OrderedSemesters = orderedSemesters;
}

public void GetNamesOfSubject(DiaryConnection db, int StudentId, Semester semester)
{
    //достаём из бд список предметов за выбранный семестр для журнала 
    var namesOfSubject = new List<String>();
    var sN = (from p in db.Subjects
              join c in db.TeachersGroupsSubjects on p.Id
              equals c.SubjectId
              join h in db.TableOfGrades on c.Id equals h.TeachersGroupsSubjectId
              join d in db.Semester on h.SemesterId equals d.Id
              join g in db.Groups on c.GroupId equals g.Id
              join s in db.Students on g.Id equals s.GroupId
              where s.Id == StudentId && d.Number == semester.Number && d.Year == semester.Year && h.TypeOfKnowledgeControl == "Journal"
              select new { Id = p.Id, Name = p.Name }).ToList(); // обёрнуто в ToList для избежания проблем с DataReader. 
    foreach (var b in sN)
    {
        namesOfSubject.Add(b.Name);
    }
    this.NamesOfSubject = namesOfSubject;
}
public void GetSubjectGrades(DiaryConnection db, int StudentId, Semester semester)
{
    //извлекаем id предметов. 
    //фактически повторный запрос 
    var idSubject = (from p in db.Subjects
                     join c in db.TeachersGroupsSubjects on p.Id equals c.SubjectId
                     join h in db.TableOfGrades on c.Id equals h.TeachersGroupsSubjectId
                     join d in db.Semester on h.SemesterId equals d.Id
                     join g in db.Groups on c.GroupId equals g.Id
                     join s in db.Students on g.Id equals s.GroupId
                     where s.Id == StudentId && d.Number == semester.Number && d.Year == semester.Year && h.TypeOfKnowledgeControl == "Journal"
                     select new { Id = p.Id }).ToList();
    //достаём из бд список оценок к найденым предметам 
    var subGr = new List<List<TableEntry>>();
    //достаём оценки отдельно для каждого предмета 
    foreach (var b in idSubject)
    {
        var listFfGrades = (from p in db.TableEntry //db.TableEntrys.Join(db.TableOfGrades, p => p.TableOfGradeId ,c => c.Id,(p,c) => ).Join 
                            join c in db.TableOfGrades on p.TableOfGradeId equals c.Id
                            join d in db.Semester on c.SemesterId equals d.Id
                            join g in db.TeachersGroupsSubjects on c.TeachersGroupsSubjectId equals g.Id
                            //join s in db.Subjects on g.SubjectId equals s.Id 
                            where p.StudentId == StudentId && g.SubjectId == b.Id && d.Number == semester.Number && d.Year == semester.Year
                            select new
                            {
                                Value = p.Value,
                                Date = p.Date,
                                Id = p.Id,
                                TableOfGradeId = p.TableOfGradeId,
                                StudentId = p.StudentId
                            }).ToList(); // обёрнуто в ToList для избежания проблем с DataReader. 

        // из списка анонимного типа в список TableEntry 
        var list1 = new List<TableEntry>(); //вложенный список 
        foreach (var c in listFfGrades)
        {
            list1.Add(new TableEntry
            {
                Id = c.Id,
                StudentId = c.StudentId,
                Date = c.Date,
                TableOfGradeId = c.TableOfGradeId,
                Value = c.Value
            });
        }
        subGr.Add(list1);
    }
    this.ListOfGrades = subGr;
    //пока у нас учтены только те дни в которых стоят оценки, надо заполнить пустые клетки в таблице 
    List < List < string>> subjectsGrades = new List<List<string>>();
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
        subjectsGrades.Add(subjGrades);
    }
    this.SubjectsGrades = subjectsGrades;
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
    DateTime startSemestr = new DateTime(2017, 2, 10);
    DateTime endSemestr = new DateTime(2017, 6, 10);
    List<int> daysMonth = new List<int>();
    int dm = 0;
    var startMonth = startSemestr.Month;
    while (startSemestr <= endSemestr)
    {
        if (startSemestr == endSemestr)
            daysMonth.Add(dm + 1);
        else if (startMonth != startSemestr.Month)
        {
            daysMonth.Add(dm);
            startMonth = startSemestr.Month;
            dm = 1;
        }
        else
            dm++;
        startSemestr = startSemestr.AddDays(1);
    }
    this.DaysMonth = daysMonth;
}
public void GetDaysSemester()
{
    DateTime startSemestr = new DateTime(2017, 2, 10);
    DateTime endSemestr = new DateTime(2017, 6, 10);
    var daysSemester = new List<string>();
    var dayOfWeek = new Dictionary<string, string>();
    dayOfWeek.Add("Monday", "Пн");
    dayOfWeek.Add("Tuesday", "Вт");
    dayOfWeek.Add("Wednesday", "Ср");
    dayOfWeek.Add("Thursday", "Чт");
    dayOfWeek.Add("Friday", "Пт");
    dayOfWeek.Add("Saturday", "Сб");
    dayOfWeek.Add("Sunday", "Вс");
    while (startSemestr <= endSemestr)
    {
        daysSemester.Add(startSemestr.Day + " " + dayOfWeek[startSemestr.DayOfWeek.ToString()]);
        startSemestr = startSemestr.AddDays(1);
    }
    this.DaysSemester = daysSemester;
}
public void GetMissed()
{
    DateTime startSemestr = new DateTime(2017, 2, 10);
    DateTime endSemestr = new DateTime(2017, 6, 10);
    var missed = new List<string>();
    var miss = new List<int>();
    var allDay = new List<int>();
    startSemestr = new DateTime(2017, 2, 10);
    foreach (var List in this.ListOfGrades)
    {
        int ad = 0;
        int ms = 0;
        foreach (var b in List)
        {

            if (b.Date <= DateTime.Today)
            {
                ad++;
                if (b.Value == "Н" || b.Value == "н")
                    ms++;
            }

        }
        missed.Add(ms + " из " + ad);
    }
    this.Missed = missed;
} 
} 
public class FinalGrade
{
    public string Value { get; set; }
    public string Subject { get; set; }
    public FinalGrade(string value, string subject)
    {
        this.Value = value;
        this.Subject = subject;
    }
} 
}

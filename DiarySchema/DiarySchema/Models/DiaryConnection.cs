namespace DiarySchema.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DiaryConnection : DbContext
    {
        public DiaryConnection()
           : base("name=Diary2")
        {
        }
        public virtual DbSet<ApplicationsForRegistration> ApplicationsForRegistration { get; set; }
        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<Semester> Semester { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Subjects> Subjects { get; set; }
        public virtual DbSet<TableEntry> TableEntry { get; set; }
        public virtual DbSet<TableOfGrades> TableOfGrades { get; set; }
        public virtual DbSet<Teachers> Teachers { get; set; }
        public virtual DbSet<TeachersGroupsSubjects> TeachersGroupsSubjects { get; set; }
        public virtual DbSet<TheTasks> TheTasks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Groups>()
                .HasMany(e => e.ApplicationsForRegistration)
                .WithRequired(e => e.Groups)
                .HasForeignKey(e => e.GroupId);

            modelBuilder.Entity<Groups>()
                .HasMany(e => e.Students1)
                .WithRequired(e => e.Groups1)
                .HasForeignKey(e => e.GroupId);

            modelBuilder.Entity<Groups>()
                .HasMany(e => e.TeachersGroupsSubjects)
                .WithRequired(e => e.Groups)
                .HasForeignKey(e => e.GroupId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Groups>()
                .HasMany(e => e.TheTasks)
                .WithRequired(e => e.Groups)
                .HasForeignKey(e => e.GroupId);

            modelBuilder.Entity<Students>()
                .HasMany(e => e.Groups)
                .WithOptional(e => e.Students)
                .HasForeignKey(e => e.MonitorId);

            modelBuilder.Entity<Students>()
                .HasMany(e => e.TableEntry)
                .WithRequired(e => e.Students)
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<Subjects>()
                .HasMany(e => e.TeachersGroupsSubjects)
                .WithRequired(e => e.Subjects)
                .HasForeignKey(e => e.SubjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TableOfGrades>()
                .HasMany(e => e.TableEntry)
                .WithRequired(e => e.TableOfGrades)
                .HasForeignKey(e => e.TableOfGradeId);

            modelBuilder.Entity<Teachers>()
                .HasMany(e => e.TeachersGroupsSubjects)
                .WithRequired(e => e.Teachers)
                .HasForeignKey(e => e.TeacherId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Teachers>()
                .HasMany(e => e.TheTasks)
                .WithRequired(e => e.Teachers)
                .HasForeignKey(e => e.TeacherId);

            modelBuilder.Entity<TeachersGroupsSubjects>()
                .HasMany(e => e.TableOfGrades)
                .WithRequired(e => e.TeachersGroupsSubjects)
                .HasForeignKey(e => e.TeachersGroupsSubjectId);
        }
    }
}

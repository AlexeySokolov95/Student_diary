namespace DiarySchema
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;
    using DiarySchema.Models;
    public partial class DiaryContext : DbContext
    {
        public static DiaryContext Create()
        {
            return new DiaryContext();
        }
        public DiaryContext(): base("name=DiaryDB"){}
        public virtual DbSet<Groups> Group { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Subjects> Subjects { get; set; }
        public virtual DbSet<TableEntry> TableEntry { get; set; }
        public virtual DbSet<TableOfGrades> TableOfGrade { get; set; }
        public virtual DbSet<TheTasks> Tasks { get; set; }
        public virtual DbSet<Teachers> Teachers { get; set; }
        public virtual DbSet<TeachersGroupsSubjects> TeachersGroupSubjects { get; set; }
        /*
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>()
                .HasMany(e => e.Students)
                .WithRequired(e => e.Group)
                .HasForeignKey(e => e.GroupId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Group>()
                .HasMany(e => e.Tasks)
                .WithRequired(e => e.Group)
                .HasForeignKey(e => e.GroupId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Group>()
                .HasMany(e => e.TeachersGroupSubjects)
                .WithRequired(e => e.Group)
                .HasForeignKey(e => e.GroupId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Students>()
                .HasMany(e => e.Group1)
                .WithOptional(e => e.Students1)
                .HasForeignKey(e => e.MonitorId);

            modelBuilder.Entity<Students>()
                .HasMany(e => e.TableEntry)
                .WithRequired(e => e.Students)
                .HasForeignKey(e => e.StudentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Subjects>()
                .HasMany(e => e.TeachersGroupSubjects)
                .WithRequired(e => e.Subjects)
                .HasForeignKey(e => e.SubjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TableOfGrade>()
                .Property(e => e.Semester)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TableOfGrade>()
                .HasMany(e => e.TableEntry)
                .WithRequired(e => e.TableOfGrade)
                .HasForeignKey(e => e.TableOfGradeId )
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tasks>()
                .Property(e => e.Task)
                .IsUnicode(false);

            modelBuilder.Entity<Teachers>()
                .HasMany(e => e.Tasks)
                .WithRequired(e => e.Teachers)
                .HasForeignKey(e => e.TeacherId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Teachers>()
                .HasMany(e => e.TeachersGroupSubjects)
                .WithRequired(e => e.Teachers)
                .HasForeignKey(e => e.TeacherId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TeachersGroupSubjects>()
                .HasMany(e => e.TableOfGrade)
                .WithRequired(e => e.TeachersGroupSubjects)
                .HasForeignKey(e => e.TeachersGroupsSubjectId);
        }
        */
    }
}

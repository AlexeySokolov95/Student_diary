namespace DiarySchema.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Groups
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Groups()
        {
            Students1 = new HashSet<Students>();
            TeachersGroupsSubjects = new HashSet<TeachersGroupsSubjects>();
            TheTasks = new HashSet<TheTasks>();
        }

        public int Id { get; set; }

        public int Number { get; set; }

        public int YearOfAdmission { get; set; }

        [Required]
        [StringLength(128)]
        public string Faculty { get; set; }

        [Required]
        [StringLength(128)]
        public string Degree { get; set; }

        [Required]
        [StringLength(128)]
        public string FormOfStudy { get; set; }

        public int? MonitorId { get; set; }

        public virtual Students Students { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Students> Students1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TeachersGroupsSubjects> TeachersGroupsSubjects { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TheTasks> TheTasks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        public virtual ICollection<ApplicationsForRegistration> ApplicationsForRegistration { get; set; }
        public int GetGroupNumber(Semester semester)
        {
            var group = this;
            string firstYear = semester.Year.Split('|')[0];
            int year = Convert.ToInt32(firstYear);
            if (year - this.YearOfAdmission > 4)
                group.Number += 400;
            else if (year > this.YearOfAdmission)
            {
                while (year != group.YearOfAdmission)
                {
                    group.Number+=100;
                    group.YearOfAdmission++;
                }
            }
            return group.Number;
        }

    }
}

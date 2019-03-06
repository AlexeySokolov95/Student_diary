namespace DiarySchema.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TeachersGroupsSubjects
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TeachersGroupsSubjects()
        {
            TableOfGrades = new HashSet<TableOfGrades>();
        }

        public int Id { get; set; }

        public int GroupId { get; set; }

        public int SubjectId { get; set; }

        public int TeacherId { get; set; }

        public virtual Groups Groups { get; set; }

        public virtual Subjects Subjects { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TableOfGrades> TableOfGrades { get; set; }

        public virtual Teachers Teachers { get; set; }
    }
}

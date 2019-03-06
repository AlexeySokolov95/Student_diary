namespace DiarySchema.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TableOfGrades
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TableOfGrades()
        {
            TableEntry = new HashSet<TableEntry>();
        }

        public int Id { get; set; }

        public int TeachersGroupsSubjectId { get; set; }

        [Required]
        [StringLength(128)]
        public string TypeOfKnowledgeControl { get; set; }

        public int SemesterId { get; set; }

        public virtual Semester Semester { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TableEntry> TableEntry { get; set; }

        public virtual TeachersGroupsSubjects TeachersGroupsSubjects { get; set; }
    }
}

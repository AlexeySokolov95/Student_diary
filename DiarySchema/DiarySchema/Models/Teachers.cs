namespace DiarySchema.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Teachers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Teachers()
        {
            TeachersGroupsSubjects = new HashSet<TeachersGroupsSubjects>();
            TheTasks = new HashSet<TheTasks>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string FullName { get; set; }

        [Required]
        [StringLength(128)]
        public string Departament { get; set; }

        [StringLength(128)]
        public string UserName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TeachersGroupsSubjects> TeachersGroupsSubjects { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TheTasks> TheTasks { get; set; }
    }
}

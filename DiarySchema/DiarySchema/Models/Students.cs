namespace DiarySchema.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Students
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Students()
        {
            Groups = new HashSet<Groups>();
            TableEntry = new HashSet<TableEntry>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Number { get; set; }

        [Required]
        [StringLength(128)]
        public string FullName { get; set; }

        public int YearOfBirth { get; set; }

        public int GroupId { get; set; }

        public byte[] Photo { get; set; }

        [StringLength(256)]
        public string UserName { get; set; }

        [StringLength(128)]
        public string Email { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Groups> Groups { get; set; }

        public virtual Groups Groups1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TableEntry> TableEntry { get; set; }
    }
}

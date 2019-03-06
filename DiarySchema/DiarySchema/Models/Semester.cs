namespace DiarySchema.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Semester")]
    public partial class Semester
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Semester()
        {
            TableOfGrades = new HashSet<TableOfGrades>();
        }

        public int Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime BeginningDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        
        [Required]
        [StringLength(128)]
        public string Number { get; set; }

        [RegularExpression(@"\d{4}\|\d{4}", ErrorMessage = "��� ������ ���� � ������� ����|����")]
        [Required]
        [StringLength(128)]
        public string Year { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TableOfGrades> TableOfGrades { get; set; }
        public static Semester Today()
        {
            Semester semester;
            using (DiaryConnection db = new DiaryConnection())
            {
                //���� �������� � ���������� � ���� ������
                var toDay = DateTime.Today;
                semester = db.Semester.Where(p => p.BeginningDate < toDay && p.EndDate > toDay).First();
            }
            return semester;
        }
    }
}

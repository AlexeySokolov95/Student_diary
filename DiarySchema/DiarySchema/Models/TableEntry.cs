namespace DiarySchema.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TableEntry")]
    public partial class TableEntry
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }

        public int TableOfGradeId { get; set; }

        [StringLength(128)]
        public string Value { get; set; }

        public virtual Students Students { get; set; }

        public virtual TableOfGrades TableOfGrades { get; set; }
    }
}

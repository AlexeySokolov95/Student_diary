namespace DiarySchema.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TheTasks
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public string TaskText { get; set; }

        public int GroupId { get; set; }

        public string Documents { get; set; }

        public virtual Groups Groups { get; set; }

        public virtual Teachers Teachers { get; set; }
    }
}

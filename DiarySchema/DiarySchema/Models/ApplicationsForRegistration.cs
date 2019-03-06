namespace DiarySchema.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ApplicationsForRegistration")]
    public partial class ApplicationsForRegistration
    {
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(64)]
        public string SecondName { get; set; }

        [Required]
        [StringLength(64)]
        public string MiddleName { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        public int GroupId { get; set; }

        [Required]
        [StringLength(64)]
        public string Login { get; set; }

        [Required]
        [StringLength(64)]
        public string Password { get; set; }

        [Required]
        [StringLength(64)]
        public string CardNumber { get; set; }
        [Required]
        [StringLength(64)]
        public string Email { get; set; }

        public virtual Groups Groups { get; set; }
    }
}

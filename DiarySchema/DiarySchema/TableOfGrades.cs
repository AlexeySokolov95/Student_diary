//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiarySchema
{
    using System;
    using System.Collections.Generic;
    
    public partial class TableOfGrades
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TableOfGrades()
        {
            this.TableEntry = new HashSet<TableEntry>();
        }
    
        public int Id { get; set; }
        public int TeachersGroupsSubjectId { get; set; }
        public string TypeOfKnowledgeControl { get; set; }
        public int SemesterId { get; set; }
    
        public virtual Semester Semester { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TableEntry> TableEntry { get; set; }
        public virtual TeachersGroupsSubjects TeachersGroupsSubjects { get; set; }
    }
}

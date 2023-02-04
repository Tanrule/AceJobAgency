using AceJobAgency.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceJobAgency.Core.Entities.Ecosystem
{
    public class Document : BaseEntity
    {
        public DocumentType DocumentType { get; set; }
        public string? FileUrl { get; set; }
        public int PersonId { get; set; }
        [ForeignKey("PersonId")]
        public Person Person { get; set; }
        public string? FileName { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }
    }

    public enum DocumentType
    {
        Resume
    }
}

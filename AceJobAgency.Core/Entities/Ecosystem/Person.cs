using AceJobAgency.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceJobAgency.Core.Entities.Ecosystem
{
    public class Person : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string NRIC { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<Document> Documents { get; set; }
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}

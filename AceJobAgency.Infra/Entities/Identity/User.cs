using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceJobAgency.Infra.Entities.Identity
{
    public class User : IdentityUser<int>
    {
        [Key]
        public override int Id { get; set; }
        public int PersonId { get; set; }
        public string SCode { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceJobAgency.Infra.Entities.Identity
{
    public class Role : IdentityRole<int>
    {
        [Key]
        public override int Id { get; set; }
    }
}

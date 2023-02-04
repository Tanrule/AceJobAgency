using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceJobAgency.Infra.Entities.Identity
{
    public class UserLogin : IdentityUserLogin<int> 
    {
        [Key]
        public int Id { get; set; }
    }
}

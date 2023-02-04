using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceJobAgency.Core.Models.User
{
    public class RegisterUserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSucceded { get; set; }
        public Dictionary<IdentityErrorType, List<string>> Errors { get; set; }
    }

    public enum IdentityErrorType
    {
        Email,
        Password
    }
}

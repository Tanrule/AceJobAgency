using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceJobAgency.Infra.Entities.Identity
{
    public class UserAudit
    {
        [Key]
        public int Id { get; set; }
        public string? PageName { get; set; } = string.Empty;
        public string? ActionName { get; set; } = string.Empty;
        public string? ActionType { get; set; } = string.Empty;
        public string? IpAddress { get; set; } = string.Empty;
        public string? LoggedInAt { get; set; } = string.Empty;
        public string? LoggedOutAt { get; set; } = string.Empty;
        public string? PageAccessed { get; set; } = string.Empty;
        public string? SessionId { get; set; } = string.Empty;
        public string? UrlReferrer { get; set; } = string.Empty;

        public int? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
        public string? Description { get; set; } = string.Empty;
    }
}

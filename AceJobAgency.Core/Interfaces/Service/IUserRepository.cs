using AceJobAgency.Core.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceJobAgency.Core.Interfaces.Service
{
    public interface IUserRepository
    {
        Task UpdateUserPersonId(int userId, int personId);
        Task InsertAudit(string actionName, string ipAdress, string loggedInAt, string loggedOutAt, string pageAccessed, string pageName, string sessionId,
            string urlReferrer, string actionType, int? userId, string description);
    }
}

using AceJobAgency.Core.Interfaces.Service;
using AceJobAgency.Core.Models.User;
using AceJobAgency.Infra.Contexts;
using AceJobAgency.Infra.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AceJobAgency.Infra.Services.Identity
{
    public class UserDataAccessService : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly IdentityContext _identityContext;

        public UserDataAccessService(UserManager<User> userManager, IdentityContext identityContext)
        {
            _userManager = userManager;
            _identityContext = identityContext;
        }

        public async Task UpdateUserPersonId(int userId, int personId)
        {
            var user = _userManager.Users.SingleOrDefault(t => t.Id == userId);
            if (user != null)
            {
                user.PersonId = personId;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task InsertAudit(string actionName, string ipAdress, string loggedInAt, string loggedOutAt, string pageAccessed, string pageName, string sessionId,
            string urlReferrer, string actionType, int? userId, string description)
        {
            var audit = new UserAudit()
            {
                ActionName = actionName,
                IpAddress = ipAdress,
                LoggedInAt = loggedInAt,
                LoggedOutAt = loggedOutAt,
                PageAccessed = pageAccessed,
                PageName = pageName,
                SessionId = sessionId,
                UrlReferrer = urlReferrer,
                UserId = userId,
                ActionType = actionType,
                Description = description
            };
            await _identityContext.UserAudits.AddAsync(audit);
            await _identityContext.SaveChangesAsync();
        }
    }
}

using AceJobAgency.Core.Interfaces.Service;
using AceJobAgency.Core.Interfaces.Utility;
using AceJobAgency.Core.Services;
using AceJobAgency.Infra.Services.Agency;
using AceJobAgency.Infra.Services.Broadcast;
using AceJobAgency.Infra.Services.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceJobAgency.Infra.Extensions
{
    public static class ServiceExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAgencyRepository, AgencyDataAccessService>();
            services.AddScoped<IUserRepository, UserDataAccessService>();
            services.AddScoped<UserService>();
            services.AddScoped<IEmailClient, EmailClient>();
        }
    }
}

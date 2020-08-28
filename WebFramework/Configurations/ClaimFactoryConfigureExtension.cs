using DataLayer.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using WebFramework.ClaimFactory;

namespace WebFramework.Configurations
{
  

    public static class ClaimFactoryConfigureExtension
    {
        public static void ClaimFactoryConfiguration(this IServiceCollection services)
        {

            services.AddScoped<IUserClaimsPrincipalFactory<Users>, ApplicationClaimFactory>();
        }
    }
}

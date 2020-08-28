//using Core;
//using DataLayer.Entities.Users;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.DependencyInjection;
//using Service;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace WebFramework.Configurations
//{
//    public static class IdentityConfigureExtensions
//    {
//        public static void AddCustomIdentity(this IServiceCollection services, IdentitySettings settings)
//        {
//            services.AddIdentity<Users, Roles>(identityOptions =>
//            {
//                //Password Settings
//                identityOptions.Password.RequireDigit = settings.PasswordRequireDigit;
//                identityOptions.Password.RequiredLength = settings.PasswordRequiredLength;
//                identityOptions.Password.RequireNonAlphanumeric = settings.PasswordRequireNonAlphanumic; //#@!
//                identityOptions.Password.RequireUppercase = settings.PasswordRequireUppercase;
//                identityOptions.Password.RequireLowercase = settings.PasswordRequireLowercase;

//                //UserName Settings
//                identityOptions.User.RequireUniqueEmail = settings.RequireUniqueEmail;

//                //Singin Settings
//                //identityOptions.SignIn.RequireConfirmedEmail = false;
//                //identityOptions.SignIn.RequireConfirmedPhoneNumber = false;

//                //Lockout Settings
//                identityOptions.Lockout.MaxFailedAccessAttempts = 5;
//                identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//                identityOptions.Lockout.AllowedForNewUsers = false;
//            })
//            .AddEntityFrameworkStores<DatabaseContext>();
//        }
//    }
//}

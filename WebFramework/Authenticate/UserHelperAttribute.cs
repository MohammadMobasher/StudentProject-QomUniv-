//using Core.CustomAttributes;
//using Core.Utilities;
//using DataLayer.SSOT;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Service.Repos.User;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Reflection;
//using System.Threading.Tasks;

//namespace WebFramework.Authenticate
//{
//    public class HasAccessAttribute : Attribute, IFilterFactory
//    {

//        public HasAccessAttribute()
//        {

//        }

//        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) =>
//            new InternalHasAccessAttribute(serviceProvider.GetService(typeof(UsersAccessRepository)) as UsersAccessRepository
//                , serviceProvider.GetService(typeof(UsersRoleRepository)) as UsersRoleRepository);

//        public bool IsReusable => false;

//        private class InternalHasAccessAttribute : ActionFilterAttribute
//        {
//            private readonly UsersAccessRepository _usersAccessRepository;
//            private readonly UsersRoleRepository _usersRoleRepository;

//            public InternalHasAccessAttribute(UsersAccessRepository usersAccessRepository, UsersRoleRepository usersRoleRepository)
//            {
//                _usersAccessRepository = usersAccessRepository ?? throw new ArgumentNullException(nameof(usersAccessRepository));
//                _usersRoleRepository = usersRoleRepository ?? throw new ArgumentNullException(nameof(usersRoleRepository));
//            }

//            public override void OnActionExecuting(ActionExecutingContext context)
//            {
//                try
//                {
//                    var isAllowAccess = context.ActionDescriptor.EndpointMetadata.Any(a => a.GetType() == typeof(AllowAccessAttribute));

//                    if (!isAllowAccess)
//                    {

//                        if (!context.HttpContext.User.Identity.IsAuthenticated)
//                        {
//                            context.HttpContext.Response.Redirect("/Account/Login", true);
//                        }
//                        else
//                        {

//                            var route = context.ActionDescriptor.RouteValues;
//                            //*************************************REVIEW**********************************************************
//                            var userId = int.Parse(context.HttpContext.User.Identity.FindFirstValue(ClaimTypes.NameIdentifier));
//                            var role = _usersRoleRepository.TableNoTracking.FirstOrDefault(a => a.UserId == userId).RoleId;

//                            // تصمیم گیری نهایی که برای ما سرعت مهم است یا امنیت
//                            //var role = int.Parse(context.HttpContext.User.Identity.FindFirstValue(ClaimTypes.Role));

//                            //*****************************************************************************************************

//                            if (!_usersAccessRepository.HasAccess(role, route))
//                            {
//                                context.Result = new BadRequestResult();
//                            }
//                        }
//                    }

//                }
//                catch
//                {
//                    context.Result = new BadRequestResult();
//                }

//            }
//        }
//    }
//}

//using DataLayer.Entities.Users;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace WebFramework.ClaimFactory
//{
//    public class ApplicationClaimFactory : UserClaimsPrincipalFactory<Users, Roles>
//    {

//        public ApplicationClaimFactory(UserManager<Users> userManager, RoleManager<Roles> roleManager, IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
//        {
//        }

//        public override async Task<ClaimsPrincipal> CreateAsync(Users user)
//        {
//            var principal = await base.CreateAsync(user);

//            if(principal.FindFirst("FirstName") != null)
//                ((ClaimsIdentity)principal.Identity).RemoveClaim(principal.FindFirst("FirstName"));

//            if (principal.FindFirst("LastName") != null)
//                ((ClaimsIdentity)principal.Identity).RemoveClaim(principal.FindFirst("LastName"));

//            if (principal.FindFirst("FullName") != null)
//                ((ClaimsIdentity)principal.Identity).RemoveClaim(principal.FindFirst("FullName"));

//            if (principal.FindFirst("UserProfile") != null)
//                ((ClaimsIdentity)principal.Identity).RemoveClaim(principal.FindFirst("UserProfile"));

//            if (principal.FindFirst("PhoneNumber") != null)
//                ((ClaimsIdentity)principal.Identity).RemoveClaim(principal.FindFirst("PhoneNumber"));

//            ((ClaimsIdentity)principal.Identity).AddClaims(new[]
//            {
//                 new Claim("FirstName", user.FirstName ?? ""),
//                 new Claim("LastName",  user.LastName ?? ""),
//                 new Claim("PhoneNumber",  user.PhoneNumber ?? ""),
//                 new Claim("FullName",  user.FirstName + " " + user.LastName),
//                 new Claim("UserProfile" , user.ProfilePic ?? "Uploads/UserImage/NoPhoto.jpg")
//            });

//            return principal;
//        }
//    }
//}

using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.UserRoleDTO;
using DataLayer.Entities.Users;
using DataLayer.SSOT;
using DataLayer.ViewModels.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.User
{
    public class UsersRoleRepository : GenericRepository<UserRoles>
    {
        public UsersRoleRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// تنظیم نقش کاربر
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public SweetAlertExtenstion SetRole(SetUserRoleViewModel vm)
        {
            var userRole = TableNoTracking.FirstOrDefault(a => a.UserId == vm.UserId);

            return ResetRole();

            #region LocalMethod

            SweetAlertExtenstion ResetRole()
            {
                if (userRole == null) MapAdd(vm, false);
                else
                {
                    Delete(userRole);
                    MapAdd(vm, false);
                }

                return Save();
            }

            #endregion
        }


        public UserRoles GetRoleByUserId(int userId)
            => GetByCondition(a => a.UserId == userId);


        public async Task<UserRoles> GetRoleByUserIdAsync(int userId)
           =>await GetByConditionAsync(a => a.UserId == userId);


        public List<UserRoleDetailDTO> GetRolesByUserId(int userId)
        {

            return (from userRole in DbContext.UserRoles 
                    join role in DbContext.Roles on userRole.RoleId equals role.Id
                    select new UserRoleDetailDTO {
                        RoleTitle = role.RoleTitle,
                        UserId = userRole.UserId,
                        RoleId = userRole.RoleId
                    }).ToList();
        }

    }
}

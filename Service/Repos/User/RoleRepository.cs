using AutoMapper.QueryableExtensions;
using Core.CustomAttributes;
using Core.Utilities;
using DataLayer.DTO.RolesDTO;
using DataLayer.DTO.UserDTO;
using DataLayer.Entities.Users;
using DataLayer.ViewModels.Role;
using DataLayer.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.User
{
    public class RoleRepository : GenericRepository<Roles>
    {
        public RoleRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

        //public async Task<SweetAlertExtenstion> CheckAndSync(params Assembly[] assemblies)
        //{
        //    var controllerInfo = GetDisplayAndNameController(assemblies);

        //    var roles = TableNoTracking.Select(a => a.Name).ToList();

        //    var roleList = new List<Roles>();

        //    foreach (var item in controllerInfo)
        //    {
        //        roleList.Add(new Roles()
        //        {
        //            Name = item.Name,
        //            ConcurrencyStamp = Guid.NewGuid().ToString(),
        //            NormalizedName = item.Name.ToUpper().Trim(),
        //            RoleTitle = item.GetCustomAttribute<ControllerRoleAttribute>()?.GetName()
        //        });
        //    }

        //    roleList = roleList.Where(a => !roles.Contains(a.Name)).ToList();

        //    await AddRangeAsync(roleList);

        //    return SweetAlertExtenstion.Ok("تمامی اطلاعات ثبت شد");
        //}


        ///// <summary>
        ///// گرفتن تایپ کنترلر هایی مورد نظر
        ///// </summary>
        ///// <param name="assemblies"></param>
        ///// <returns></returns>
        //public List<Type> GetDisplayAndNameController(params Assembly[] assemblies)
        //      => typeof(ControllerRoleAttribute).GetTypesHasAttribute(assemblies).ToList();



        public async Task<Tuple<int, List<RoleManageDTO>>> LoadAsyncCount(
        int skip = -1,
        int take = -1,
        RolesSearchViewModel model = null)
        {
            var query = Entities.Where(a => a.NormalizedName != ImportantNames.AdminNormalTitle()).ProjectTo<RoleManageDTO>();

            

            if (!string.IsNullOrEmpty(model.RoleTitle))
                query = query.Where(x => x.RoleTitle.Contains(model.RoleTitle));


            if (!string.IsNullOrEmpty(model.Name))
                query = query.Where(x => x.Name.Contains(model.Name));

            

            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);

            
            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);



            return new Tuple<int, List<RoleManageDTO>>(Count, await query.ToListAsync());
        }





        public string GetRoleNameByRoleId(int roleId)
              => TableNoTracking.FirstOrDefault(a => a.Id == roleId).NormalizedName;


        /// <summary>
        /// افزودن یک نقش 
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="roleTitle"></param>
        /// <returns></returns>
        public async Task<int> AddRole(RoleInsertViewModel vm)
        {
            var role = new Roles()
            {
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Name = vm.Name,
                NormalizedName = vm.Name.Trim().ToUpper(),
                RoleTitle = vm.RoleTitle
            };

            await AddAsync(role);

            return role.Id;
        }

        /// <summary>
        /// ویرایش یک نقش 
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="roleTitle"></param>
        /// <returns></returns>
        public void UpdateRole(RoleUpdateViewModel vm)
        {
            var model = Map(vm);
            model.NormalizedName = vm.Name.Trim().ToUpper();
            model.ConcurrencyStamp = Guid.NewGuid().ToString();

            DbContext.SaveChanges();
        }

        /// <summary>
        /// چک کردن اینکه این شناسه نقش ادمین است یا خیر
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool IsAdmin(int roleId)
            => TableNoTracking.FirstOrDefault(a => a.Id == roleId).NormalizedName == ImportantNames.AdminNormalTitle();


        
    }
}

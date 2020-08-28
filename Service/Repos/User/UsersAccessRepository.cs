using Core.Utilities;
using DataLayer.Entities.Users;
using DataLayer.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Repos.User
{
    public class UsersAccessRepository : GenericRepository<UsersAccess>
    {
        private readonly RoleRepository _roleRepository;
        private readonly UsersRoleRepository _usersRoleRepository;

        public UsersAccessRepository(DatabaseContext dbContext
            , RoleRepository roleRepository
            , UsersRoleRepository usersRoleRepository) : base(dbContext)
        {
            _roleRepository = roleRepository;
            _usersRoleRepository = usersRoleRepository;
        }

        public bool HasAccess(int role, IDictionary<string, string> path)
        {
            var roleName = _roleRepository.GetRoleNameByRoleId(role);
            if (roleName == ImportantNames.AdminNormalTitle()) return true;

            var userAccess = TableNoTracking.Include(a => a.Roles).Where(a => a.Roles.Id == role).ToList();



            foreach (var item in userAccess)
            {
                if (item.Controller.ToUpper() == path["controller"].ToUpper() + ImportantNames.ControllerName())
                {
                    var actions = item.Actions == null ? null : JsonConvert.DeserializeObject<List<string>>(item.Actions);

                    if (actions != null && actions.Contains(path["action"])) return true;
                }
            }
            return false;
        }


        public bool HasAccess(int roleId, string controller, string action) /*=> true;*/
        {
            var roleName = _roleRepository.GetRoleNameByRoleId(roleId);
            if (roleName == ImportantNames.AdminNormalTitle()) return true;

            var userAccess = TableNoTracking.Where(a => a.RoleId == roleId).ToList();

            foreach (var item in userAccess)
            {
                if (item.Controller.ToUpper() == controller.ToUpper() + ImportantNames.ControllerName())
                {
                    var actions = item.Actions == null ? null : JsonConvert.DeserializeObject<List<string>>(item.Actions);

                    if (actions != null && actions.Contains(action)) return true;
                }
            }
            return false;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public void AddAccessRole(List<UserAccessSubmitViewModel> vm, int roleId)
        {
            foreach (var item in vm)
            {
                if (!string.IsNullOrEmpty(item.Controller) && item.Actions!=null)
                {
                    Add(new UsersAccess()
                    {
                        RoleId = roleId,
                        Actions = item.Actions != null ? JsonConvert.SerializeObject(item.Actions) : null,
                        Controller = item.Controller
                    });
                }
            }
        }

        /// <summary>
        /// زمانی دسترسی های نقشی تغییر پیدا میکند ابتدا باید تمامی نقش های آن پاک شود
        /// </summary>
        /// <param name="roleId"></param>
        public void RemoveAccessRole(int roleId)
        {
            var lstAccessRole = TableNoTracking.Where(a => a.RoleId == roleId);
            if (lstAccessRole.Any()) DeleteRange(lstAccessRole);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public void UpdateAccessRole(List<UserAccessSubmitViewModel> vm, int roleId)
        {
            RemoveAccessRole(roleId);

            foreach (var item in vm)
            {
                if (!string.IsNullOrEmpty(item.Controller) && item.Actions !=null)
                {
                    Add(new UsersAccess()
                    {
                        RoleId = roleId,
                        Actions = item.Actions != null ? JsonConvert.SerializeObject(item.Actions) : null,
                        Controller = item.Controller
                    });
                }
            }
        }

        /// <summary>
        /// لیست تمامی کنترلر ها و اکشن ها که کاربر دسترسی دارد
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<UserAccessListViewModel> GetAllUserAccesss(int userId)
        {
            var lst = new List<UserAccessListViewModel>();
            if (userId != 0)
            {
                var role = _usersRoleRepository.GetRoleByUserId(userId);
                if (role == null) return null;

                if (!_roleRepository.IsAdmin(role.RoleId))
                {

                    var model = TableNoTracking.Where(a => a.RoleId == role.RoleId).ToList();

                    foreach (var item in model)
                    {
                        var actions = JsonConvert.DeserializeObject<List<string>>(item.Actions);

                        foreach (var action in actions)
                        {
                            lst.Add(new UserAccessListViewModel()
                            {
                                Action = action,
                                Controller = item.Controller
                            });
                        }
                    }
                }
                else
                {
                    lst.Add(new UserAccessListViewModel() { IsAdmin = true });
                }
                return lst;
            }
            else return null;
        }
    }
}

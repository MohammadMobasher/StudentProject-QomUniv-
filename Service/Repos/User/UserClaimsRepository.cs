using DataLayer.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.User
{
    public class UserClaimsRepository : GenericRepository<UserClams>
    {
        public UserClaimsRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task RemoveClamsByUserId(int id)
        {
            var model = await Table.Where(a => a.UserId == id).ToListAsync();

            await DeleteRangeAsync(model);
        }
    }
}

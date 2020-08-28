using DataLayer.Entities.TreeInfo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.TreeInfo
{
    public class UserTreeRemindedRepository : GenericRepository<UserTreeReminded>
    {
        public UserTreeRemindedRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }


        /// <summary>
        /// درصورتی که از قبل این کاربر رکوردی داشته باشد 
        /// آن را ویرایش میکند در غیر اینطورت رکوردی برای آن کاربر ثبت میکند
        /// </summary>
        /// <param name="userId">شماره کاربر</param>
        /// <param name="rate">میزان</param>
        /// <returns></returns>
        public async Task<int> Insert(int userId, double rate)
        {
            var entity = await Entities.SingleOrDefaultAsync(x => x.UserId == userId);

            if(entity != null)
            {
                entity.Reminded += rate;
                if(entity.Reminded > 1)
                {
                    var reminded = entity.Reminded % 1;
                    entity.Reminded = reminded;

                    return (int)entity.Reminded / 1;
                }
                return 0;
            }
            else
            {
                await Entities.AddAsync(new UserTreeReminded { UserId = userId, Reminded = rate });
                return 0;
            }
        }

    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.FAQs;
using DataLayer.Entities.FAQs;
using DataLayer.ViewModels.FaqGroup;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.FAQs
{
    public class FaqGroupRepository : GenericRepository<FaqGroup>
    {
        public FaqGroupRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }


        public async Task<Tuple<int, List<FaqGroupDTO>>> LoadAsyncCount(
           int skip = -1,
           int take = -1,
           FaqGroupSearchViewModel model = null)
        {
            var query = Entities.ProjectTo<FaqGroupDTO>();

            if (!string.IsNullOrEmpty(model.Title))
                query = query.Where(x => x.Title.Contains(model.Title));
            

            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);

            return new Tuple<int, List<FaqGroupDTO>>(Count, await query.ToListAsync());
        }



        /// <summary>
        /// ثبت یک آیتم در جدول مورد نظر
        /// </summary>
        /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> AddAsync(FaqGroupInsertViewModel model)
        {

            try
            {
                var entity = Mapper.Map<FaqGroup>(model);
                await AddAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }

        }



        /// <summary>
        /// ثبت یک آیتم در جدول مورد نظر
        /// </summary>
        /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> UpdateAsync(FaqGroupUpdateViewModel model)
        {

            try
            {
                var entity = Mapper.Map<FaqGroup>(model);
                await UpdateAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch (Exception e)
            {
                return SweetAlertExtenstion.Error();
            }

        }


        /// <summary>
        /// ثبت یک آیتم در جدول مورد نظر
        /// </summary>
        /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> DeleteAsync(int Id)
        {

            try
            {
                var entity = await GetByIdAsync(Id);
                await DeleteAsync(entity);
                return SweetAlertExtenstion.Ok("عملیات با موفقیت انجام شد");
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }

        }
    }
}

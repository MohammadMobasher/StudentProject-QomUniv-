using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.Condition;
using DataLayer.Entities;
using DataLayer.ViewModels.Condition;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos
{
    public class ConditionRepository : GenericRepository<Condition>
    {
        public ConditionRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }


        public async Task<Tuple<int, List<ConditionDTO>>> LoadAsyncCount(
           int skip = -1,
           int take = -1,
           ConditionSearchViewModel model = null)
        {
            var query = Entities.ProjectTo<ConditionDTO>();

            if (!string.IsNullOrEmpty(model.Title))
                query = query.Where(x => x.Title.Contains(model.Title));

            if (!string.IsNullOrEmpty(model.Name))
                query = query.Where(x => x.Name.Contains(model.Name));


            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);

            return new Tuple<int, List<ConditionDTO>>(Count, await query.ToListAsync());
        }


        /// <summary>
        /// ثبت یک آیتم در این جدول
        /// </summary>
        /// <param name="model">مدلی که از صفحه به این تابع ارسال می شود</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> InsertAsync(ConditionInsertViewModel model)
        {
            try
            {
                var entity = Mapper.Map<Condition>(model);
                
                await AddAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }

        /// <summary>
        /// گرفتن تمام اطلاعات
        /// </summary>
        /// <returns></returns>
        public async Task<List<ConditionDTO>> GetAllAsync()
        {
            return await Entities.ProjectTo<ConditionDTO>().ToListAsync();
        }


        public async Task<SweetAlertExtenstion> UpdateAsync(ConditionUpdateViewModel model)
        {
            try
            {
                var entity = Mapper.Map<Condition>(model);
                await UpdateAsync(entity);

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

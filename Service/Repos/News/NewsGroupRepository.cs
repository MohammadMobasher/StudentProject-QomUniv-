using DataLayer.DTO;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using AutoMapper.QueryableExtensions;
using System.Linq;
using Core.Utilities;
using DataLayer.ViewModels.NewsGroup;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Service.Repos
{
    public class NewsGroupRepository : GenericRepository<NewsGroup>
    {
        public NewsGroupRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

        public IQueryable ddd()
        {
            
            return this.Entities;
        }

        public async Task<Tuple<int, List<NewsGroupDTO>>> LoadAsyncCount(
            int skip = -1,
            int take = -1,
            NewsGroupSearchViewModel model = null)
        {
            var query = Entities.ProjectTo<NewsGroupDTO>();


            if (model.Id != null)
                query = query.Where(x => x.Id == model.Id);

            if(!string.IsNullOrEmpty(model.Title))
                query = query.Where(x => x.Title.Contains(model.Title));

            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);



            return new Tuple<int, List<NewsGroupDTO>>(Count, await query.ToListAsync());
        }


        /// <summary>
        /// ثبت یک آیتم در جدول مورد نظر
        /// </summary>
        /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> AddAsync(NewsGroupInsertViewModel model)
        {

            try
            {
                var entity = Mapper.Map<NewsGroup>(model);
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
        public async Task<SweetAlertExtenstion> UpdateAsync(NewsGroupUpdateViewModel model)
        {

            try
            {
                var entity = Mapper.Map<NewsGroup>(model);
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
                var entity = new NewsGroup { Id = Id };
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

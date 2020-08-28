using DataLayer.DTO;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using AutoMapper.QueryableExtensions;
using System.Linq;
using System.Threading.Tasks;
using Core.Utilities;
using AutoMapper;
using DataLayer.ViewModels.News;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Service.Repos
{
    public class NewsRepository : GenericRepository<News>
    {
        
        public NewsRepository(DatabaseContext dbContext) : base(dbContext)
        {
            
        }

        /// <summary>
        /// گرفتن 10 خبر محبوب آخر
        /// این تابع بر این اساس است که اخبار موجود را به نزولی مرتب میکند
        /// و 10 خبر آخر را برمیگرداند
        /// </summary>
        /// <returns></returns>
        public List<NewsDTO> GetFavoritNews()
        {
            return this.Entities
                .ProjectTo<NewsDTO>()
                .Where(x=> x.IsActive == true)
                .OrderByDescending(x => x.ViewCount)
                .Skip(0)
                .Take(10)
                .ToList();
        }


        //TODO
        /// <summary>
        /// گرفتن اطلاعات کامل یک خبر
        /// </summary>
        /// <param name="id">شماره خبر</param>
        /// <returns></returns>
        public NewsDTO GetItemDetail(int id)
        {
            return this.Entities.ProjectTo<NewsDTO>().Where(x => x.Id == id).SingleOrDefault();
        }


        //TODO
        /// <summary>
        /// گرفتن اطلاعات کامل یک خبر
        /// </summary>
        /// <param name="id">شماره خبر</param>
        /// <returns></returns>
        public async Task<NewsDTO> GetItemDetailAsync(int id)
        {
            return this.Entities.ProjectTo<NewsDTO>().Where(x => x.Id == id).SingleOrDefault();
        }



        /// <summary>
        /// ثبت اطلاعات در جدول مورد نظر 
        /// </summary>
        /// <param name="model">مدلی که باید به این تابع پاس داده شود  تا بتوان آن را ذخیره کرد</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> Insert(InsertNewsListView model)
        {
            try
            {
                var entity = Mapper.Map<News>(model);

                #region ذخیره فایل مورد نظر

                entity.ImageAddress = await MFile.Save(model.ImageFile, "Uploads/NewsImages");

                #endregion

                entity.Date = DateTime.Now;
                
                await AddAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }


        public async Task<SweetAlertExtenstion> UpdateAsync(NewsUpdateViewModel model)
        {
            try
            {
                var entity = await GetByIdAsync(model.Id);

                entity = (News)Mapper.Map(model, entity, typeof(NewsUpdateViewModel), typeof(News));

                #region ذخیره فایل مورد نظر

                if (model.ImageFile != null)
                {
                    //حذف فایل قبلی
                    await MFile.Delete(entity.ImageAddress);
                    // ذخیره فایل جدید
                    entity.ImageAddress = await MFile.Save(model.ImageFile, "Uploads/NewsImages");
                    
                }

                #endregion

                await DbContext.SaveChangesAsync();
                return SweetAlertExtenstion.Ok();
            }
            catch(Exception e)
            {
                return SweetAlertExtenstion.Error();
            }
        }

        public async Task<SweetAlertExtenstion> ActiveDeactive(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                entity.IsActive = entity.IsActive ? false : true;

                await DbContext.SaveChangesAsync();
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }

        public async Task<Tuple<int, List<NewsDTO>>> LoadAsyncCount(
            int skip = -1,
            int take = -1,
            NewsSearchViewModel model = null)
        {
            var query = Entities.ProjectTo<NewsDTO>();


            if (model.Id != null)
                query = query.Where(x => x.Id == model.Id);

            if (!string.IsNullOrEmpty(model.Title))
                query = query.Where(x => x.Title.Contains(model.Title));


            if (!string.IsNullOrEmpty(model.SummeryNews))
                query = query.Where(x => x.SummeryNews.Contains(model.SummeryNews));


            if (model.Date != DateTime.MinValue)
                query = query.Where(x => x.Date.Equals(model.Date));


            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);



            return new Tuple<int, List<NewsDTO>>(Count, await query.ToListAsync());
        }


        /// <summary>
        /// ثبت یک آیتم در جدول مورد نظر
        /// </summary>
        /// <param name="Id">شماره خبر</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> DeleteAsync(int Id)
        {

            try
            {
                var entity = await GetByIdAsync(Id);

                await MFile.Delete(entity.ImageAddress);

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

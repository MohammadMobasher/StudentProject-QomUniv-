using DataLayer.Entities;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DataLayer.DTO;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using DataLayer.ViewModels.SlideShow;
using System.Threading.Tasks;
using System;
using Core.Utilities;
using AutoMapper;

namespace Service.Repos
{
    public class SlideShowRepository : GenericRepository<SlideShow>
    {
        private readonly DatabaseContext _context;   

        public SlideShowRepository(DatabaseContext db) : base(db)
        {
            this._context = db;
        }



        /// <summary>
        /// گرفتن 5 آیتم آخر این جدول
        /// </summary>
        /// <param name="count"></param>
        public List<SlideShowDTO> GetLastItems(int count = 5)
        {
            
            return this._context.SlideShow.Where(x => x.IsActive == true)
                .ProjectTo<SlideShowDTO>()
                .ToList();



        }



        public async Task<Tuple<int, List<SlideShowDTO>>> LoadAsyncCount(
           int skip = -1,
           int take = -1,
           SlideShowSearchViewModel model = null)
        {
            var query = Entities.ProjectTo<SlideShowDTO>();

            if (!string.IsNullOrEmpty(model.Title))
                query = query.Where(x => x.Title.Contains(model.Title));

            

            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);

            return new Tuple<int, List<SlideShowDTO>>(Count, await query.ToListAsync());
        }



        /// <summary>
        /// ثبت یک آیتم در جدول مورد نظر
        /// </summary>
        /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> AddAsync(SlideShowInsertViewModel model)
        {

            try
            {
                var entity = Mapper.Map<SlideShow>(model);

                #region ذخیره فایل مورد نظر

                entity.ImgAddress = await MFile.Save(model.ImageFile, "Uploads/SlideShow");

                #endregion

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
        public async Task<SweetAlertExtenstion> UpdateAsync(SlideShowUpdateViewModel model)
        {

            try
            {
                var entity = Mapper.Map<SlideShow>(model);

                #region ذخیره فایل مورد نظر

                if (model.ImageFile != null)
                {
                    //حذف فایل قبلی
                    await MFile.Delete(entity.ImgAddress);
                    // ذخیره فایل جدید
                    entity.ImgAddress = await MFile.Save(model.ImageFile, "Uploads/SlideShow");

                }

                #endregion

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
                await MFile.Delete(entity.ImgAddress);
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

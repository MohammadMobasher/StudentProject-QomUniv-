using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.LogoManufactory;
using DataLayer.Entities;
using DataLayer.ViewModels.LogoManufactory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos
{
    public class LogoManufactoryRepository : GenericRepository<LogoManufactory>
    {
        public LogoManufactoryRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<Tuple<int, List<LogoManufactoryDTO>>> LoadAsyncCount(
           int skip = -1,
           int take = -1,
           LogoManufactorySearchViewModel model = null)
        {
            var query = Entities.ProjectTo<LogoManufactoryDTO>();

            if (!string.IsNullOrEmpty(model.Title))
                query = query.Where(x => x.Title.Contains(model.Title));



            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);

            return new Tuple<int, List<LogoManufactoryDTO>>(Count, await query.ToListAsync());
        }


        /// <summary>
        /// ثبت اطلاعات در جدول مورد نظر 
        /// </summary>
        /// <param name="model">مدلی که باید به این تابع پاس داده شود  تا بتوان آن را ذخیره کرد</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> Insert(InsertLogoManufactoryListView model)
        {
            try
            {
                var entity = Mapper.Map<LogoManufactory>(model);

                #region ذخیره فایل مورد نظر

                entity.AddressImg = await MFile.Save(model.ImageFile, "Uploads/LogoManufactoryImages");

                #endregion


                await AddAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }


        public async Task<SweetAlertExtenstion> Update(UpdateLogoManufactoryViewModel model)
        {
            try
            {
                var entity = await GetByIdAsync(model.Id);

                entity = (LogoManufactory)Mapper.Map(model, entity, typeof(UpdateLogoManufactoryViewModel), typeof(LogoManufactory));

                #region ذخیره فایل مورد نظر

                if (model.ImageFile != null)
                {
                    //حذف فایل قبلی
                    await MFile.Delete(entity.AddressImg);
                    // ذخیره فایل جدید
                    entity.AddressImg = await MFile.Save(model.ImageFile, "Uploads/LogoManufactoryImages");
                }

                #endregion

                await DbContext.SaveChangesAsync();
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
        public async Task<SweetAlertExtenstion> Delete(int Id)
        {

            try
            {
                var entity = await GetByIdAsync(Id);
                await MFile.Delete(entity.AddressImg);
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

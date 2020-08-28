using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.ProductGroupDependencies;
using DataLayer.Entities;
using DataLayer.ViewModels.ProductGroupDependencies;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.Product
{
    public class ProductGroupDependenciesRepository : GenericRepository<ProductGroupDependencies>
    {
        public ProductGroupDependenciesRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }


        public async Task<Tuple<int, List<ProductGroupDependenciesFullDTO>>> LoadAsyncCount(
         int skip = -1,
         int take = -1,
         ProductGroupDependenciesSearchViewModel model = null)
        {



            var query = (from productGroupDependencies in DbContext.ProductGroupDependencies
                             // ارتباط برای گروه وابسته
                         join productGroup1 in DbContext.ProductGroup on productGroupDependencies.GroupId1 equals productGroup1.Id
                         // برای ویژگی از جدول وابسته
                         join feature1 in DbContext.Feature on productGroupDependencies.Feature1 equals feature1.Id
                         //join featureItem1 in DbContext.FeatureItem on feature1.Id equals featureItem1.FeatureId
                         //where featureItem1.Id == productGroupDependencies.Value1

                         // ارتباط برای جدولی که وابستگی به آن است
                         join productGroup2 in DbContext.ProductGroup on productGroupDependencies.GroupId2 equals productGroup2.Id
                          // برای ویژگی از جدول وابسته
                          join feature2 in DbContext.Feature on productGroupDependencies.Feature2 equals feature2.Id
                         

                          select new ProductGroupDependenciesFullDTO
                          {
                              Id = productGroupDependencies.Id,

                              Title = productGroupDependencies.Title,

                              #region جدول اول
                              GroupId1 = productGroupDependencies.GroupId1,
                              GroupId1Title = productGroup1.Title,
                              Feature1 = productGroupDependencies.Feature1,
                              Feature1Title = feature1.Title,
                              Value1 = productGroupDependencies.Value1,
                              //FeatureValueSelected = (featureItem1.Value == )
                              #endregion

                              #region جدول دوم
                              GroupId2 = productGroupDependencies.GroupId2,
                              GroupId2Title = productGroup2.Title,
                              Feature2 = productGroupDependencies.Feature2,
                              Feature2Title = feature2.Title,
                              Value2 = productGroupDependencies.Value2,
                              #endregion

                              ConditionId = productGroupDependencies.ConditionId,
                              ConditionTitle = productGroupDependencies.Condition.Title


                          });


            // شرط 
            if (model.ConditionId != null)
                query = query.Where(x => x.ConditionId == model.ConditionId);

            if (!string.IsNullOrEmpty(model.Title))
                query = query.Where(x => x.Title.Contains(model.Title));


            #region جستجو در آیتم‌های مربوط به گروه اول
            if (model.GroupId1 != null)
                query = query.Where(x => x.GroupId1 == model.GroupId1);

            if (model.Feature1 != null)
                query = query.Where(x => x.Feature1 == model.Feature1);

            if(!string.IsNullOrEmpty(model.Value1))
                query = query.Where(x => x.Value1.Contains(model.Value1));
            #endregion


            #region جستجو در آیتم‌های مربوط به گروه دوم
            if (model.GroupId2 != null)
                query = query.Where(x => x.GroupId2 == model.GroupId2);

            if (model.Feature2 != null)
                query = query.Where(x => x.Feature2 == model.Feature2);

            if (!string.IsNullOrEmpty(model.Value2))
                query = query.Where(x => x.Value2.Contains(model.Value2));
            #endregion


            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);

            return new Tuple<int, List<ProductGroupDependenciesFullDTO>>(Count, await query.ToListAsync());
        }



        /// <summary>
        /// ثبت یک آیتم در این جدول
        /// </summary>
        /// <param name="model">مدلی که به این تابع باید پاس داده شود</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> InsertAsync(ProductGroupDependenciesInsertViewModel model)
        {

            try
            {
                var entity = Mapper.Map<ProductGroupDependencies>(model);
                await AddAsync(entity);

                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }

        }


        /// <summary>
        /// به روز رسانی یک آیتم در این جدول
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> UpdateAsync(ProductGroupDependenciesUpdateViewModel model)
        {
            try
            {
                var entity = Mapper.Map<ProductGroupDependencies>(model);
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

        /// <summary>
        /// حذف یک آیتم در جدول مورد نظر
        ///  با استفاده شماره ویژگی مورد نظر
        /// </summary>
        /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> DeleteByFeatureIdAsync(int FeatureId)
        {
            try
            {
                var entities = await Entities.Where(x => x.Feature1 == FeatureId || x.Feature2 == FeatureId).ToListAsync();
                await DeleteRangeAsync(entities);
                return SweetAlertExtenstion.Ok("عملیات با موفقیت انجام شد");
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }


        /// <summary>
        /// حذف یک آیتم در جدول مورد نظر
        ///  با استفاده شماره ویژگی مورد نظر
        /// </summary>
        /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> DeleteByFeatureIdAndGroupIdAsync(int FeatureId, int GroupId)
        {
            try
            {
                var entities = await Entities.Where(x => (x.Feature1 == FeatureId && x.GroupId1 == GroupId) 
                    || (x.Feature2 == FeatureId && x.GroupId2 == GroupId)).ToListAsync();
                await DeleteRangeAsync(entities);
                return SweetAlertExtenstion.Ok("عملیات با موفقیت انجام شد");
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }

    }
}

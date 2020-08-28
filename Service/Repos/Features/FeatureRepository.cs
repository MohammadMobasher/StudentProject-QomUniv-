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
using DataLayer.ViewModels.Feature;
using Service.Repos.Product;
using DataLayer.DTO.Feature;
using DataLayer.ViewModels;

namespace Service.Repos
{
    public class FeatureRepository : GenericRepository<Feature>
    {
        private readonly FeatureItemRepository _featureItemRepository;
        private readonly ProductFeatureRepository _productFeatureRepository;
        private readonly ProductGroupFeatureRepository _productGroupFeatureRepository;
        private readonly ProductGroupDependenciesRepository _productGroupDependenciesRepository;

        public FeatureRepository(DatabaseContext dbContext,
            FeatureItemRepository featureItemRepository,
            ProductFeatureRepository productFeatureRepository,
            ProductGroupFeatureRepository productGroupFeatureRepository,
            ProductGroupDependenciesRepository productGroupDependenciesRepository)  : base(dbContext)
        {
            _featureItemRepository = featureItemRepository;
            _productFeatureRepository = productFeatureRepository;
            _productGroupFeatureRepository = productGroupFeatureRepository;
            _productGroupDependenciesRepository = productGroupDependenciesRepository;
        }




        //TODO
        /// <summary>
        /// گرفتن اطلاعات کامل یک خبر
        /// </summary>
        /// <param name="id">شماره خبر</param>
        /// <returns></returns>
        public FeatureFullDetailDTO GetItemDetail(int id)
        {
            return this.Entities.ProjectTo<FeatureFullDetailDTO>().Where(x => x.Id == id).SingleOrDefault();
        }


        //TODO
        /// <summary>
        /// گرفتن اطلاعات کامل یک خبر
        /// </summary>
        /// <param name="id">شماره خبر</param>
        /// <returns></returns>
        public async Task<FeatureFullDetailDTO> GetItemDetailAsync(int id)
        {
            var result = this.Entities.ProjectTo<FeatureFullDetailDTO>().Where(x => x.Id == id).SingleOrDefault();
            return result;
        }

        /// <summary>
        /// ثبت اطلاعات در جدول مورد نظر 
        /// </summary>
        /// <param name="model">مدلی که باید به این تابع پاس داده شود  تا بتوان آن را ذخیره کرد</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> Insert(FeatureInsertViewModel model)
        {
            try
            {
                var entity = Mapper.Map<Feature>(model);


                await AddAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }

        /// <summary>
        /// ثبت اطلاعات در جدول مورد نظر 
        /// </summary>
        /// <param name="model">مدلی که باید به این تابع پاس داده شود  تا بتوان آن را ذخیره کرد</param>
        /// <returns></returns>
        public async Task<int?> InsertFeature(FeatureInsertViewModel model)
        {
            try
            {
                var entity = Mapper.Map<Feature>(model);


                await AddAsync(entity);
                return entity.Id;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// ویرایش یک ویژگی 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="vmItems"></param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> UpdateAsync(FeatureUpdateViewModel model, FeatureItemsViewModel vmItems)
        {
            try
            {
                var entity = await GetByIdAsync(model.Id);
                entity = (Feature)Mapper.Map(model, entity, typeof(FeatureUpdateViewModel), typeof(Feature));

                if (vmItems != null && vmItems.Items.Count > 0)
                {
                    //ویرایش آیتم‌‌ها
                    await _featureItemRepository.UpdateAsync(model.Id, vmItems.Items);
                    // حذف این ویژگی برای تمام محصولات
                    await _productFeatureRepository.DeleteAsync(model.Id);
                }

                await DbContext.SaveChangesAsync();
                return SweetAlertExtenstion.Ok();
            }
            catch (Exception e)
            {
                return SweetAlertExtenstion.Error();
            }
        }


        public async Task<Tuple<int, List<FeatureFullDetailDTO>>> LoadAsyncCount(
            int skip = -1,
            int take = -1,
            FeatureSearchViewModel model = null)
        {
            var query = Entities.ProjectTo<FeatureFullDetailDTO>();

            if (!string.IsNullOrEmpty(model.Title))
                query = query.Where(x => x.Title.Contains(model.Title));


            if (model.FeatureType != null)
                query = query.Where(x => x.FeatureType == model.FeatureType);


            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);



            return new Tuple<int, List<FeatureFullDetailDTO>>(Count, await query.ToListAsync());
        }

        public async Task<SweetAlertExtenstion> ShowInSearchSite(int Id)
        {
            try
            {
                var entity = await GetByIdAsync(Id);
                if (entity.ShowForSearch)
                    entity.ShowForSearch = false;
                else
                    entity.ShowForSearch = true;

                await DbContext.SaveChangesAsync();

                return SweetAlertExtenstion.Ok();
            }
            catch(Exception e)
            {
                return SweetAlertExtenstion.Error();
            }
        }


        /// <summary>
        /// حذف یک آیتم در جدول مورد نظر
        /// </summary>
        /// <param name="Id">شماره آیتم</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> DeleteAsync(int Id)
        {

            try
            {
                var entity = await GetByIdAsync(Id);

                // removing ProductFeature
                await _productFeatureRepository.DeleteAsync(Id);
                // remove this feature from ProductGroupFeature
                await _productGroupFeatureRepository.DeleteFeatureFromAllGroup(Id);

                // باید از وابستگی‌ها هم حذف بشه
                await _productGroupDependenciesRepository.DeleteByFeatureIdAsync(Id);

                await DeleteAsync(entity);
                return SweetAlertExtenstion.Ok("عملیات با موفقیت انجام شد");
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }

        public async Task<List<Feature>> GetFeaturesByListFeatureId(List<int> featureIds)
             => await TableNoTracking.Include(a => a.Features).Where(a => featureIds.Contains(a.Id)).ToListAsync();


        /// <summary>
        /// گرفتن تمام آیتم‌ها
        /// </summary>
        /// <returns></returns>
        public async Task<List<FeatureIdTitleDTO>> GetAllAsync()
        {
            return await Entities.ProjectTo<FeatureIdTitleDTO>().ToListAsync();
        }

    }
}

using AutoMapper;
using DataLayer.Entities;
using Core.Utilities;
using DataLayer.DTO;
using DataLayer.DTO.Feature;
using DataLayer.DTO.FeatureItem;
using DataLayer.DTO.ProductGroupFeature;

using DataLayer.ViewModels.ProductGroup;
using DataLayer.ViewModels.ProductGroupFeature;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.ViewModels.Products;
using System.Data;
using Dapper;

namespace Service.Repos.Product
{
    public class ProductGroupFeatureRepository : GenericRepository<ProductGroupFeature>
    {
        private readonly ProductFeatureRepository _productFeatureRepository;
        private readonly ProductRepostitory _productRepostitory;
        private readonly ProductGroupDependenciesRepository _productGroupDependenciesRepository;
        private readonly IDbConnection _connection;

        public ProductGroupFeatureRepository(DatabaseContext dbContext,
            ProductFeatureRepository productFeatureRepository,
            ProductRepostitory productRepostitory,
            ProductGroupDependenciesRepository productGroupDependenciesRepository,
            IDbConnection connection) : base(dbContext)
        {
            _productFeatureRepository = productFeatureRepository;
            _productRepostitory = productRepostitory;
            _productGroupDependenciesRepository = productGroupDependenciesRepository;
            _connection = connection;
        }

        public async Task<Tuple<int, List<ProductGroupFeatureDTO>>> LoadAsyncCount(
            int productGroupId,
           int skip = -1,
           int take = -1,
           ProductGroupFeatureSearchViewModel model = null)
        {

            var query = (from productGroupFeature in this.DbContext.ProductGroupFeature
                         where productGroupFeature.ProductGroupId == productGroupId
                         join feature in this.DbContext.Feature on productGroupFeature.FeatureId equals feature.Id
                         orderby productGroupFeature.Id
                         select new ProductGroupFeatureDTO
                         {
                             Id = productGroupFeature.Id,
                             ProductGroupId = productGroupId,
                             FeatureId = feature.Id,
                             FeatureTitle = feature.Title

                         });

            if (!string.IsNullOrEmpty(model.FeatureTitle))
                query = query.Where(x => x.FeatureTitle.Contains(model.FeatureTitle));


            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);

            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);

            return new Tuple<int, List<ProductGroupFeatureDTO>>(Count, await query.ToListAsync());
        }


        /// <summary>
        /// در این تابع لیست ویژگی‌هایی برگردانده می‌شود که متعلق به یک گروه نباشد
        /// لیستی که برمیگرداند حاوی شماره ویژگی به همراه عنوان ویژگی است
        /// </summary>
        /// <param name="productGroupId">شماره گروه مورد نظر</param>
        /// <returns></returns>
        public async Task<List<FeatureIdTitleDTO>> GetOtherFeaturesByGroupId(int productGroupId)
        {
            // لیست ویژگی‌هایی که این گروه دارد
            var hasFeature = (from productGroupFeature in this.DbContext.ProductGroupFeature
                              where productGroupFeature.ProductGroupId == productGroupId
                              join feature in this.DbContext.Feature on productGroupFeature.FeatureId equals feature.Id
                              orderby productGroupFeature.Id
                              select feature.Id);

            // لیست ویژگی‌هایی که این گروه ندارد
            var query = await (from feature_ in this.DbContext.Feature
                               where !hasFeature.Contains(feature_.Id)
                               select new FeatureIdTitleDTO
                               {
                                   Id = feature_.Id,
                                   Title = feature_.Title
                               }).ToListAsync();

            return query;
        }


        /// <summary>
        /// حذف یک آیتم از این جدول
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> DeleteAsync(int id)
        {
            try
            {
                //==============================================================================
                // برای حذف ابتدا باید از داخل جدولی مقادیر مربوط به ویژگی‌ها برای محصولات است را 
                // واکشی کرده وسپس پاک کنیم
                // محصولاتی که اولا مربوط به آن گروه باشد دوما دارای آن ویژگی باشد  
                List<ProductFeatureDeleteFeatureIdProductId> ItemsForDelete = new List<ProductFeatureDeleteFeatureIdProductId>();
                //==============================================================================

                var entity = await GetByIdAsync(id);


                // شماره محصولاتی که در این گروه قرار میگیرند
                List<int> products = await _productRepostitory.GetProductIdsByGroupId(entity.ProductGroupId);
                if(products != null)
                {
                    foreach (var item in products)
                        ItemsForDelete.Add(new ProductFeatureDeleteFeatureIdProductId { ProductId = item, FeatureId = entity.FeatureId });
                    var result = await _productFeatureRepository.DeleteAsync(ItemsForDelete);
                    if (result.Succeed == true)
                        await DeleteAsync(entity);
                    else
                        return SweetAlertExtenstion.Error();
                }

                // حذف از جدول وابستگی‌ها
                await _productGroupDependenciesRepository.DeleteByFeatureIdAndGroupIdAsync(entity.FeatureId, entity.ProductGroupId);
                
                return SweetAlertExtenstion.Ok("عملیات با موفقیت انجام شد");
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }

        /// <summary>
        // حذف یک ویژگی از تمام گروه‌ها
        /// </summary>
        /// <param name="id">شماره ویژگی مورد نظر</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> DeleteFeatureFromAllGroup(int FeatureId)
        {

            var Groups = await Entities.Where(x => x.FeatureId == FeatureId).ToListAsync();
            try
            {
                if (Groups == null)
                    await DeleteRangeAsync(Groups);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }


        }



        /// <summary>
        /// تعداد گروه‌هایی که یک ویژگی‌ خاص را دارند
        /// </summary>
        /// <param name="id">شماره ویژگی مورد نظر</param>
        /// <returns></returns>
        public async Task<int> NumberGroupHasFeature(int FeatureId)
        {

            var Groups = await Entities.Where(x => x.FeatureId == FeatureId).ToListAsync();
            if (Groups == null)
                return 0;
            else
                return Groups.Count;

        }


        public async Task<List<FeatureIdTitleDTO>> GetFeaturesByGroupIdRecAsync(int productGroupId)
        {
            try
            {

                string query = @"
                    declare @T table(Id int);
                        with A as (
                           select Id, ParentId
                               from ProductGroup
                               where Id = " + productGroupId + @"
                               union all
                           select c.Id, c.ParentId
                               from ProductGroup c
                                   join A p on p.Id = c.ParentId) 
                    insert into @T(Id) select Id from A;

                    select distinct Feature.Id, Feature.Title from ProductGroupFeature 
                    join Feature on ProductGroupFeature.FeatureId = Feature.Id
                    where ProductGroupId in (select Id from @T) and Feature.FeatureType = 2
                ";
                var results = await _connection.QueryMultipleAsync(query);

                var resultData = await results.ReadAsync<FeatureIdTitleDTO>();
                return resultData.ToList();

            }
            catch(Exception e)
            {
                return new List<FeatureIdTitleDTO>();
            }
        }



        /// <summary>
        /// در این تابع لیست ویژگی‌هایی برگردانده می‌شود که متعلق به یک گروه باشد
        /// </summary>
        /// <param name="productGroupId">شماره گروه مورد نظر</param>
        /// <returns></returns>
        public async Task<List<FeatureFullDetailDTO>> GetFeaturesByGroupId(int productGroupId)
        {
            // لیست ویژگی‌هایی که این گروه دارد
            return await (from productGroupFeature in this.DbContext.ProductGroupFeature
                          where productGroupFeature.ProductGroupId == productGroupId

                          join feature in this.DbContext.Feature on productGroupFeature.FeatureId equals feature.Id

                          orderby productGroupFeature.Id

                          select new FeatureFullDetailDTO
                          {

                              Id = feature.Id,
                              Title = feature.Title,
                              FeatureType = feature.FeatureType,
                              IsRequired = feature.IsRequired,
                              FeatureItems = (from featureItem in this.DbContext.FeatureItem
                                              where featureItem.FeatureId == feature.Id
                                              select new FeatureItemDTO
                                              {
                                                  Id = featureItem.Id,
                                                  Value = featureItem.Value,
                                                  FeatureId = feature.Id
                                              }).ToList()
                          }).ToListAsync();
        }

        public async Task<List<FeatureFullDetailDTO>> SearchableFeatureByGroupId(int groupId)
        {
            
            var sql = $@"with A as (
                           select Id, ParentId
                               from ProductGroup
                               where Id = {groupId}
                               union all
                           select c.Id, c.ParentId
                               from ProductGroup c
                                   join A p on p.Id = c.ParentId) 
                        select Id from A
                ";
            try
            {
                var model = await _connection.QueryAsync<int>(sql);
                model = model.ToList();

                return (from productGroupFeature in this.DbContext.ProductGroupFeature
                              where model.Contains(productGroupFeature.ProductGroupId)

                              join feature in this.DbContext.Feature on productGroupFeature.FeatureId equals feature.Id
                              where feature.ShowForSearch == true

                              orderby productGroupFeature.Id

                              

                              select new FeatureFullDetailDTO
                              {

                                  Id = feature.Id,
                                  Title = feature.Title,
                                  FeatureType = feature.FeatureType,
                                  IsRequired = feature.IsRequired,
                                  FeatureItems = (from featureItem in this.DbContext.FeatureItem
                                                  where featureItem.FeatureId == feature.Id
                                                  select new FeatureItemDTO
                                                  {
                                                      Id = featureItem.Id,
                                                      Value = featureItem.Value,
                                                      FeatureId = feature.Id
                                                  }).ToList()
                              }).DistinctBy(x=> x.Id).ToList();


            }
            catch
            {
                return new List<FeatureFullDetailDTO>();
            }
        }


        /// <summary>





        /// <summary>
        /// ثبت یک آیتم در جدول مورد نظر
        /// </summary>
        /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> AddAsync(ProductGroupFeatureInsertViewModel model)
        {
            try
            {
                var entity = Mapper.Map<ProductGroupFeature>(model);
                await AddAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch (Exception E)
            {
                return SweetAlertExtenstion.Error();
            }

        }


        //    /// <summary>
        //    /// ثبت یک آیتم در جدول مورد نظر
        //    /// </summary>
        //    /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        //    /// <returns></returns>
        //    public async Task<SweetAlertExtenstion> UpdateAsync(ProductGroupUpdateViewModel model)
        //    {

        //        try
        //        {
        //            var entity = Mapper.Map<ProductGroup>(model);
        //            await UpdateAsync(entity);
        //            return SweetAlertExtenstion.Ok();
        //        }
        //        catch
        //        {
        //            return SweetAlertExtenstion.Error();
        //        }

        //    }


        //    /// <summary>
        //    /// ثبت یک آیتم در جدول مورد نظر
        //    /// </summary>
        //    /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        //    /// <returns></returns>
        //    public async Task<SweetAlertExtenstion> DeleteAsync(int Id)
        //    {

        //        try
        //        {
        //            var entity = new ProductGroup { Id = Id };
        //            await DeleteAsync(entity);
        //            return SweetAlertExtenstion.Ok("عملیات با موفقیت انجام شد");
        //        }
        //        catch
        //        {
        //            return SweetAlertExtenstion.Error();
        //        }

        //    }
        //}

        /// <summary>
        /// تمامی ویژگی های گروه یک محصول
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProductGroupFeature>> GetAllProductGroupFeature(int groupId)
            => await TableNoTracking.Include(a => a.Feature).Where(a => a.ProductGroupId == groupId).ToListAsync();


    }
}

using Core.Utilities;
using DataLayer.Entities;
using DataLayer.SSOT;
using DataLayer.ViewModels.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.Product
{
    public class ProductFeatureRepository : GenericRepository<ProductFeature>
    {
        public ProductFeatureRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// افزودن گروهی ویژگی ها
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> AddFeatureRange(ProductFeatureInsertViewModel vm)
        {
            var lst = new List<ProductFeature>();
            if (vm.Items != null && vm.Items.Count > 0)
            {
                foreach (var item in vm.Items)
                {
                    lst.Add(new ProductFeature()
                    {
                        FeatureId = item.FeatureId,
                        FeatureValue = item.FeatureValue,
                        ProductId = vm.ProductId
                    });
                }
                await AddRangeAsync(lst);
            }
            return SweetAlertExtenstion.Ok();
        }

        /// <summary>
        /// افزودن گروهی ویژگی ها
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> UpdateFeatureRange(ProductFeatureInsertViewModel vm)
        {
            DeleteFeatures(vm.ProductId);

            var lst = new List<ProductFeature>();

            foreach (var item in vm.Items)
            {
                lst.Add(new ProductFeature()
                {
                    FeatureId = item.FeatureId,
                    FeatureValue = item.FeatureValue,
                    ProductId = vm.ProductId
                });
            }
            await AddRangeAsync(lst);
            return SweetAlertExtenstion.Ok();
        }

        void DeleteFeatures(int id)
        {
            var model = TableNoTracking.Where(a => a.ProductId == id).ToList();

            DbContext.RemoveRange(model);
        }

        /// <summary>
        /// گرفتن اطلاعات ویژگی های محصولات بر اساس شناسه محصولات
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<List<ProductFeature>> GetAllProductFeatureByProductId(int productId)
            => await TableNoTracking.Where(a => a.ProductId == productId).ToListAsync();


        /// <summary>
        /// تعداد گروه‌هایی که یک ویژگی‌ خاص را دارند
        /// </summary>
        /// <param name="id">شماره ویژگی مورد نظر</param>
        /// <returns></returns>
        public async Task<int> NumberProductHasFeature(int FeatureId)
        {

            var Groups = await Entities.Where(x => x.FeatureId == FeatureId).ToListAsync();
            if (Groups == null)
                return 0;
            else
                return Groups.Count;

        }


        /// <summary>
        /// حذف یک آیتم بر اساس شماره ویژگی و شماره محصول
        /// </summary>
        /// <param name="FeatureId"></param>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> DeleteAsync(int FeatureId, int ProductId)
        {
            try
            {
                var entity = await GetByConditionAsync(x => x.FeatureId == FeatureId && x.ProductId == ProductId);
                await DeleteAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }


        /// <summary>
        /// حذف یک آیتم بر اساس شماره ویژگی و شماره محصول
        /// </summary>
        /// <param name="FeatureId"></param>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> DeleteAsync(int FeatureId)
        {
            try
            {
                var entity = await Entities.Where(x=> x.FeatureId == FeatureId).ToListAsync();
                await DeleteRangeAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }



        /// <summary>
        /// حذف یک آیتم بر اساس شماره ویژگی و شماره محصول
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> DeleteAsync(List<ProductFeatureDeleteFeatureIdProductId> items)
        {
            try
            {
                List<ProductFeature> realItems = new List<ProductFeature>(); 
                foreach (var item in items)
                    realItems.Add(Entities.SingleOrDefault(x => x.ProductId == item.ProductId && x.FeatureId == item.FeatureId));

                await DeleteRangeAsync(realItems);
                return SweetAlertExtenstion.Ok();
            }
            catch(Exception e)
            {
                return SweetAlertExtenstion.Error();
            }
        }


        /// <summary>
        /// گرفتن اطلاعات ویژگی های محصول بر اساس شناسه محصول
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<ProductFeature>> GetFeaturesByProductId(int id)
        {
            var model = await TableNoTracking.Include(a => a.Feature)
                .Where(a => a.ProductId == id).ToListAsync();

            return model;
        }

        public List<int> FeaturesWithSSOTType(List<ProductFeature> productFeatures)
        {
            var model = productFeatures.Where(y => y.Feature.FeatureType == FeatureTypeSSOT.Fssot)
                .Select(z => z.FeatureId).ToList();

            return model;
        }
    }
}

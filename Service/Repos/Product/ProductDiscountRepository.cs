using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.ProductDiscounts;
using DataLayer.Entities;
using DataLayer.SSOT;
using DataLayer.ViewModels.ProductDiscount;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.Product
{
    public class ProductDiscountRepository : GenericRepository<ProductDiscount>
    {
        public ProductDiscountRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }


        public async Task<List<ProductDiscountDTO>> DiscountToAll(Expression<Func<ProductDiscount, bool>> where = null
            , Func<IQueryable<ProductDiscount>, IOrderedQueryable<ProductDiscount>> orderBy = null)
                => await TableNoTracking.WhereIf(where != null, where).OrderByIf(orderBy != null, orderBy)
                         .ProjectTo<ProductDiscountDTO>().ToListAsync();


        /// <summary>
        /// آیا این تخفیف برای این محصول قبلا ثبت شده است یا خیر 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> IsProductSubmited(int id)
        {
            if (await IsExpired(id)) return false;

            return await GetByConditionAsync(a => a.ProductId == id) != null;
        }


        /// <summary>
        /// آیا این تخفیف برای این گروه محصول قبلا ثبت شده است یا خیر 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> IsProductGroupSubmited(int id)
        {
            if (await IsGroupExpired(id)) return false;


            return await GetByConditionAsync(a => a.ProductGroupId == id) != null;
        }


        public async Task UpdateDiscount(ProductDiscountUpdateViewModel vm)
        {
            var model = await GetByIdAsync(vm.Id);

            Mapper.Map(vm, model);

            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// محاسبه تخفیف
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<ProductDiscount> CalculatePrice(int? productId, int? groupId)
        {
            if (productId != null) return await CulculateProduct();
            else if (groupId != null) return await CalculateGroupProduct();
            return await CalculateAll();

            #region LocalMethods
            // بررسی و اعمال تخفیف روی محصولات
            async Task<ProductDiscount> CulculateProduct()
            {
                var model = await TableNoTracking.FirstOrDefaultAsync(a => a.ProductId == productId.Value);

                if (model == null) return await CalculateGroupProduct();

                return model;
            }

            // بررسی و اعمال تخفیف رو گروه مخحصولات
            async Task<ProductDiscount> CalculateGroupProduct()
            {
                var model = await TableNoTracking.FirstOrDefaultAsync(a => a.ProductGroupId == groupId.Value);

                if (model == null) return await CalculateAll();

                return model;
            }

            // بررسی و اعمال تخفیف رو تمامی محصولات
            async Task<ProductDiscount> CalculateAll()
               => await TableNoTracking.FirstOrDefaultAsync(a => a.ProductGroupId == null && a.ProductId == null);

            #endregion
        }


        /// <summary>
        /// حذف با استفاده از لیست شماره محصولات
        /// </summary>
        /// <param name="productIds">شماره محصولاتی که تخفیف‌‌های مربوط به آنها باید از بین برود</param>
        /// <returns></returns>
        public async Task<bool> DeleteByProductIds(List<int> productIds)
        {
            try
            {
                var result = await Entities.Where(x => productIds.Contains(x.ProductId.Value)).ToListAsync();
                await DeleteRangeAsync(result);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        /// <summary>
        /// چک کردن انقضای تخفیف
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> IsExpired(int id)
        {
            var model = await TableNoTracking.LastOrDefaultAsync(a => a.ProductId == id && a.IsActive == true);

            return model == null || DateTime.Now > model.EndDate;

        }


        /// <summary>
        /// چک کردن انقضای تخفیف
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> IsGroupExpired(int id)
        {
            var model = await TableNoTracking.LastOrDefaultAsync(a => a.ProductGroupId == id && a.IsActive == true);

            return model == null || DateTime.Now > model.EndDate;

        }


        public async Task<SweetAlertExtenstion> ArchiveDiscount(int id)
        {
            var model = await GetByIdAsync(id);

            model.IsArchive = true;
            model.IsActive = false;

            await UpdateAsync(model, false);

            return await SaveAsync();
        }


    }
}

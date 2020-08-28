using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.Feature;
using DataLayer.DTO.FeatureItem;
using DataLayer.DTO.Products;
using DataLayer.Entities;
using DataLayer.ViewModels.Products;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Service.Repos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos
{
    public class ProductPackageDetailsRepostitory : GenericRepository<DataLayer.Entities.ProductPackageDetails>
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ProductDiscountRepository _productDiscountRepository;
        private readonly ProductRepostitory _productRepostitory;

        public ProductPackageDetailsRepostitory(DatabaseContext dbContext,
            IHostingEnvironment hostingEnvironment,
            ProductDiscountRepository productDiscountRepository, ProductRepostitory productRepostitory) : base(dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _productDiscountRepository = productDiscountRepository;
            _productRepostitory = productRepostitory;
        }


        #region 


        /// <summary>
        /// محاسبه قیمت کلی محصول
        /// </summary>
        /// <param name="packageId"></param>
        /// <returns></returns>
        public async Task<string> CalculatePrice(int packageId)
        {
            try
            {
                var model = await TableNoTracking.Include(a => a.Product).Where(a => a.PackageId == packageId)
                        .SumAsync(a => a.Product.Price);

                return model.ToString("n0");
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<bool> IsExist(int packageId, int productId)
         => await TableNoTracking.AnyAsync(a => a.ProductId == productId && a.PackageId == packageId);


        public async Task<long> ResultPrice(int packageId)
        {
            var model = await TableNoTracking
                .Where(a => a.PackageId == packageId)
                .Include(a => a.Product)
                .ToListAsync();

            var sum = default(long);

            foreach (var item in model)
            {
                sum += await _productRepostitory.ResultPrice(item.ProductId);
            }

            return sum;
        }

        #endregion


        public async Task<SweetAlertExtenstion> ProductPackageAddItem(List<int> products
            , int packageId, int groupId)
        {
            await DeleteProductsbyGroupId();

            return await AddItem();

            #region LocalFunction

            async Task DeleteProductsbyGroupId()
            {
                var model = await GetListAsync(a => a.PackageId == packageId && a.ProductGroupId == groupId);

                if (model.Any())
                    await DeleteRangeAsync(model);
            }

            async Task<SweetAlertExtenstion> AddItem()
            {
                var lst = new List<ProductPackageDetails>();
                foreach (var item in products)
                {
                    lst.Add(new ProductPackageDetails()
                    {
                        PackageId = packageId,
                        ProductGroupId = groupId,
                        ProductId = item
                    });
                }

                await AddRangeAsync(lst, false);
                return Save();
            }

            #endregion
        }

        /// <summary>
        /// گرفتن اطلاعات پکیج های ثبت شده بر اساس گروه و شناسه پکیج
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<List<int>> GetAllByGroupIdAndPackageId(int packageId, int groupId)
        {
            var model = TableNoTracking.Where(a => a.PackageId == packageId && a.ProductGroupId == groupId);

            return await model.Select(a => a.ProductId).ToListAsync();
        }

        public async Task<List<DataLayer.Entities.ProductPackageDetails>> GetProductByPackageId(int packageId)
        {
            return await TableNoTracking.Where(x => x.PackageId == packageId).Include(x => x.Product).ToListAsync();
        }

        
        public async Task RemoveProductsbyPackageId(int packageId)
        {
            var model = await GetListAsync(a => a.PackageId == packageId);

            await DeleteRangeAsync(model);
        }
    }
}

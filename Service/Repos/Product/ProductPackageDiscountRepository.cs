using AutoMapper;
using Core.Utilities;
using DataLayer.Entities;
using DataLayer.ViewModels.ProductDiscount;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.Product
{
    public class ProductPackageDiscountRepository : GenericRepository<ProductPackageDiscount>
    {
        public ProductPackageDiscountRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }



        public async Task<bool> IsPackageSubmited(int id)
        {
            if (await IsPackageExpired(id)) return false;


            return await GetByConditionAsync(a => a.PackageId == id) != null;
        }


        public async Task<bool> IsPackageExpired(int id)
        {
            var model = await TableNoTracking.LastOrDefaultAsync(a => a.PackageId == id && a.IsActive == true);

            return model == null || DateTime.Now > model.EndDate;

        }



        public async Task<int?> UpdateDiscount(PackageDiscountUpdateViewModel vm)
        {
            var model = await GetByIdAsync(vm.Id);

            Mapper.Map(vm, model);

            await DbContext.SaveChangesAsync();

            return model.PackageId;
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

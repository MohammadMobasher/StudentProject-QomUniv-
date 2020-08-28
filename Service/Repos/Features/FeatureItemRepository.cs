using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.Feature;
using DataLayer.Entities;
using DataLayer.ViewModels.Feature;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos
{
    public class FeatureItemRepository : GenericRepository<FeatureItem>
    {
        public FeatureItemRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<SweetAlertExtenstion> InsertFeatureItem(FeatureItemsViewModel vm)
        {
            var lst = new List<FeatureItem>();

            foreach (var item in vm.Items)
            {
                lst.Add(new FeatureItem()
                {
                    FeatureId = vm.FeatureId,
                    Value = item
                });
            }

            await AddRangeAsync(lst);

            return SweetAlertExtenstion.Ok();
        }


        /// <summary>
        /// ویرایش آیتم‌های یک ویژگی 
        /// ابتدا مقادیر قبلی را حذف 
        /// سپس مقادیر جدید را ثبت میکنیم 
        /// </summary>
        /// <param name="featureId">شماره ویژگی </param>
        /// <param name="Items">ایتم‌های جدید</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> UpdateAsync(int featureId, List<string> Items)
        {
            try
            {
                var featureItems = await Entities.Where(x => x.FeatureId == featureId).ToListAsync();
                DbContext.RemoveRange(featureItems);
                // درصورتی که آیتمی را دوباره برای ثبت کاربر ارسال کرده باشد
                if (Items != null && Items.Count > 0)
                {
                    //==========================================
                    List<FeatureItem> newItems = new List<FeatureItem>();
                    //==========================================

                    foreach (var item in Items)
                    {
                        newItems.Add(new FeatureItem()
                        {
                            FeatureId = featureId,
                            Value = item
                        });
                    }

                    await AddRangeAsync(newItems);
                }

                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }


        public async Task<List<FeatureIdTitleDTO>> GetitemsByFeatureId(int featureId)
        {
            return await Entities.Where(a => a.FeatureId == featureId).Select(x=> 
            new FeatureIdTitleDTO
            {
                Id = x.Id,
                Title = x.Value
            }).ToListAsync();
        }


            public async Task<List<FeatureItem>> GetAllFeatureItemByFeatureId(int featureId)
            => await TableNoTracking.Where(a => a.FeatureId == featureId).ToListAsync();
        
    }
}

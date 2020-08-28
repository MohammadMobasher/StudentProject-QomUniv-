
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.ShopOrderStatus;
using DataLayer.Entities;
using DataLayer.SSOT;
using DataLayer.ViewModels.ShopOrderStatus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.Product
{
    public class ShopOrderStatusRepository : GenericRepository<ShopOrderStatus>
    {
        public ShopOrderStatusRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }


        /// <summary>
        /// ثبت اطلاعات در جدول مورد نظر 
        /// </summary>
        /// <param name="model">مدلی که باید به این تابع پاس داده شود  تا بتوان آن را ذخیره کرد</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> InsertAsync(ShopOrderStatusInsertViewModel model)
        {
            try
            {
                var entity = Mapper.Map<ShopOrderStatus>(model);

                await AddAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }


        /// <summary>
        /// گرفتن تمام وضعیت های مربوط به یک شماره فاکتور 
        /// </summary>
        /// <param name="shopOrderId"></param>
        /// <returns></returns>
        public async Task<List<ShopOrderStatusDTO>> GetItemsByOrderId(int shopOrderId)
        {
            return await TableNoTracking.ProjectTo<ShopOrderStatusDTO>().Where(x => x.ShopOrderId == shopOrderId).ToListAsync();
        }


        /// <summary>
        /// تغییر وضعیت یک فاکتور 
        /// در صورتی که فاکتور در در ایتدای کار باشد یعنی پرداخت شده باشد باید در وضعیت ثبت سفارش قرار بگیرد
        /// در غیر اینصورت باید به مرحله بعدی برود
        /// در صورتی که در مرحله تحویل باشد
        /// دیگر نمیتوان وضعیت آن را تغییر داد
        /// </summary>
        /// <param name="shopOrderId"></param>
        /// <returns></returns>
        public async Task<Tuple<SweetAlertExtenstion, ShopOrderStatusSSOT>> SendNextStatus(int shopOrderId)
        {
            var result = await TableNoTracking.Where(x => x.ShopOrderId == shopOrderId).OrderBy(x=> x.Date).ToListAsync();

            // حالتی که فاکتور تازه ثبت شده است یعنی به عبارت دیگر پرداخت توسط مشتری انجام شده است
            if(result == null || result.Count == 0)
                return new Tuple<SweetAlertExtenstion, ShopOrderStatusSSOT>(await this.InsertAsync(new ShopOrderStatusInsertViewModel
                {
                    ShopOrderId = shopOrderId,
                    Status = ShopOrderStatusSSOT.Ordered
                }), ShopOrderStatusSSOT.Ordered);

            var lastStatus = result.LastOrDefault().Status;

            if (lastStatus == ShopOrderStatusSSOT.Delivery)
                return new Tuple<SweetAlertExtenstion, ShopOrderStatusSSOT>(SweetAlertExtenstion.Error("فاکتور مورد نظر در مرحله تحویل است."), ShopOrderStatusSSOT.Nothing);

            return new Tuple<SweetAlertExtenstion, ShopOrderStatusSSOT>(await this.InsertAsync(new ShopOrderStatusInsertViewModel {

                ShopOrderId = shopOrderId,
                Status = lastStatus + 1

            }), lastStatus + 1);

        }


        
    }
}

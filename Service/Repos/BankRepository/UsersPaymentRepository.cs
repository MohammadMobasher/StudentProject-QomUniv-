using DataLayer.Entities.Bank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.BankRepository
{
    public class UsersPaymentRepository : GenericRepository<UsersPayment>
    {
        public UsersPaymentRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// مشخص شدن وضعیت درخواست کاربر
        /// </summary>
        /// <param name="shopOrderId"></param>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> ResultOrder(int shopOrderId,string orderId,int userId
            ,bool status,string resCode)
        {
            var model = await GetByConditionAsync(a => a.ShopOrderId == shopOrderId
            && a.UserId == userId && a.OrderId.Equals(orderId));

            if (model == null) return false;

            model.IsSuccessed = status;
            model.UpdateDate = DateTime.Now;
            model.ResCode = resCode;
            model.IsCallBackRecive = true;

            Update(model,false);
            return Save() ;
        }


        /// <summary>
        /// مشخص شدن وضعیت درخواست کاربر
        /// </summary>
        /// <param name="shopOrderId"></param>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> ResultOrderCallBack(int shopOrderId, string orderId, int userId)
        {
            var model = await GetByConditionAsync(a => a.ShopOrderId == shopOrderId
            && a.UserId == userId && a.OrderId.Equals(orderId));

            if (model == null) return false;

            model.IsSuccessed = false;
            model.IsCallBackRecive = true;
            model.UpdateDate = DateTime.Now;

            Update(model, false);
            return Save();
        }
    }
}

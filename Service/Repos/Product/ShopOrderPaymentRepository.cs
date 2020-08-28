using Core.Utilities;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.Product
{
    public class ShopOrderPaymentRepository : GenericRepository<ShopOrderPayment>
    {
        private readonly ShopOrderRepository _shopOrderRepository;

        public ShopOrderPaymentRepository(DatabaseContext dbContext
            , ShopOrderRepository shopOrderRepository) : base(dbContext)
        {
            _shopOrderRepository = shopOrderRepository;
        }


        public async Task<int> CreatePayment(int orderId)
        {
            var model = await _shopOrderRepository.GetByIdAsync(orderId);

            var count = Math.Ceiling((decimal)model.PaymentAmount / 50000000);

            var adds = new List<ShopOrderPayment>();

            if (count == 1)
            {
                adds.Add(new ShopOrderPayment()
                {
                    ShopOrderId = orderId,
                    IsSuccess = false,
                    PaymentAmount = model.PaymentAmount,
                    PaymentDate = null,
                });
            }
            else
            {
                var payment = model.PaymentAmount;
                for (int i = 0; i < count; i++)
                {
                    adds.Add(new ShopOrderPayment()
                    {
                        ShopOrderId = orderId,
                        IsSuccess = false,
                        PaymentAmount = payment > 50000000 ? 50000000 : payment,
                        PaymentDate = null,
                    });

                    payment = payment - 50000000;
                }
            }

            await AddRangeAsync(adds);

            return (int)count;
        }


        /// <summary>
        /// در این تابع مشخص می شود که آیا برای یک فاکتور تمام رکورد ها پرداخت شده است یا نه
        /// </summary>
        /// <param name="shopOrderId">شماره فاکتور</param>
        /// <returns></returns>
        public async Task<bool> AllIsPay(int shopOrderId)
        {
            var items = await DbContext.ShopOrderPayment.Where(x => x.ShopOrderId == shopOrderId && x.IsSuccess == /*false?*/ true).ToListAsync();
            if (items != null && items.Count > 0)
                return true;

            return false;

        }

        public async Task<bool> UpdateStatus(int? id)
        {
            var model = GetByCondition(a => a.Id == id);

            if (model == null) return false;

            model.IsSuccess = true;
            model.SuccessDate = DateTime.Now;

            await UpdateAsync(model);
            return true;
        }




    }
}

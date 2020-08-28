using Core.Utilities;
using Dapper;
using DataLayer.Entities;
using DataLayer.Entities.Warehouse;
using DataLayer.ViewModels;
using Microsoft.EntityFrameworkCore;
using Service.Repos.User;
using Service.Repos.Warehouses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos
{
    public class ShopOrderRepository : GenericRepository<ShopOrder>
    {
        private readonly UserAddressRepository _userAddressRepository;
        private readonly ShopProductRepository _shopProductRepository;
        private readonly IDbConnection _connection;
        private readonly WarehouseProductCheckRepository _warehouseProductCheckRepository;

        public ShopOrderRepository(DatabaseContext dbContext
            , UserAddressRepository userAddressRepository
            , ShopProductRepository shopProductRepository
            , IDbConnection connection
            , WarehouseProductCheckRepository warehouseProductCheckRepository) : base(dbContext)
        {
            _userAddressRepository = userAddressRepository;
            _shopProductRepository = shopProductRepository;
            _connection = connection;
            _warehouseProductCheckRepository = warehouseProductCheckRepository;
        }

        public async Task<int> CreateFactor(List<ShopProduct> list, int userId)
        {
            try
            {
                var tariff = CalculateTariff(userId) ?? 0;

                // در صورت داشتن مقدار فاکتوری ثبت نمی شود
                var _orderId = await CheckOrderFinaled(userId);

                if (_orderId == 0)
                {
                    var model = new ShopOrder()
                    {
                        Amount = await _shopProductRepository.CalculateCartPriceNumber(userId),
                        CreateDate = DateTime.Now,
                        IsSuccessed = false,
                        UserId = userId,
                        TransferProductPrice = tariff,

                    };

                    model.PaymentAmount = model.Amount + tariff;

                    await AddAsync(model);
                    // مشخص کردن اینکه این سبد محصولات مربوط به کدام فاکتور می باشد
                    await _shopProductRepository.ChangeStatus(list, model.Id);
                    return model.Id;
                }
                else
                {
                    var model = GetByCondition(a => a.Id == _orderId);

                    model.Amount = await _shopProductRepository.CalculateCartPriceNumber(userId);
                    model.TransferProductPrice = tariff;
                    model.PaymentAmount = model.Amount + tariff;

                    await UpdateAsync(model);
                }
                await _shopProductRepository.ChangeStatus(list, _orderId);
                return _orderId;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<ShopOrder>> ListInvoice(int userId, string Title)
        {
            if (!string.IsNullOrEmpty(Title))
            {
                return await Entities.Where(x => x.IsInvoice == true && x.UserId == userId && x.Title.Contains(Title)).ToListAsync();
            }
            else
            {
                return await Entities.Where(x => x.IsInvoice == true && x.UserId == userId).ToListAsync();
            }
            
        }

        public async Task<List<ShopOrder>> ListSpecialInvoice(string Title)
        {
            if (!string.IsNullOrEmpty(Title))
            {
                return await Entities.Where(x => x.IsSpecialInvoice == true && x.Title.Contains(Title)).ToListAsync();
            }
            else
            {
                return await Entities.Where(x => x.IsSpecialInvoice == true).ToListAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">چه کاربری</param>
        /// <param name="title">با چه عنوانی</param>
        /// <param name="IsInvoice">پیش فاکتور است یا خیر</param>
        /// <param name="specialInvoice">ادمین به عنوان پیش فاکتور پیش فرض ثبت کرده یا خیر </param>
        /// <returns></returns>
        public async Task<int> AddFactor(int userId, string title,bool IsInvoice,bool specialInvoice)
        {
            var model = new ShopOrder()
            {
                Amount = await _shopProductRepository.CalculateCartPriceNumber(userId),
                CreateDate = DateTime.Now,
                IsSuccessed = false,
                UserId = userId,
                Title = title,
                IsInvoice = IsInvoice,
                PaymentAmount = await _shopProductRepository.CalculateCartPriceNumber(userId),
                IsSpecialInvoice = specialInvoice
            };

            await AddAsync(model);

            var list = await DbContext.ShopProduct.Where(x=> x.UserId == userId && !x.IsFinaly && !x.IsFactorSubmited).ToListAsync();
            // مشخص کردن اینکه این سبد محصولات مربوط به کدام فاکتور می باشد
            await _shopProductRepository.ChangeStatus(list, model.Id);

            // در جدول مربوط به آدرس
            // شماره فاکتور را قرار میدهیم تا بعد بتوانیم از آن استفاده کنیم
            await _userAddressRepository.UpdateShopOrderId(model.Id, userId);

            return model.Id;
        }


        public async Task<int> UpdatePaymentFactor(int factorId,IEnumerable<ShopProduct> shopProducts)
        {
            try
            {
                var model = await GetByIdAsync(factorId);

                var tariff = CalculateTariffByOrderId(factorId) ?? 0;
             
                model.Amount = await _shopProductRepository.CalculateCartPriceNumber(model.UserId,factorId);
                model.PaymentAmount = model.Amount + tariff;
                

                await UpdateAsync(model);
                // در جدول مربوط به آدرس
                // شماره فاکتور را قرار میدهیم تا بعد بتوانیم از آن استفاده کنیم
                await _userAddressRepository.UpdateShopOrderId(model.Id, model.UserId);

                await _shopProductRepository.ChangeStatus(shopProducts.ToList(),model.Id);

                return model.Id;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public async Task<int> CreatePaymentFactor(List<ShopProduct> list, int userId)
        {
            try
            {
                var tariff = CalculateTariff(userId) ?? 0;
                var model = new ShopOrder()
                {
                    Amount = await _shopProductRepository.CalculateCartPriceNumber(userId),
                    CreateDate = DateTime.Now,
                    IsSuccessed = false,
                    UserId = userId,
                    TransferProductPrice = tariff,

                };

                

                model.PaymentAmount = model.Amount + tariff;

                await AddAsync(model);
                // در جدول مربوط به آدرس
                // شماره فاکتور را قرار میدهیم تا بعد بتوانیم از آن استفاده کنیم
                await _userAddressRepository.UpdateShopOrderId(model.Id, userId);

                // مشخص کردن اینکه این سبد محصولات مربوط به کدام فاکتور می باشد
                await _shopProductRepository.ChangeStatus(list, model.Id);



                return model.Id;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }




        /// <summary>
        /// این برای زمانی است که یک فاکتور زده شده ولی به تایید نایی نرسیده است 
        /// و دوباره میخواهد به سمت درگاه برود برای جلوگیری  از ازدیاد فاکتور ها
        /// </summary>
        /// <returns></returns>
        public async Task<int> CheckOrderFinaled(int userId)
        {
            var model = await GetListAsync(a => a.UserId == userId && !a.IsSuccessed);

            return model.Count() > 0 ? model.LastOrDefault().Id : 0;
        }

        /// <summary>
        /// حذف یک پیش فاکتور با مخلفاتش
        /// </summary>
        /// <param name="id">شماره پیش فاکتور</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> DeleteInvoice(int id)
        {
            try
            {
                var entity = await Entities.SingleOrDefaultAsync(x => x.Id == id);

                await _shopProductRepository.DeleteByShopOrderId(id);

                DbContext.Remove(entity);
                await DbContext.SaveChangesAsync();
                return SweetAlertExtenstion.Ok();
            }
            catch (Exception e)
            {
                return SweetAlertExtenstion.Error();
            }

        }

        /// <summary>
        /// زمانی که خرید با موفقیت انجام شد ما در دیتا بیس ثبت میکنیم
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> SuccessedOrder(int id, int userId)
        {
            var model = GetByCondition(a => a.Id == id && a.UserId == userId);

            if (model == null) return false;

            model.IsSuccessed = true;
            model.SuccessDate = DateTime.Now;

            await UpdateAsync(model);
            return true;
        }


        public async Task<Tuple<int, List<ShopOrder>>> OrderLoadAsync(ShopOrdersSearchViewModel vm, int page = 0, int pageSize = 10)
        {
            var model = TableNoTracking.Where(a => !a.IsDeleted);

            model = model.WhereIf(vm.Id != null, a => a.Id == vm.Id);
            model = model.WhereIf(vm.Amount != null, a => a.Amount == vm.Amount);
            model = model.WhereIf(vm.Status != null, a => a.Status == vm.Status);
            model = model.WhereIf(vm.IsSuccessed != null, a => a.IsSuccessed == vm.IsSuccessed);

            var count = model.Count();

            model = model.Include(a => a.Users);
            model = model.OrderByDescending(a => a.SuccessDate).ThenByDescending(a => a.Id);

            return new Tuple<int, List<ShopOrder>>(count, await model.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());
        }


        public async Task<ShopOrder> GetItemByIdWithUserAsync(int Id)
        {
            return await TableNoTracking.Include(x => x.Users).Where(x => x.Id == Id).SingleOrDefaultAsync();
        }

        public long? CalculateTariff(int userId)
        {
            var sqlQuery = $@"
                DECLARE @UserInfo TABLE (productId int,UserArea int, Area int)
                INSERT INTO @UserInfo(productId,UserArea, Area)
                select distinct ShopProduct.ProductId , UserAddress.TehranAreasFrom , Warehouse.Region
                from AspNetUsers
                	JOIN ShopProduct ON AspNetUsers.Id = ShopProduct.UserId
                	JOIN UserAddress ON AspNetUsers.Id = UserAddress.UserId
                	JOIN WarehouseProductCheck on ShopProduct.ProductId = WarehouseProductCheck.ProductId
                	JOIN Warehouse on WarehouseProductCheck.WarehouseId = Warehouse.Id
                where IsFinaly = 0 and IsFactorSubmited= 0 and AspNetUsers.Id = {userId}
                
                DECLARE @OrderDetail TABLE (ProductSize BIGINT,Area int)
                insert INTO @OrderDetail(ProductSize,Area)
                select SUM(ProductSize),WareHouse.Area 
                from @UserInfo as WareHouse
                	JOIN Product on WareHouse.ProductId = Product.Id
                Group By Area
                
                SELECT SUM(Maxtariff) AS Tariff
                FROM (
                	select MAX(tariff) as Maxtariff 
                	from TransportationTariff
                		JOIN @OrderDetail as Orders on TransportationTariff.TehranAreasFrom = Orders.Area
                	Where TehranAreasTO = (select Top 1 UserArea from @UserInfo)
                		AND Orders.ProductSize between  TransportationTariff.ProductSizeFrom
                		AND TransportationTariff.ProductSizeTo
                	group by tehranareasFrom,TehranAreasTO,ProductSize)t";

            var tariff = _connection.Query<long?>(sqlQuery).FirstOrDefault();

            return tariff;
        }

        public long? CalculateTariffByOrderId(int orderId)
        {
            var sqlQuery = $@"
                DECLARE @UserInfo TABLE (productId int,UserArea int, Area int)
                INSERT INTO @UserInfo(productId,UserArea, Area)
                select distinct ShopProduct.ProductId , UserAddress.TehranAreasFrom , Warehouse.Region
                from ShopProduct 
                	JOIN WarehouseProductCheck on ShopProduct.ProductId = WarehouseProductCheck.ProductId
                	JOIN Warehouse on WarehouseProductCheck.WarehouseId = Warehouse.Id
					JOIN UserAddress ON ShopProduct.ShopOrderId = UserAddress.ShopOrderId
                where ShopProduct.ShopOrderId = {orderId}
                
                DECLARE @OrderDetail TABLE (ProductSize BIGINT,Area int)
                insert INTO @OrderDetail(ProductSize,Area)
                select SUM(ProductSize),WareHouse.Area 
                from @UserInfo as WareHouse
                	JOIN Product on WareHouse.ProductId = Product.Id
                Group By Area
                
                SELECT SUM(Maxtariff) AS Tariff
                FROM (
                	select MAX(tariff) as Maxtariff 
                	from TransportationTariff
                		JOIN @OrderDetail as Orders on TransportationTariff.TehranAreasFrom = Orders.Area
                	Where TehranAreasTO = (select Top 1 UserArea from @UserInfo)
                		AND Orders.ProductSize between  TransportationTariff.ProductSizeFrom
                		AND TransportationTariff.ProductSizeTo
                	group by tehranareasFrom,TehranAreasTO,ProductSize)t";

            var tariff = _connection.Query<long?>(sqlQuery).FirstOrDefault();

            return tariff;
        }


        public async Task<SweetAlertExtenstion> DeleteOrder(int id)
        {
            var model = GetByCondition(a => a.Id == id);

            if (model == null) return SweetAlertExtenstion.Error();

            model.IsDeleted = true;
            Update(model);

            var productIds = DbContext.ShopProduct.Where(x => x.ShopOrderId == id).ToList();
            List<WarehouseProductCheck> items = new List<WarehouseProductCheck>();

            foreach (var item in productIds)
            {
                items.Add(new WarehouseProductCheck
                {
                    Count = item.Count,
                    Date = DateTime.Now,
                    ProductId = item.ProductId.Value,
                    TypeSSOt = DataLayer.SSOT.WarehouseTypeSSOT.In,
                });
            }

            await _warehouseProductCheckRepository.AddFromShopOrder(items);

            return SweetAlertExtenstion.Ok();
        }


        /// <summary>
        /// زمانی که کاربر به سمت درگاه میرود 
        /// باید زمان درخواست این کالا به روز شود
        /// </summary>
        /// <param name="shopOrderId"></param>
        /// <returns></returns>
        public async Task UpdateCreateDate(int shopOrderId)
        {
            var result = await Entities.SingleOrDefaultAsync(x => x.Id == shopOrderId);
            result.CreateDate = DateTime.Now;
            await UpdateAsync(result);
        }


        /// <summary>
        /// ایجاد فاکتور از پیش فاکتور
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int?> OverWriteShopOrder(int id,int userId)
        {
            try
            {
                var model = await GetByIdAsync(id);

                if (model == null) return null;

                var entity = new ShopOrder()
                {
                    Amount = model.Amount,
                    CreateDate = DateTime.Now,
                    IsDeleted = false,
                    IsSuccessed = false,
                    DiscountCode = model.DiscountCode,
                    IsInvoice = false,
                    TransferProductPrice = model.TransferProductPrice,
                    SuccessDate = null,
                    Status = null,
                    OrderId = null,
                    Title = model.Title,
                    PaymentAmount = model.PaymentAmount,
                    UserId = model.IsSpecialInvoice ? userId : model.UserId
                };

                await AddAsync(entity);

                await _shopProductRepository.OverwriteShopProduct(id, entity.Id,userId);

                return entity.Id;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// محاسبه تعرفه و قیمت نهایی برای فاکتور
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> SetTariffForFactor(int orderId)
        {
            var model = await GetByIdAsync(orderId);

            if (model == null) return SweetAlertExtenstion.Error();

            var tariff = CalculateTariffByOrderId(orderId) ?? 0;

            model.TransferProductPrice = tariff;
            model.PaymentAmount = (model.TransferProductPrice ?? 0) + model.Amount;

            await UpdateAsync(model,false);

            return await SaveAsync();
        }


        /// <summary>
        /// محاسبه دوباره قیمت قبل از به ثبت نهایی رسیدن
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> ReloadPrice(int orderId)
        {
            var model = GetById(orderId);

            if (model == null) return SweetAlertExtenstion.Error("اطلاعاتی با این شناسه یافت نشد");

            model.Amount =await  _shopProductRepository.CalculateCartPriceNumber(model.UserId, orderId);
            model.TransferProductPrice = CalculateTariffByOrderId(orderId);
            model.PaymentAmount = model.Amount + (model.TransferProductPrice ?? 0);

            await UpdateAsync(model, false);

            return Save();
        }
    }
}

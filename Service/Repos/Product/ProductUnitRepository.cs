using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO;
using DataLayer.DTO.ProductUnit;
using DataLayer.Entities;
using DataLayer.ViewModels.ProductGroup;
using DataLayer.ViewModels.ProductUnit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.Product
{
    public class ProductUnitRepository : GenericRepository<ProductUnit>
    {
        private readonly ProductRepostitory _productRepostitory;

        public ProductUnitRepository(DatabaseContext dbContext,
            ProductRepostitory productRepostitory) : base(dbContext)
        {
            _productRepostitory = productRepostitory;
        }

        public async Task<Tuple<int, List<ProductUnitDTO>>> LoadAsyncCount(
           int skip = -1,
           int take = -1,
           ProductUnitSearchViewModel model = null)
        {
            var query = Entities.ProjectTo<ProductUnitDTO>();
            
            if (!string.IsNullOrEmpty(model.Title))
                query = query.Where(x => x.Title.Contains(model.Title));

            if (!string.IsNullOrEmpty(model.Name))
                query = query.Where(x => x.Title.Contains(model.Name));

            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);

            return new Tuple<int, List<ProductUnitDTO>>(Count, await query.ToListAsync());
        }



        /// <summary>
        /// ثبت یک آیتم در جدول مورد نظر
        /// </summary>
        /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> AddAsync(ProductUnitInsertViewModel model)
        {

            try
            {
                var entity = Mapper.Map<ProductUnit>(model);
                await AddAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }

        }


        /// <summary>
        /// ثبت یک آیتم در جدول مورد نظر
        /// </summary>
        /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> UpdateAsync(ProductUnitUpdateViewModel model)
        {

            try
            {
                var entity = Mapper.Map<ProductUnit>(model);
                await UpdateAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch (Exception e)
            {
                return SweetAlertExtenstion.Error();
            }

        }


        /// <summary>
        /// ثبت یک آیتم در جدول مورد نظر
        /// </summary>
        /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> DeleteAsync(int Id)
        {

            try
            {
                var entity = new ProductUnit { Id = Id };
                if(await _productRepostitory.DeleteByProductUnitId(Id))
                    await DeleteAsync(entity);
                else
                    return SweetAlertExtenstion.Error();
                return SweetAlertExtenstion.Ok("عملیات با موفقیت انجام شد");
            }
            catch(Exception e)
            {

                return SweetAlertExtenstion.Error();
            }

        }
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.FAQs;
using DataLayer.Entities.FAQs;
using DataLayer.ViewModels.FAQ;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.FAQs
{
    public class FAQRepository : GenericRepository<FAQ>
    {
        public FAQRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<Tuple<int, List<FAQDTO>>> LoadAsyncCount(
           int skip = -1,
           int take = -1,
           FAQSearchViewModel model = null)
        {
            var query = Entities.ProjectTo<FAQDTO>();

            if (!string.IsNullOrEmpty(model.QuestionText))
                query = query.Where(x => x.QuestionText.Contains(model.QuestionText));


            if (!string.IsNullOrEmpty(model.AnswerText))
                query = query.Where(x => x.AnswerText.Contains(model.AnswerText));


            if (model.IsActive != null)
                query = query.Where(x => x.IsActive == model.IsActive);


            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);

            return new Tuple<int, List<FAQDTO>>(Count, await query.ToListAsync());
        }

        /// <summary>
        /// گرفتن آیتم‌های مربوط به یک گروه
        /// </summary>
        /// <param name="id">شماره گروه</param>
        /// <returns></returns>
        public async Task<List<FAQDTO>> GetItemsByGroupId(int id)
        {
            return await Entities.ProjectTo<FAQDTO>().Where(x => x.FaqGroupId == id).OrderByDescending(x => x.Id).ToListAsync();
        }



        /// <summary>
        /// ثبت یک آیتم در جدول مورد نظر
        /// </summary>
        /// <param name="model">مدلی که از سمت کلاینت در حال پاس دادن آن هستیم</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> AddAsync(FAQInsertViewModel model)
        {

            try
            {
                var entity = Mapper.Map<FAQ>(model);
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
        public async Task<SweetAlertExtenstion> UpdateAsync(FAQUpdateViewModel model)
        {

            try
            {
                var entity = Mapper.Map<FAQ>(model);
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
                var entity = await GetByIdAsync(Id);
                await DeleteAsync(entity);
                return SweetAlertExtenstion.Ok("عملیات با موفقیت انجام شد");
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }

        }

        /// <summary>
        /// جستجو در بین سوالات پر تکرار
        /// از این تابع برای صفحه اصلی موجود در سوالات پر تکرار استفاده می شود
        /// </summary>
        /// <param name="text">متنی که باید مورد جسجتو قرار بگیرد</param>
        /// <returns></returns>
        public async Task<List<FAQDTO>> Search(string text)
        {
            return await Entities.ProjectTo<FAQDTO>().Where(x => x.QuestionText.Contains(text) || x.AnswerText.Contains(text)).ToListAsync();
        }

    }
}

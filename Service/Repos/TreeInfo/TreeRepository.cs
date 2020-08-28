using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.TreeInfo;
using DataLayer.Entities;
using DataLayer.Entities.TreeInfo;
using DataLayer.ViewModels.TreeInfo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.TreeInfo
{
    public class TreeRepository : GenericRepository<Tree>
    {
        private readonly SiteSettingRepository _siteSettingRepository;
        private readonly UserTreeRemindedRepository _userTreeRemindedRepository;

        public TreeRepository(
            DatabaseContext dbContext,
            SiteSettingRepository siteSettingRepository,
            UserTreeRemindedRepository userTreeRemindedRepository) : base(dbContext)
        {
            _siteSettingRepository = siteSettingRepository;
            _userTreeRemindedRepository = userTreeRemindedRepository;
        }


        /// <summary>
        /// محاسبه مقدار امتیاز تعلق گرفته به هر خرید
        /// </summary>
        /// <param name="shopOrder">فاکتور خرید مورد نظر</param>
        /// <returns></returns>
        public async Task<double> CalculateRateTreeFromAmount(long amount)
        {
            double rate = 0.0;

            var siteSetting = await _siteSettingRepository.GetInfo();

            rate = amount / siteSetting.TreeRate.Value;

            return rate;
        }


        public async Task CalculateRateTreeFromAmountAndInsert(long amount, int userId)
        {
            var rate = await this.CalculateRateTreeFromAmount(amount);

            if(rate >= 1)
            {
                for (int i = 0; i < (rate / 1); i++)
                    await this.InsertAsync(new TreeInsertViewModel { UserId = userId, IsPlanted = false});
                await _userTreeRemindedRepository.Insert(userId, rate % 1);
            }
            else
            {
                await _userTreeRemindedRepository.Insert(userId, rate);
            }
        }


        /// <summary>
        /// ثبت اطلاعات در جدول مورد نظر 
        /// </summary>
        /// <param name="model">مدلی که باید به این تابع پاس داده شود  تا بتوان آن را ذخیره کرد</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> InsertAsync(TreeInsertViewModel model)
        {
            try
            {
                var entity = Mapper.Map<Tree>(model);
                
                await AddAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }


        public async Task<SweetAlertExtenstion> UpdateAsync(TreeUpdateViewModel model)
        {
            try
            {
                var entity = await GetByIdAsync(model.Id);

                entity = (Tree)Mapper.Map(model, entity, typeof(TreeUpdateViewModel), typeof(Tree));
                await DbContext.SaveChangesAsync();
                return SweetAlertExtenstion.Ok();
            }
            catch (Exception e)
            {
                return SweetAlertExtenstion.Error();
            }
        }



        public async Task<Tuple<int, List<TreeDTO>>> LoadAsyncCount(
           int skip = -1,
           int take = -1,
           TreeSearchViewModel model = null)
        {
            var query = Entities.Include(x=> x.User).ProjectTo<TreeDTO>();

            if (!string.IsNullOrEmpty(model.NameFamily))
                query = query.Where(x => x.User.FirstName.Contains(model.NameFamily) || x.User.LastName.Contains(model.NameFamily));

            if (model.TreeNumber != null)
                query = query.Where(x => x.TreeNumber == model.TreeNumber);

            if (model.IsPlanted != null)
                query = query.Where(x => x.IsPlanted == model.IsPlanted);


            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);

            return new Tuple<int, List<TreeDTO>>(Count, await query.ToListAsync());
        }
    }
}

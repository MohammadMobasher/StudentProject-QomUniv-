using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.Condition;
using DataLayer.DTO.SuggestionsAndComplaint;
using DataLayer.Entities;
using DataLayer.ViewModels.Condition;
using DataLayer.ViewModels.SuggestionsAndComplaint;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos
{
    public class SuggestionsAndComplaintRepository : GenericRepository<SuggestionsAndComplaint>
    {
        public SuggestionsAndComplaintRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }


        public async Task<Tuple<int, List<SuggestionsAndComplaintDTO>>> LoadAsyncCount(
           int skip = -1,
           int take = -1,
           SuggestionsAndComplaintSearchViewModel model = null)
        {
            var query = Entities.ProjectTo<SuggestionsAndComplaintDTO>();

            if (!string.IsNullOrEmpty(model.Name))
                query = query.Where(x => x.Name.Contains(model.Name));

            if (!string.IsNullOrEmpty(model.Family))
                query = query.Where(x => x.Name.Contains(model.Family));


            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);

            return new Tuple<int, List<SuggestionsAndComplaintDTO>>(Count, await query.ToListAsync());
        }


        /// <summary>
        /// ثبت یک آیتم در این جدول
        /// </summary>
        /// <param name="model">مدلی که از صفحه به این تابع ارسال می شود</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> InsertAsync(SuggestionsAndComplaintInsertViewModel model)
        {
            try
            {
                var entity = Mapper.Map<SuggestionsAndComplaint>(model);

                await AddAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }


    }
}

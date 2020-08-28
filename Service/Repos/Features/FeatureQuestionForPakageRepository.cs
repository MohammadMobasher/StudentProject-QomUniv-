using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Utilities;
using Dapper;
using DataLayer.DTO.FeatureQuestionForPakage;
using DataLayer.Entities.Features;
using DataLayer.ViewModels.Feature;
using DataLayer.ViewModels.FeatureQuestionForPakage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.Features
{
    public class FeatureQuestionForPakageRepository : GenericRepository<FeatureQuestionForPakage>
    {
        private readonly IDbConnection connection;
        public FeatureQuestionForPakageRepository(DatabaseContext dbContext,IDbConnection connection) : base(dbContext)
        {
            this.connection = connection;
        }


        public async Task<Tuple<int, List<FeatureQuestionForPakageDTO>>> LoadAsyncCount(
            int skip = -1,
            int take = -1,
            FeatureQuestionForPakageSearchViewModel model = null)
        {
            var query = Entities.ProjectTo<FeatureQuestionForPakageDTO>();

            if (!string.IsNullOrEmpty(model.QuestionTitle))
                query = query.Where(x => x.QuestionTitle.Contains(model.QuestionTitle));


            if (model.FeatureId != 0)
                query = query.Where(x => x.FeatureId == model.FeatureId);

            if (model.GroupId != 0)
                query = query.Where(x => x.GroupId == model.GroupId);


            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);



            return new Tuple<int, List<FeatureQuestionForPakageDTO>>(Count, await query.ToListAsync());
        }


        /// <summary>
        /// ثبت یک رکورد در این جدول
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> Insert(FeatureQuestionForPakageInsertViewModel model)
        {
            try
            {
                var entity = Mapper.Map<FeatureQuestionForPakage>(model);
                await AddAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch(Exception e)
            {
                return SweetAlertExtenstion.Error();
            }
        }


        public async Task<SweetAlertExtenstion> UpdateAsync(FeatureQuestionForPakageUpdateViewModel model)
        {
            try
            {
                
                var entity = Mapper.Map<FeatureQuestionForPakage>(model);
                await UpdateAsync(entity);


                await DbContext.SaveChangesAsync();
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
        /// <param name="Id">شماره خبر</param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> DeleteAsync(int Id)
        {

            try
            {
                var entity = await GetByIdAsync(Id);

                await DeleteAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }

        }

        #region واکشی سوالات

        public List<FeatureQuestionListViewModel> ListQuestions()
        {
            var questionQuery = $@"
                SELECT FeatureQuestionForPakage.QuestionTitle,Feature.Id,FeatureQuestionForPakage.Id as QuestionId from FeatureQuestionForPakage join
                Feature on FeatureQuestionForPakage.FeatureId = Feature.Id
                ";

            var questionModels = connection.Query<FeatureQuestionViewModel>(questionQuery).ToList();

            var questionItem = $@"
            select * Into #tmp from
            [dbo].[FN_GetTableOfs]('{FeatureIds()}')
            
            Select  * from FeatureItem
            where FeatureId in (select * from #tmp)
            ";

            var items = connection.Query<FeatureQuestionListItemViewModel>(questionItem);

            return GetItems().ToList();

            #region LocalFunctions

            string FeatureIds()
            {
                var featureIds = questionModels.Select(a => a.Id).ToList();

                return string.Join(",", featureIds).TrimEnd(',');
            }
             
            IEnumerable<FeatureQuestionListViewModel> GetItems()
            {
                foreach (var item in questionModels)
                {
                    yield return new FeatureQuestionListViewModel()
                    {
                        Question = item,
                        Items = items.Where(a => a.FeatureId == item.Id).ToList()
                    };
                }

            }

            #endregion

        }

        public List<FeatureQuestionListViewModel> ListQuestions(List<int> group)
        {
            var questionQuery = $@"
                
            select * Into #tmp from
            [dbo].[FN_GetTableOfs]('{GroupIds()}')

            SELECT FeatureQuestionForPakage.QuestionTitle,Feature.Id,FeatureQuestionForPakage.Id as QuestionId ,
	        FeatureQuestionForPakage.GroupId as GroupId
	        from FeatureQuestionForPakage join
            Feature on FeatureQuestionForPakage.FeatureId = Feature.Id and FeatureQuestionForPakage.GroupId in (select * from #tmp)
                ";

            var questionModels = connection.Query<FeatureQuestionViewModel>(questionQuery).ToList();

            var questionItem = $@"
            select * Into #tmp from
            [dbo].[FN_GetTableOfs]('{FeatureIds()}')
            
            Select  * from FeatureItem
            where FeatureId in (select * from #tmp)
            ";

            var items = connection.Query<FeatureQuestionListItemViewModel>(questionItem);

            return GetItems().ToList();

            #region LocalFunctions

            string FeatureIds()
            {
                var featureIds = questionModels.Select(a => a.Id).ToList();

                return string.Join(",", featureIds).TrimEnd(',');
            }

            string GroupIds()
            {
                return string.Join(",", group).TrimEnd(',');
            }

            IEnumerable<FeatureQuestionListViewModel> GetItems()
            {
                foreach (var item in questionModels)
                {
                    yield return new FeatureQuestionListViewModel()
                    {
                        Question = item,
                        Items = items.Where(a => a.FeatureId == item.Id).ToList()
                    };
                }

            }

            #endregion

        }

        #endregion
    }

}

using Core.Utilities;
using Dapper;
using DataLayer.Entities;
using DataLayer.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.Product
{
    public class PackageUserAnswerRepository : GenericRepository<PackageUserAnswers>
    {
        private readonly IDbConnection _connection;

        public PackageUserAnswerRepository(DatabaseContext dbContext, IDbConnection connection) : base(dbContext)
        {
            _connection = connection;
        }


        public async Task<SweetAlertExtenstion> AddAnswer(PackageFeatureInsertViewModel vm
            , int userId, int packageId)
        {
            var list = new List<PackageUserAnswers>();

            foreach (var item in vm.Items)
            {
                list.Add(new PackageUserAnswers()
                {
                    Answer = item.FeatureValue.ToString(),
                    QuestionId = item.QuestionId,
                    IsManager = true,
                    PackageId = packageId,
                    UserId = userId,
                    FeatureId = item.FeatureId
                });
            }

            await AddRangeAsync(list, false);

            return await SaveAsync();
        }


        public async Task<SweetAlertExtenstion> UpdateAnswer(PackageFeatureInsertViewModel vm
            , int userId, int packageId)
        {
            var model =await GetListAsync(a => a.PackageId == packageId);

            await DeleteRangeAsync(model.ToList());

            return await AddAnswer(vm, userId, packageId);
        }


     
    }
}

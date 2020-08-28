using Dapper;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.Product
{
    public class ProductPackageGroupRepository : GenericRepository<ProductPackageGroups>
    {
        private readonly IDbConnection _connection;
        public ProductPackageGroupRepository(DatabaseContext dbContext, IDbConnection connection) : base(dbContext)
        {
            _connection = connection;
        }


        public async Task AddGroupRange(List<int> groups, int packageId)
        {
            DeleteRange(await GetListAsync(a => a.PackageId == packageId));

            var list = new List<ProductPackageGroups>();

            foreach (var item in groups)
            {
                list.Add(new ProductPackageGroups()
                {
                    GroupId = item,
                    PackageId = packageId
                });
            }

            await AddRangeAsync(list);
        }


        public async Task<List<ProductPackageGroups>> getItemsByPackageId(int packageId)
        {
            return await Entities.Include(x => x.ProductGroup).Where(x => x.PackageId == packageId).ToListAsync();
        }

        public bool IsGroupChanged(int packageId, List<int> groupsId)
        {
            var sql = $@"
                select GroupId from ProductPackageGroups
                where PackageId = {packageId}
                ";
            var execute = _connection.Query<int>(sql);

            var model = execute.ToList();

            var result = groupsId.Except(model).Count();

            var decrease = model.Except(groupsId).Count();

            return result != 0 || decrease != 0;
        }
    }
}

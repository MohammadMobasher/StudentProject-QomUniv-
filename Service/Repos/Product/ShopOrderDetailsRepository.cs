using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Repos.Product
{
    public class ShopOrderDetailsRepository : GenericRepository<ShopOrderDetails>
    {
        public ShopOrderDetailsRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}

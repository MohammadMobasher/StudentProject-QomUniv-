using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Repos
{
    public class ConvertRepository : GenericRepository<ConvertAddress>
    {
        public ConvertRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }


    }
}

using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Repos
{
    public class LogRepository : GenericRepository<Log>
    {
        public LogRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}

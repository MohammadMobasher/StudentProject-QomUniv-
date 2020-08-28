using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Repos.User
{
    public class FavoritesRepository : GenericRepository<Favorites>
    {
        public FavoritesRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }
    }
}

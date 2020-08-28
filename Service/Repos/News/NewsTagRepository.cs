using DataLayer.DTO;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using AutoMapper.QueryableExtensions;

namespace Service.Repos
{
    public class NewsTagRepository : GenericRepository<NewsTag>
    {
        public NewsTagRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }


       

    }
}

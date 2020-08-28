using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.CustomMapping
{
    public class CustomMappingProfile : Profile
    {
        public CustomMappingProfile(IEnumerable<IMapping> haveCustomMappings)
        {
            foreach (var item in haveCustomMappings)
                item.CreateMappings(this);
        }
    }
}

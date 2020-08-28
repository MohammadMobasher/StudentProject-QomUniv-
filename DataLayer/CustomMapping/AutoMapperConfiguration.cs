using AutoMapper;
using DataLayer.DTO.FAQs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataLayer.CustomMapping
{
    public static class AutoMapperConfiguration
    {
        public static void AddCustomMapping()
        {
            Mapper.Initialize(config =>
            {
                config.AddCustomMappingProfile();
            });

            Mapper.Configuration.CompileMappings();
        }


        public static void AddCustomMappingProfile(this IMapperConfigurationExpression config)
        {
          
            config.AddCustimMappingProfile(Assembly.GetAssembly(typeof(FAQDTO)));
        }

        public static void AddCustimMappingProfile(this IMapperConfigurationExpression config, params Assembly[] asm)
        {
            var allList = asm.SelectMany(a => a.GetExportedTypes());

            var list = allList.Where(type => type.IsClass && !type.IsAbstract &&
                        type.GetInterfaces().Contains(typeof(IMapping)))
                        .Select(type => (IMapping)Activator.CreateInstance(type));

            var profile = new CustomMappingProfile(list);

            config.AddProfile(profile);
        }

    }
}

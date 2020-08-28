//using AutoMapper;
//using DataLayer.CustomMapping;
//using IHostedServiceSample;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Service.Mappers;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Text;

//namespace Service
//{
//    public static class Startup
//    {
//        public static void DatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
//        {
//            services.AddDbContext<DatabaseContext>(options
//                => options.UseSqlServer(configuration.GetConnectionString("MyConnection")));

//            //services.AddScope<IDbConnection>(
//            //    _ => new SqlConnection(configuration.GetConnectionString("MyConnection")));

//            services.AddSingleton<IDbConnection>(
//       _ => new SqlConnection(configuration.GetConnectionString("MyConnection")));

//            //services.AddSingleton<IHostedService, DeleteOrderService>();


//            services.Scan(scan =>
//                scan.FromAssemblyOf<DatabaseContext>()
//                    .AddClasses(classes => classes.InNamespaceOf<DatabaseContext>() )
//                    .AsSelf()
//                    .WithScopedLifetime());

//            #region Mapper
//            services.AddAutoMapper(typeof(SlideShowMapper));
//            // TODO
//            //AutoMapperConfiguration.AddCustomMapping();
//            #endregion
//        }

//    }
//}

//using DNTScheduler.Core;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Service.Scheduler;

//namespace WebFramework.Configurations
//{
//    public static class ScheduleConfiguration
//    {
//        public static void AddSchedulers(this IServiceCollection services)
//        {
//            services.AddDNTScheduler(options =>
//            {

//                options.AddPingTask = true;
//                options.AddScheduledTask<OrdersScheduler>(
//                    runAt: utcNow =>
//                    {
//                        var now = utcNow.AddHours(4.5);

//                        return now.Hour == 23 && now.Minute == 59 && now.Second == 59;
//                    },
//                    order: 1);
//            });

//        }

//    }
//}


//using DNTScheduler.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;


namespace WebFramework.Configurations
{
    public static class ScheduleConfiguration
    {
        public static void AddSchedulers(this IServiceCollection services)
        {
            //services.AddDNTScheduler(options =>
            //{

            //    options.AddPingTask = true;
            //    options.AddScheduledTask<OrdersScheduler>(
            //        runAt: utcNow =>
            //        {
            //            var now = utcNow.AddMinutes(2);

            //            return now.Second % 10 == 0;
            //        },
            //        order: 1);
            //});

        }

    }
}

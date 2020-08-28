using Dapper;
using Microsoft.AspNetCore.Http;
using Service.Repos;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace IHostedServiceSample
{
    public class DeleteOrderService : HostedService
    {
        private readonly IDbConnection _connection;
        //private readonly LogRepository _logRepository;

        public DeleteOrderService(
            IDbConnection connection
            //LogRepository logRepository
            )
        {
            _connection = connection;
            //_logRepository = logRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    string query = $@"


                                select Id into #tmp from ShopOrder 
	                                where GetDate() > DATEADD(mi,30,ShopOrder.CreateDate) and ShopOrder.IsSuccessed = 0 
	                                and Id in (SELECT   ShopOrderId
                                     FROM     ShopOrderPayment As b
                                     GROUP BY b.ShopOrderId
                                     HAVING   Count(*) = (
		                                SELECT   Count(*)
			                                FROM     ShopOrderPayment As d
			                                where d.IsSuccess = 0 and b.ShopOrderId = d.ShopOrderId
			                                GROUP BY d.ShopOrderId
	                                 ))

                               


                                    update ShopProduct
                                    set ShopOrderId = NULL, IsFactorSubmited = 0, RequestedDate = getdate()
                                    where ShopOrderId in (select * from #tmp)

                                    delete ShopOrderPayment where ShopOrderId in (select * from #tmp)
                                    delete UserAddress where ShopOrderId in (select * from #tmp)
                                    
                                    delete ShopOrder 
                                    where Id in (select * from #tmp)

                                    If(OBJECT_ID('tempdb..#tmp') Is Not Null)
                                    Begin
                                        Drop Table #Tmp
                                    End
                ";

                    string query2 = $@"

 
  select * into #tmp from ShopProduct
   where GetDate() > DATEADD(mi,30,RequestedDate) and IsFinaly = 0 and ShopOrderId is NULL


   INSERT INTO WarehouseProductCheck(
	 Count, 
	 Date, 
	 ProductId,
	 TypeSSOt,
	 WarehouseId
    ) 
   select #tmp.Count, getDate(), #tmp.ProductId, 1, (select top 1 WarehouseId from WarehouseProductCheck where ProductId = #tmp.ProductId)
     from #tmp

   delete ShopProduct
   where Id in (select Id from #tmp)

   

   If(OBJECT_ID('tempdb..#tmp') Is Not Null)
   Begin
       Drop Table #tmp
   End

                ";
                    _connection.Execute(query);
                    
                    _connection.Execute(query2);
                }
                catch(Exception e)
                {

                }
                await Task.Delay(TimeSpan.FromMinutes(30), cancellationToken);
            }
        }
    }
}
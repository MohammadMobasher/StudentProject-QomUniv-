using AutoMapper;
using Core.Utilities;
using DataLayer.Entities.Warehouse;
using DataLayer.ViewModels.Warehouse;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.Warehouses
{
    public class WarehouseRepository : GenericRepository<Warehouse>
    {
        public WarehouseRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<SweetAlertExtenstion> UpdateMappingAsync(WarehouseUpdateViewModel vm)
        {
            try
            {
                var entity = Mapper.Map<Warehouse>(vm);
                await UpdateAsync(entity);
                return SweetAlertExtenstion.Ok();
            }
            catch
            {
                return SweetAlertExtenstion.Error();
            }
        }
    }
}

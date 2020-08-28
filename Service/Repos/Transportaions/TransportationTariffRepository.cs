using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.Transportations.Tariff;
using DataLayer.Entities.Transportation;
using DataLayer.ViewModels.Transportations.Tariff;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.Transportaions
{
    public class TransportationTariffRepository : GenericRepository<TransportationTariff>
    {
        public TransportationTariffRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }
        public async Task<Tuple<int, List<TransportationTariffFullDto>>> GetAllTariff(TariffSearchViewModel model, int skip, int take)
        {
            var query = TableNoTracking.Include(a => a.CarTransport).Where(a => !a.IsDeleted).ProjectTo<TransportationTariffFullDto>();

            query = query.WhereIf(!string.IsNullOrEmpty(model.CarName), a => a.CarTransportCarName.Contains(model.CarName));
            query = query.WhereIf(model.TehranAreasFrom != null, a => a.TehranAreasFrom == model.TehranAreasFrom);
            query = query.WhereIf(model.TehranAreasTO != null, a => a.TehranAreasTO == model.TehranAreasTO);
            query = query.WhereIf(model.ProductSizeTo != null, a => a.ProductSizeTo == model.ProductSizeTo);
            query = query.WhereIf(model.ProductSizeFrom != null, a => a.ProductSizeFrom == model.ProductSizeFrom);
            query = query.WhereIf(model.Tariff != null, a => a.Tariff == model.Tariff);

            int Count = query.Count();
            query = query.OrderByDescending(x => x.Id);

            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);

            return new Tuple<int, List<TransportationTariffFullDto>>(Count, await query.ToListAsync());
        }

        public async Task<TransportationTariffFullDto> GetTariffById(int id)
            => await TableNoTracking.Where(a => a.Id == id)
                .ProjectTo<TransportationTariffFullDto>().FirstOrDefaultAsync();


        public async Task<SweetAlertExtenstion> InsertTariff(TariffInsertViewModel model)
        {
            var result = model.ToEntity();

            await AddAsync(result);

            return SweetAlertExtenstion.Ok();
        }

        public async Task<SweetAlertExtenstion> UpdateTariff(TariffUpdateViewModel model)
        {
            var result = model.ToEntity(await GetByIdAsync(model.Id));

            await UpdateAsync(result);

            return SweetAlertExtenstion.Ok();
        }



        public async Task<SweetAlertExtenstion> DeleteTariff(int id)
        {
            var model = await GetByIdAsync(id);
            model.IsDeleted = true;

            await UpdateAsync(model);

            return SweetAlertExtenstion.Ok();
        }
    }
}

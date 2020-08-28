using AutoMapper.QueryableExtensions;
using Core.Utilities;
using DataLayer.DTO.Transportations.Cars;
using DataLayer.Entities.Transportation;
using DataLayer.ViewModels;
using DataLayer.ViewModels.Transportations.Car;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos.Transportaions
{
    public class CarTransportRepository : GenericRepository<CarTransport>
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public CarTransportRepository(DatabaseContext dbContext, IHostingEnvironment hostingEnvironment) : base(dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        ///TODO Use Generic Grid View 
        public async Task<Tuple<int, List<CarsTransportaionsFullDto>>> GetAllCars(CarTransportationSearchViewModel model, int skip, int take)
        {
            var query = TableNoTracking.Where(a => !a.IsDeleted).ProjectTo<CarsTransportaionsFullDto>();

            query = query.WhereIf(!string.IsNullOrEmpty(model.CarName), a => a.CarName.Contains(model.CarName));
            query = query.WhereIf(!string.IsNullOrEmpty(model.CarModel), a => a.CarModel.Contains(model.CarModel));
            query = query.WhereIf(!string.IsNullOrEmpty(model.MotorSerial), a => a.MotorSerial.Contains(model.MotorSerial));
            query = query.WhereIf(!string.IsNullOrEmpty(model.Plaque), a => a.Plaque.Contains(model.Plaque));
            query = query.WhereIf(model.TransportSize != null, a => a.TransportSize == model.TransportSize);

            int Count = query.Count();
            query = query.OrderByDescending(x => x.Id);

            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);

            return new Tuple<int, List<CarsTransportaionsFullDto>>(Count, await query.ToListAsync());
        }

        public async Task<CarsTransportaionsFullDto> GetCarById(int id)
            => await TableNoTracking.Where(a => a.Id == id)
                .ProjectTo<CarsTransportaionsFullDto>().FirstOrDefaultAsync();


        public async Task<SweetAlertExtenstion> InsertCar(CarTransportationInsertViewModel model, IFormFile file)
        {
            if (file == null)
            {
                model.Pic = "Images/no-Pic.jpg";
            }
            else
            {
                model.Pic = await MFile.Save(file, FilePath.Cars.GetDescription());
            }

            var result = model.ToEntity();

            await AddAsync(result);

            return SweetAlertExtenstion.Ok();
        }

        public async Task<SweetAlertExtenstion> UpdateCars(CarTransportaionUpdateViewModel model, IFormFile file)
        {
            if (file != null)
            {
                if (model.Pic != null)
                {
                    var WebContent = _hostingEnvironment.WebRootPath;
                    if (model.Pic != "Images/no-Pic.jpg")
                        System.IO.File.Delete(WebContent + FilePath.Product.GetDescription());
                }
                model.Pic = await MFile.Save(file, FilePath.Cars.GetDescription());
            }


            var result = model.ToEntity(await GetByIdAsync(model.Id));

            await UpdateAsync(result);

            return SweetAlertExtenstion.Ok();
        }

        public async Task<SweetAlertExtenstion> ChangeStatus(int id)
        {
            var model = await GetByIdAsync(id);
            model.IsActive = !model.IsActive;

            await UpdateAsync(model);

            return SweetAlertExtenstion.Ok();
        }


        public async Task<SweetAlertExtenstion> DeleteCar(int id)
        {
            var model = await GetByIdAsync(id);
            model.IsDeleted = true;

            await UpdateAsync(model);

            return SweetAlertExtenstion.Ok();
        }

        public async Task<List<IdTitle>> GetCarsIdTitle()
        {
            var model = await GetListAsync(a => !a.IsDeleted && a.IsActive);

            return model.Select(a => new IdTitle() { Id = a.Id, Title = a.CarName }).ToList();
        }
    }
}

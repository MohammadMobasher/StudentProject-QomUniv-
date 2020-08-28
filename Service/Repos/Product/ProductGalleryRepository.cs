using Core.Utilities;
using DataLayer.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Repos
{
    public class ProductGalleryRepository : GenericRepository<DataLayer.Entities.ProductGallery>
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ProductGalleryRepository(DatabaseContext dbContext,IHostingEnvironment hostingEnvironment) : base(dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// آپلود گالری
        /// </summary>
        /// <returns></returns>
        
        public async Task<SweetAlertExtenstion> UploadGalley(List<IFormFile> files, int productId)
        {
            var lst = new List<ProductGallery>();

            foreach (var item in files)
            {
                lst.Add(new ProductGallery()
                {
                    ProductId = productId,
                    Pic = await UploadPic(item)
                });
            }

            await AddRangeAsync(lst);

            return SweetAlertExtenstion.Ok();
        }

        public async Task UpdateRemindedGallery(List<int> remindedGallery, int productId)
        {
            var productGallery = await TableNoTracking.Where(a => a.ProductId == productId &&
                !remindedGallery.Contains(a.Id)).ToListAsync();

            DeletePic(productGallery.Select(x => x.Pic).ToList());

            await DeleteRangeAsync(productGallery);
        }


         async Task<string> UploadPic(IFormFile file)
            => await MFile.Save(file, FilePath.ProductGallery.GetDescription());

        public void DeletePic(List<string> path)
        {
            var WebAddress = _hostingEnvironment.WebRootPath;

            foreach (var item in path)
            {
                System.IO.File.Delete(WebAddress +"\\"+ item);
            }
        }
        
        /// <summary>
        /// لیست گالری بر اساس شناسه محصول
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<ProductGallery>> GetListGalleryByProductId(int id)
        {
            var model = await TableNoTracking.Where(a => a.ProductId == id).ToListAsync();

            return model;
        }
    }
}

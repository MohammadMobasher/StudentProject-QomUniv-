using DataLayer.Entities;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Utilities;
using DataLayer.ViewModels.SiteSetting;
using System;

namespace Service.Repos
{
    public class SiteSettingRepository : GenericRepository<SiteSetting>
    {
        public SiteSettingRepository(DatabaseContext dbContext) : base(dbContext)
        {
        }


        public async Task<SiteSetting> GetInfo()
        {
            return await Entities.FirstOrDefaultAsync();
        }


        public async Task<SweetAlertExtenstion> UpdateInfo(SiteSettingInsertViewModel vm)
        {
            try
            {
                var entity = await Entities.FirstOrDefaultAsync();
                if(vm.LogoFile != null)
                {
                    //حذف فایل قبلی
                    await MFile.Delete(entity.Logo);
                    // ذخیره فایل جدید
                    entity.Logo = await MFile.Save(vm.LogoFile, "Uploads/SiteSetting");
                }

                if (vm.TabIconFile != null)
                {
                    //حذف فایل قبلی
                    await MFile.Delete(entity.TabIcon);
                    // ذخیره فایل جدید
                    entity.TabIcon = await MFile.Save(vm.TabIconFile, "Uploads/SiteSetting");
                }

                entity.InstaURL = vm.InstaURL;
                entity.WhatsAppURL = vm.WhatsAppURL;
                entity.TwitterURL = vm.TwitterURL;
                entity.TelegramURL = vm.TelegramURL;

                await DbContext.SaveChangesAsync();

                return SweetAlertExtenstion.Ok();
            }
            catch(Exception e) {
                return SweetAlertExtenstion.Error();
            }
        }
    }
}

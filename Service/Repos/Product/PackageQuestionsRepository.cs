using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Core.Utilities;
using DataLayer.Entities;
using DataLayer.ViewModels.Feature;

namespace Service.Repos.Package {
    public class PackageQuestionsRepository : GenericRepository<Feature> {
        public PackageQuestionsRepository (DatabaseContext dbContext) : base (dbContext) { }

        /// <summary>
        /// تعریف یک سوال برای ویژگی
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public async Task<SweetAlertExtenstion> SetQuestion (FeatureQuestionsViewModel vm) {
            var model = await GetByIdAsync (vm.Id);

            if (model == null) return SweetAlertExtenstion.Error ();

            if (model.Id == vm.Id && model.IsQuestion == true) {
                return SweetAlertExtenstion.Error ("برای این ویژگی قبلا سوالی به ثبت رسیده است");
            }

            Mapper.Map (vm, model);

            return Save ();
        }

    }
}
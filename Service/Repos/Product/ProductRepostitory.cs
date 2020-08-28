using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Utilities;
using Dapper;
using DataLayer.DTO.Feature;
using DataLayer.DTO.FeatureItem;
using DataLayer.DTO.Products;
using DataLayer.ViewModels.Products;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Service.Repos.Product;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.SSOT;
using DataLayer.ViewModels.Feature;
using DataLayer.DTO;
using DataLayer.DTO.ProductGroupDependencies;
using DataLayer.DTO.ProductFeatures;
using System.Text.RegularExpressions;
using DataLayer.ViewModels;

namespace Service.Repos
{
    public class ProductRepostitory : GenericRepository<DataLayer.Entities.Product>
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ProductDiscountRepository _productDiscountRepository;
        private readonly IDbConnection _connection;

        public ProductRepostitory(DatabaseContext dbContext,
            IHostingEnvironment hostingEnvironment,
            ProductDiscountRepository productDiscountRepository,
            IDbConnection connection) : base(dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _productDiscountRepository = productDiscountRepository;
            _connection = connection;
        }


        public async Task<Tuple<int, List<ProductFullDTO>>> LoadAsyncCount(
         int skip = -1,
         int take = -1,
         ProductSearchViewModel model = null)
        {
            var query = Entities.Where(a => !a.IsDeleted).ProjectTo<ProductFullDTO>();


            if (model.Id != null)
                query = query.Where(x => x.Id == model.Id);

            if (!string.IsNullOrEmpty(model.Title))
                query = query.Where(x => x.Title.Contains(model.Title));

            if (model.Price != null)
            {
                query = query.Where(x => x.Price == model.Price);
            }

            if (model.ProductGroupId != null)
            {
                query = query.Where(x => x.ProductGroupId == model.ProductGroupId.Value);
            }

            if (model.Likes != null)
            {
                query = query.Where(x => (x.Like - x.DisLike) == model.Likes.Value);
            }


            int Count = query.Count();

            query = query.OrderByDescending(x => x.Id);


            if (skip != -1)
                query = query.Skip((skip - 1) * take);

            if (take != -1)
                query = query.Take(take);

            return new Tuple<int, List<ProductFullDTO>>(Count, await query.ToListAsync());
        }

        public async Task<List<select2IdTextImage>> SearchForSelect2(string term)
        {
            term = term.Trim();
            term = Regex.Replace(term, " ( )+", " ");


            return await (from product in DbContext.Product
                          where
                                   product.Title.Contains(term.CleanString())
                                || product.ShortDescription.Contains(term.CleanString())
                                || product.SearchKeyWord.Contains(term.CleanString())
                                || product.Tags.Contains(term.CleanString())

                          select new select2IdTextImage
                          {
                              Id = product.Id,
                              Text = product.Title,
                              Image = product.IndexPic
                          }).ToListAsync();
        }


        /// <summary>
        /// گرفتن اطلاعات گروه محصول بر اساس شناسه محصول
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int?> GetProductGroupIdbyProductId(int id)
        {
            var product = await TableNoTracking.FirstOrDefaultAsync(a => a.Id == id);

            if (product == null) return null;

            return product.ProductGroupId;
        }

        public async Task<int> SubmitProduct(DataLayer.ViewModels.Products.ProductInsertViewModel vm, IFormFile file)
        {
            if (file == null)
            {
                vm.IndexPic = "Images/no-Pic.jpg";
            }
            else
            {
                vm.IndexPic = await MFile.Save(file, FilePath.Product.GetDescription());
            }

            var mapModel = Map(vm);

            await AddAsync(mapModel);

            return mapModel.Id;
        }

        public async Task<int> UpdateProduct(DataLayer.ViewModels.Products.ProductUpdateViewModel vm, IFormFile file)
        {
            if (file != null)
            {
                if (vm.IndexPic != null)
                {
                    var WebContent = _hostingEnvironment.WebRootPath;
                    if (vm.IndexPic != "Images/no-Pic.jpg")
                        System.IO.File.Delete(WebContent + FilePath.Product.GetDescription());
                }

                vm.IndexPic = await MFile.Save(file, FilePath.Product.GetDescription());
            }


            var model = GetById(vm.Id);
            if (file == null)
                vm.IndexPic = model.IndexPic;
            Mapper.Map(vm, model);

            DbContext.SaveChanges();

            return model.Id;
        }


        /// در این تابع لیست ویژگی‌هایی برگردانده می‌شود که متعلق به یک محول است
        /// </summary>
        /// <param name="productId">شماره محصول</param>
        /// <returns></returns>
        public async Task<List<FeatureValueFullDetailDTO>> GetFeaturesValuesByProductId(int productId)
        {

            return await (from product in this.DbContext.Product
                          where product.Id == productId

                          // ارتباط با ویژگی‌های مربوط به گروه این کالا
                          // با این جدول به این جهت ارتباط میدهیم تا اگر یک ویژگی به این گروه اضافه شده باشد
                          // بتواند به عنوان ویژگی‌ جدید به کاربر نمایش دهیم
                          join productGroupFeature in this.DbContext.ProductGroupFeature on product.ProductGroupId equals productGroupFeature.ProductGroupId


                          //join productFeature in this.DbContext.ProductFeature on product.Id equals productFeature.ProductId

                          // برای این منظور با این جدول ارتباط میدهیم که بتوانیم اسم ویژگی و نوع آن را به دست آوریم
                          join feature in this.DbContext.Feature on productGroupFeature.FeatureId equals feature.Id

                          select new FeatureValueFullDetailDTO
                          {
                              Id = feature.Id,
                              Title = feature.Title,
                              FeatureType = feature.FeatureType,
                              IsRequired = feature.IsRequired,
                              FeatureItems = (from featureItem in this.DbContext.FeatureItem
                                              where featureItem.FeatureId == feature.Id
                                              select new FeatureItemDTO
                                              {
                                                  Id = featureItem.Id,
                                                  Value = featureItem.Value,
                                                  FeatureId = feature.Id
                                              }).ToList(),

                              //// با این جدول به این جهت که بتوان مقادیر ویژگی‌ها را به دست آورد ارتباط میدهیم
                              Value = (from productFeature in this.DbContext.ProductFeature
                                       where productFeature.FeatureId == feature.Id && productFeature.ProductId == productId
                                       select productFeature.FeatureValue).SingleOrDefault()

                          }).ToListAsync();
        }


        public async Task ChangeStateProduct(int id)
        {
            var model = await GetByIdAsync(id);

            model.IsActive = model.IsActive == null ? true : !model.IsActive;
            await UpdateAsync(model);
        }


        public async Task ChangeSpecial(int id)
        {
            var model = await GetByIdAsync(id);

            model.IsSpecialSell = !model.IsSpecialSell;

            await UpdateAsync(model);
        }

        /// <summary>
        /// شماره محصولاتی که در یک گروه قرار دارد
        /// </summary>
        /// <param name="productGroupId">شماره گروه مورد نظر</param>
        /// <returns></returns>
        public async Task<List<int>> GetProductIdsByGroupId(int productGroupId)
        {
            return await Entities.Where(x => x.ProductGroupId == productGroupId).Select(x => x.Id).ToListAsync();
        }



        /// <summary>
        /// تعداد کالاهایی که دارای یک واحد میباشند
        /// </summary>
        /// <param name="productUnitId">شماره واحد مورد نظر</param>
        /// <returns></returns>
        public async Task<int> NumProductByProductUnitId(int productUnitId)
        {
            var result = await Entities.Where(x => x.ProductUnitId == productUnitId).ToListAsync();
            if (result != null && result.Count > 0)
                return result.Count;
            return 0;

        }

        public async Task<SweetAlertExtenstion> Delete(int ID)
        {
            try
            {
                var value = new { productId = ID };
                var results = _connection.Query("[productDeleteSP]", value, commandType: CommandType.StoredProcedure).ToList();


                return SweetAlertExtenstion.Ok();
            }
            catch (Exception E)
            {
                return SweetAlertExtenstion.Error();
            }
        }

        /// <summary>
        /// تعداد کالاهایی که دارای یک واحد میباشند
        /// </summary>
        /// <param name="productUnitId">شماره واحد مورد نظر</param>
        /// <returns></returns>
        public async Task<bool> DeleteByProductUnitId(int productUnitId)
        {
            try
            {
                var result = await Entities.Where(x => x.ProductUnitId == productUnitId).ToListAsync();

                if (await _productDiscountRepository.DeleteByProductIds(result.Select(x => x.Id).ToList()))
                    await DeleteRangeAsync(result);
                else
                    return false;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        /// <summary>
        /// اضافه کردن تعداد بازدید کننده
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task VisitPlus(int id)
        {
            var model = await GetByIdAsync(id);

            model.Visit = model.Visit + 1;
            await UpdateAsync(model);
        }

        /// <summary>
        /// اضافه کردن تعداد لایک
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> AddLike(int id)
        {
            var model = await GetByIdAsync(id);

            model.Like = model.Like + 1;
            await UpdateAsync(model);

            return model.Like;
        }


        /// <summary>
        /// اضافه کردن تعداد دیس لایک
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> AddDisLike(int id)
        {
            var model = await GetByIdAsync(id);

            model.DisLike = model.DisLike + 1;
            await UpdateAsync(model);

            return model.DisLike;
        }


        public List<ProductFullDTO> GetProductByGroupId(int groupId, int take = -1)
        {

            var result = TableNoTracking.ProjectTo<ProductFullDTO>().Where(x =>
           x.IsActive == true &&
           x.ProductGroupId == groupId);

            if (take != -1)
                result = result.Skip(1).Take(take);

            return result.ToList();
        }


        public async Task<List<ProductFullDTO>> GetProductByGroupIdAsync(int groupId, int take = -1)
        {

            var result = TableNoTracking.ProjectTo<ProductFullDTO>().Where(x =>
           x.IsActive == true &&
           x.ProductGroupId == groupId);

            if (take != -1)
                result = result.Skip(1).Take(take);

            return await result.ToListAsync();
        }


        public async Task<Tuple<int, List<ProductFullDTO>>> GetProductQuery(int productGroupId, string titleSearch, List<int> selectedsubGroup, List<FeatureSearchableViewModel> featureSearchableVM, int skip, int take)
        {

            string groupAndSubGroupId = "";
            if (selectedsubGroup != null && selectedsubGroup.Count > 0)
            {
                selectedsubGroup.Add(productGroupId);
                groupAndSubGroupId = string.Join(",", selectedsubGroup);
            }

            string featureSearchWhere = "";

            if (featureSearchableVM != null && featureSearchableVM.Count > 0)
            {
                featureSearchWhere += @" where Id in ( select ProductId
		                                    from ProductFeature
		                                    where ";

                foreach (var distinctFeature in featureSearchableVM.DistinctBy(x => x.FeaureId))
                {

                    featureSearchWhere += " (ProductFeature.FeatureId in (" + distinctFeature.FeaureId + ") ";
                    featureSearchWhere += @" and ProductFeature.FeatureValue in ( " +
                        string.Join(",", featureSearchableVM.Where(x => x.FeaureId == distinctFeature.FeaureId).Select(x => x.FeatureValue))
                        + ")) or ";

                }
                featureSearchWhere = featureSearchWhere.Substring(0, featureSearchWhere.Length - 3) + @" group by ProductId
		            having count(ProductId) = " + featureSearchableVM.DistinctBy(x => x.FeaureId).Count() + ") and ";
            }

            string sql = @"
                        declare @T table(Id int);

                        with A as (
                           select Id, ParentId
                               from ProductGroup
                               where Id = " + productGroupId + @"
                               union all
                           select c.Id, c.ParentId
                               from ProductGroup c
                                   join A p on p.Id = c.ParentId) 

                        insert into @T(Id)
                          select Id
                        from A;";

            string countQuery = $@"                    
                    select count(*) as Count from Product " +
                        (featureSearchableVM != null && featureSearchableVM.Count > 0 ? featureSearchWhere : " where ")
                    + @"
                    ProductGroupId in (" + ((selectedsubGroup != null && selectedsubGroup.Count > 0 ? groupAndSubGroupId : "select * from @T")) + @")
                        and Title LIKE '%" + titleSearch + @"%' and Product.IsDeleted = 0;";

            var productQuery = $@"                    
                    select Product.* from Product " +
                    (featureSearchableVM != null && featureSearchableVM.Count > 0 ? featureSearchWhere : " where ")
                    + @"
                    ProductGroupId in (" + ((selectedsubGroup != null && selectedsubGroup.Count > 0 ? groupAndSubGroupId : "select * from @T")) + @")
                        and Title LIKE '%" + titleSearch + @"%' and Product.IsDeleted = 0
                    order by NEWID()
                    OFFSET " + (skip - 1) * take + @" ROWS
                    FETCH NEXT " + take + @" ROWS ONLY;
                ";

            try
            {

                var results = await _connection.QueryMultipleAsync(sql + countQuery + productQuery);

                var CountResult = await results.ReadAsync<CountDTO>();
                var ProductsResult = await results.ReadAsync<ProductFullDTO>();

                return new Tuple<int, List<ProductFullDTO>>(CountResult.SingleOrDefault().Count, ProductsResult.ToList());
            }
            catch (Exception e)
            {

                return new Tuple<int, List<ProductFullDTO>>(0, new List<ProductFullDTO>()); ;
            }


        }

        public async Task<long> ResultPrice(int productId)
        {
            var product = await GetByIdAsync(productId);
            var productDiscount = await _productDiscountRepository.GetByConditionAsync(a => a.ProductId == productId);
            if (productDiscount == null) return product.Price;

            if (DateTime.Now > productDiscount.StartDate && DateTime.Now < productDiscount.EndDate)
            {


                var calculate = productDiscount.DiscountType == ProductDiscountSSOT.Percent ?
                    (product.Price - (product.Price * productDiscount.Discount) / 100)
                    : (product.Price - productDiscount.Discount);

                return calculate;
            }

            return product.Price;
        }

        public async Task<Tuple<int, List<DataLayer.Entities.Product>>> GetProducts(ProductSearchListViewModel vm, int skip, int take)
        {

            vm.Title = Regex.Replace(vm.Title, " ( )+", " ");

            var model = TableNoTracking
               .Include(a => a.ProductGroup)
               .Where(a => a.IsActive == true && !a.IsDeleted);
            if (vm.Group != null && vm.Group != -1)
            {
                string sql = @"
                        declare @T table(Id int);

                        with A as (
                           select Id, ParentId
                               from ProductGroup
                               where Id = " + vm.Group.Value + @"
                               union all
                           select c.Id, c.ParentId
                               from ProductGroup c
                                   join A p on p.Id = c.ParentId) 

                        select Id from A";

                var result = await _connection.QueryMultipleAsync(sql);
                var groupsId = await result.ReadAsync<int>();


                model = model
                .WhereIf(!string.IsNullOrEmpty(vm.Title), a =>
                a.Title.Contains(vm.Title.CleanString())
                || a.ShortDescription.Contains(vm.Title.CleanString())
                //|| a.Text.CleanString().Contains(vm.Title.CleanString())
                //|| a.Text.Contains(vm.Title)
                || a.SearchKeyWord.Contains(vm.Title.CleanString())
                || a.Tags.Contains(vm.Title.CleanString()))
                //.WhereIf(vm.Group != null && vm.Group != -1, a => a.ProductGroupId.Equals(vm.Group.Value))
                .WhereIf(vm.Group != null && vm.Group != -1, a => groupsId.Contains(a.ProductGroupId))
                .WhereIf(vm.MaxPrice != null && vm.MinPrice != null, a => a.Price >= long.Parse(vm.MinPrice) && a.Price <= long.Parse(vm.MaxPrice));

            }

            else
            {
                model = model
                    .WhereIf(!string.IsNullOrEmpty(vm.Title), a => a.Title.Contains(vm.Title)
                    || a.ShortDescription.Contains(vm.Title)
                    // || a.Text.Contains(vm.Title)
                    || a.SearchKeyWord.Contains(vm.Title)
                    || a.Tags.Contains(vm.Title))

                    .WhereIf(vm.MaxPrice != null && vm.MinPrice != null, a => a.Price >= long.Parse(vm.MinPrice) && a.Price <= long.Parse(vm.MaxPrice));
            }

            var count = model.Count();

            if (skip != 0)
                model = model.Skip((skip - 1) * take);

            if (take != 0)
                model = model.Take(take);

            return new Tuple<int, List<DataLayer.Entities.Product>>(count, await model.ToListAsync());
        }



        public async Task<Tuple<int, List<DataLayer.Entities.Product>>> GetProductsDiscount(ProductSearchListViewModel vm, int skip, int take)
        {

            var model = TableNoTracking
               .Include(a => a.ProductGroup)
               .Where(a => a.IsActive == true && !a.IsDeleted && a.Price != a.PriceWithDiscount);
            if (vm.Group != null && vm.Group != -1)
            {
                string sql = @"
                        declare @T table(Id int);

                        with A as (
                           select Id, ParentId
                               from ProductGroup
                               where Id = " + vm.Group.Value + @"
                               union all
                           select c.Id, c.ParentId
                               from ProductGroup c
                                   join A p on p.Id = c.ParentId) 

                        select Id from A";

                var result = await _connection.QueryMultipleAsync(sql);
                var groupsId = await result.ReadAsync<int>();


                model = model
                .WhereIf(!string.IsNullOrEmpty(vm.Title), a => a.Title.Contains(vm.Title)
                || a.ShortDescription.Contains(vm.Title)
                || a.Text.Contains(vm.Title)
                || a.Tags.Contains(vm.Title))
                //.WhereIf(vm.Group != null && vm.Group != -1, a => a.ProductGroupId.Equals(vm.Group.Value))
                .WhereIf(vm.Group != null && vm.Group != -1, a => groupsId.Contains(a.ProductGroupId))
                .WhereIf(vm.MaxPrice != null && vm.MinPrice != null, a => a.Price >= long.Parse(vm.MinPrice) && a.Price <= long.Parse(vm.MaxPrice));

            }

            else
            {
                model = model
                    .WhereIf(!string.IsNullOrEmpty(vm.Title), a => a.Title.Contains(vm.Title)
                    || a.ShortDescription.Contains(vm.Title)
                    || a.Text.Contains(vm.Title)
                    || a.Tags.Contains(vm.Title))

                    .WhereIf(vm.MaxPrice != null && vm.MinPrice != null, a => a.Price >= long.Parse(vm.MinPrice) && a.Price <= long.Parse(vm.MaxPrice));
            }
            var count = model.Count();

            if (skip != 0)
                model = model.Skip((skip - 1) * take);

            if (take != 0)
                model = model.Take(take);

            return new Tuple<int, List<DataLayer.Entities.Product>>(count, await model.ToListAsync());
        }


        /// <summary>
        /// گرفتن اطلاعات محصول بر اساس شناسه برا نمایش جزِئیات محصول
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProductFullDTO> GetProductDetail(int id)
        {
            var model = await TableNoTracking
                .Include(a => a.ProductGroup)
                .Where(a => a.Id == id && !a.IsDeleted)
                .ProjectTo<ProductFullDTO>()
                .FirstOrDefaultAsync();

            return model;
        }

        public async Task<List<ProductQueryFullDTO>> GetProductForPackage(int packageId)
        {
            var sqlQuery = $@"
                With A as (
                select FeatureQuestionForPakage.GroupId, PackageUserAnswers.Answer,PackageUserAnswers.FeatureId  from PackageUserAnswers
                join FeatureQuestionForPakage on PackageUserAnswers.QuestionId = FeatureQuestionForPakage.Id
                where PackageUserAnswers.PackageId= {packageId}
                )
                select
                Product.Id,Product.Title,Product.Price,Product.PriceWithDiscount,Product.ProductGroupId, 
                ProductGroup.Title As GroupTitle, ProductGroup.ParentId 
                from Product
                join ProductGroup on Product.ProductGroupId = ProductGroup.Id
                join ProductFeature on Product.Id = ProductFeature.ProductId
                where ProductGroup.Id in (select GroupId from A) or ProductGroup.ParentId in (select GroupId from A) and 
                ProductFeature.FeatureId in (select FeatureId from A) and 
                ProductFeature.FeatureValue in (select Answer from A ) and 
                Product.IsActive = 1
                ";

            var model = await _connection.QueryAsync<ProductQueryFullDTO>(sqlQuery);

            return model.ToList();
        }

        public async Task<List<ProductQueryFullDTO>> GetProductForPackage(int packageId
            , int groupId, List<int> beforeGroups, string title = null)
        {
            try
            {
                var sqlQuery = $@"
                declare @T table(Id int);
                declare @thisGroup_ table(Id int);
                declare @beforeGroup_ table(Id int);

                With A as (
	                select 
		                FeatureQuestionForPakage.GroupId,
		                PackageUserAnswers.Answer,
		                PackageUserAnswers.FeatureId  
	                from PackageUserAnswers
		                join FeatureQuestionForPakage on PackageUserAnswers.QuestionId = FeatureQuestionForPakage.Id
                    where 
		                PackageUserAnswers.PackageId= ${packageId} and
		                FeatureQuestionForPakage.GroupId = ${groupId} 
                )


                insert into @T(Id)  
                  select
	                  Product.Id
                  from ProductFeature
                  join Product on 
		                Product.Id = ProductFeature.ProductId
                  join ProductGroup on Product.ProductGroupId = ProductGroup.Id
	                where 
	                  ProductFeature.FeatureId in (select FeatureId from A) and 
	                  ProductFeature.FeatureValue in (select Answer from A ) and
	                  (ProductGroup.Id in (select GroupId from A) or ProductGroup.ParentId in (select GroupId from A)) and 
	                  Product.IsActive = 1


                select * from Product where Id in (select * from @T);
                select * from ProductFeature where ProductId in (select * from @T);
                
";

                if (beforeGroups != null && beforeGroups.Count > 0)
                {
                    sqlQuery += $@"with thisGroup as (
                           select Id, ParentId
                               from ProductGroup
                               where Id = { groupId}
                    union all
                           select c.Id, c.ParentId
                               from ProductGroup c
                                   join thisGroup p on p.Id = c.ParentId)

                
                insert into @thisGroup_(Id)
                          select Id
                        from thisGroup;

                    with beforeGroup as (
                               select Id, ParentId
                                   from ProductGroup
                                   where Id in ({ string.Join(',', beforeGroups)})
                               union all
                           select c.Id, c.ParentId
                               from ProductGroup c
                                   join beforeGroup p on p.Id = c.ParentId)

                insert into @beforeGroup_(Id)
                          select Id
                        from beforeGroup;
                select 
                    *,
                    Condition.Name

                from ProductGroupDependencies 
                join Condition on ProductGroupDependencies.ConditionId = Condition.Id
                where 
                    GroupId1 in (select Id from @thisGroup_) and
                    GroupId2 in (select Id from @beforeGroup_);
                    
                select Distinct
	                Feature.Id as FeatureId,
	                ProductFeature.[FeatureValue],
	                Product.Id as ProductId,
	                Product.ProductGroupId
	            from ProductPackageDetails
	               join Product on ProductPackageDetails.ProductId = Product.Id
	               join ProductFeature on Product.Id = ProductFeature.ProductId 
	               join Feature on ProductFeature.FeatureId = Feature.Id
	            where PackageId = 15 and Feature.FeatureType = 2 and Product.ProductGroupId in (select Id from @beforeGroup_)

                    
                    ";
                }



                var results = await _connection.QueryMultipleAsync(sqlQuery);
                var products = await results.ReadAsync<ProductQueryFullDTO>();
                var Features = await results.ReadAsync<ProductFeaturesFullDTO>();
                if (beforeGroups != null && beforeGroups.Count > 0)
                {
                    var dependency = await results.ReadAsync<ProductGroupDependenciesFullDTO>();
                    if (dependency != null && dependency.Count() > 0)
                    {
                        var selectedProduct = await results.ReadAsync<ProductPackageSelectedDTO>();
                        foreach (ProductGroupDependenciesFullDTO item in dependency)
                        {
                            var features = selectedProduct.Where(x => x.FeatureId == item.Feature2 && x.FeatureValue == item.Value2);
                            if (features.Any())
                            {
                                switch (item.Name)
                                {
                                    case ">":

                                        products = products.Where(x =>

                                            Features
                                            .Where(a =>
                                                a.FeatureId == item.Feature1 &&
                                                Convert.ToInt32(a.FeatureValue) > Convert.ToInt32(item.Value1))
                                            .Select(a => a.ProductId).ToList().Contains(x.Id)

                                        );


                                        break;

                                    case ">=":

                                        products = products.Where(x =>

                                            Features
                                            .Where(a =>
                                                a.FeatureId == item.Feature1 &&
                                                Convert.ToInt32(a.FeatureValue) >= Convert.ToInt32(item.Value1))
                                            .Select(a => a.ProductId).ToList().Contains(x.Id)

                                        );


                                        break;


                                    case "<":

                                        products = products.Where(x =>

                                            Features
                                            .Where(a =>
                                                a.FeatureId == item.Feature1 &&
                                                Convert.ToInt32(a.FeatureValue) < Convert.ToInt32(item.Value1))
                                            .Select(a => a.ProductId).ToList().Contains(x.Id)

                                        );


                                        break;

                                    case "<=":

                                        products = products.Where(x =>

                                            Features
                                            .Where(a =>
                                                a.FeatureId == item.Feature1 &&
                                                Convert.ToInt32(a.FeatureValue) <= Convert.ToInt32(item.Value1))
                                            .Select(a => a.ProductId).ToList().Contains(x.Id)

                                        );


                                        break;
                                    case "!=":

                                        products = products.Where(x =>

                                            Features
                                            .Where(a =>
                                                a.FeatureId == item.Feature1 &&
                                                Convert.ToInt32(a.FeatureValue) != Convert.ToInt32(item.Value1))
                                            .Select(a => a.ProductId).ToList().Contains(x.Id)

                                        );


                                        break;

                                    case "==":

                                        products = products.Where(x =>

                                            Features
                                            .Where(a =>
                                                a.FeatureId == item.Feature1 &&
                                                Convert.ToInt32(a.FeatureValue) == Convert.ToInt32(item.Value1))
                                            .Select(a => a.ProductId).ToList().Contains(x.Id)

                                        );


                                        break;
                                }
                            }
                        }
                    }
                }

                var result = products.AsQueryable().WhereIf(title != null, a => a.Title.Contains(title)).ToList();

                return result;
            }
            catch (Exception E)
            {
                return null;
            }
        }


        public async Task<SweetAlertExtenstion> DeletedProduct(int id)
        {
            var model = await GetByIdAsync(id);

            model.IsDeleted = true;

            await UpdateAsync(model, false);

            return Save();
        }

    }
}

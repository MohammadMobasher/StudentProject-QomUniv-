//using Core.Utilities;
//using DataLayer.Entities;
//using DataLayer.Entities.FAQs;
//using DataLayer.Entities.Common;
//using DataLayer.Entities.Users;
//using DataLayer.Entities.Warehouse;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Service
//{
//    public class DatabaseContext : IdentityDbContext<Users, Roles, int, UserClams, UserRoles, UserLogin, RoleClams, UserTokens>
//    {
//        public DatabaseContext(DbContextOptions options) : base(options)
//        {
//        }

//        #region Tables

//        public DbSet<Log> Log { get; set; }
//        public DbSet<Tree> Tree { get; set; }
//        public DbSet<UserTreeReminded> UserTreeReminded { get; set; }
//        public DbSet<ShopOrderPayment> ShopOrderPayment { get; set; }
//        public DbSet<CarTransport> CarTransport { get; set; }
//        public DbSet<TransportationTariff> TransportationTariff { get; set; }
//        public DbSet<SiteSetting> SiteSetting { get; set; }
//        public DbSet<ShopOrderStatus> ShopOrderStatus { get; set; }
//        public DbSet<SuggestionsAndComplaint> SuggestionsAndComplaint { get; set; }
//        public DbSet<FeatureQuestionForPakage> FeatureQuestionForPakage { get; set; }
//        public DbSet<Feature> Feature { get; set; }
//        public DbSet<FAQ> FAQ { get; set; }
//        public DbSet<LogoManufactory> LogoManufactory { get; set; }
//        public DbSet<FaqGroup> FaqGroup { get; set; }
//        public DbSet<FeatureItem> FeatureItem { get; set; }
//        public DbSet<Product> Product { get; set; }
//        public DbSet<Warehouse> Warehouse { get; set; }
//        public DbSet<WarehouseProductCheck> WarehouseProductCheck { get; set; }
//        public DbSet<FactorAndPackage> FactorAndPackage { get; set; }
//        public DbSet<FactorItem> FactorItem { get; set; }
//        public DbSet<Condition> Condition { get; set; }
//        public DbSet<ProductFeature> ProductFeature { get; set; }
//        public DbSet<UsersAccess> UsersAccess { get; set; }
//        public DbSet<UsersPayment> UsersPayment { get; set; }
//        public DbSet<ShopProduct> ShopProduct { get; set; }
//        public DbSet<ShopOrder> ShopOrder { get; set; }
//        public DbSet<ShopOrderDetails> ShopOrderDetails { get; set; }

//        public DbSet<ProductPackageGroups> ProductPackageGroups { get; set; }

//        #region News

//        public DbSet<News> News { get; set; }
//        public DbSet<NewsComment> NewsComment { get; set; }
//        public DbSet<NewsGroup> NewsGroup { get; set; }
//        public DbSet<NewsTag> NewsTag { get; set; }

//        #endregion

//        public DbSet<ProductGroup> ProductGroup { get; set; }
//        public DbSet<ProductUnit> ProductUnit { get; set; }
//        public DbSet<StoreRoom> StoreRoom { get; set; }
//        public DbSet<ProductGroupDependencies> ProductGroupDependencies { get; set; }
//        public DbSet<ProductGroupFeature> ProductGroupFeature { get; set; }
//        public DbSet<ProductGallery> Gallery { get; set; }
//        public DbSet<SlideShow> SlideShow { get; set; }
//        public DbSet<ProductPackage> ProductPackage { get; set; }
//        public DbSet<ProductPackageGallery> ProductPackageGallery { get; set; }
//        public DbSet<ProductPackageDetails> ProductPackageDetails { get; set; }

//        public DbSet<ProductDiscount> ProductDiscount { get; set; }
//        public DbSet<PackageUserAnswers> PackageUserAnswers { get; set; }
//        public DbSet<ProductPackageDiscount> ProductPackageDiscount { get; set; }

//        public DbSet<Favorites> Favorites { get; set; }
//        public DbSet<UserAddress> UserAddress { get; set; }
//        #endregion

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {

//            base.OnModelCreating(modelBuilder);

//            var entitiesAssembly = typeof(IEntity).Assembly;

//            //modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
//            //modelBuilder.AddRestrictDeleteBehaviorConvention();

//            modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);
//            modelBuilder.AddSequentialGuidForIdConvention();

//            //modelBuilder.Entity<ProductGroup>()
//            //        .HasOne(i => i.Parent)
//            //        .WithMany()
//            //        .HasForeignKey(i => i.ParentId);

//            //modelBuilder.Entity<ProductGroup>(entity =>
//            //{
//            //    entity
//            //        .HasOne(e => e.ParentGroup)
//            //        .WithOne(e => e.ParentArticleComment) //Each comment from Replies points back to its parent
//            //        .HasForeignKey(e => e.ParentArticleCommentId);
//            //});


//        }

        

//        #region CleanString
//        // این بخش برای یکپارچه سازی کاراکتر ها می باشد به صورتی که اگر کاربری 
//        // ی عربی وارد کرد تبدیل به ی فارسی و ....

//        public override int SaveChanges()
//        {
//            _cleanString();
//            return base.SaveChanges();
//        }

//        public override int SaveChanges(bool acceptAllChangesOnSuccess)
//        {
//            _cleanString();
//            return base.SaveChanges(acceptAllChangesOnSuccess);
//        }

//        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
//        {
//            _cleanString();
//            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
//        }

//        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
//        {
//            _cleanString();
//            return base.SaveChangesAsync(cancellationToken);
//        }

//        private void _cleanString()
//        {
//            var changedEntities = ChangeTracker.Entries()
//                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
//            foreach (var item in changedEntities)
//            {
//                if (item.Entity == null)
//                    continue;

//                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
//                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

//                foreach (var property in properties)
//                {
//                    var propName = property.Name;
//                    var val = (string)property.GetValue(item.Entity, null);

//                    if (val.HasValue())
//                    {
//                        var newVal = val.Fa2En().FixPersianChars();
//                        if (newVal == val)
//                            continue;
//                        property.SetValue(item.Entity, newVal, null);
//                    }
//                }
//            }
//        }

//        #endregion
//    }
//}

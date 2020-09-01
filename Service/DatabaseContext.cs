using Core.Utilities;
using DataLayer.Entities;
using DataLayer.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ConvertAddress> ConvertAddress { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            var entitiesAssembly = typeof(IEntity).Assembly;

            //modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
            //modelBuilder.AddRestrictDeleteBehaviorConvention();

            modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);
            modelBuilder.AddSequentialGuidForIdConvention();

            //modelBuilder.Entity<ProductGroup>()
            //        .HasOne(i => i.Parent)
            //        .WithMany()
            //        .HasForeignKey(i => i.ParentId);

            //modelBuilder.Entity<ProductGroup>(entity =>
            //{
            //    entity
            //        .HasOne(e => e.ParentGroup)
            //        .WithOne(e => e.ParentArticleComment) //Each comment from Replies points back to its parent
            //        .HasForeignKey(e => e.ParentArticleCommentId);
            //});


        }



        #region CleanString
        // این بخش برای یکپارچه سازی کاراکتر ها می باشد به صورتی که اگر کاربری 
        // ی عربی وارد کرد تبدیل به ی فارسی و ....

        public override int SaveChanges()
        {
            _cleanString();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _cleanString();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            _cleanString();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _cleanString();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void _cleanString()
        {
            var changedEntities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity == null)
                    continue;

                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                foreach (var property in properties)
                {
                    var propName = property.Name;
                    var val = (string)property.GetValue(item.Entity, null);

                    if (val.HasValue())
                    {
                        var newVal = val.Fa2En().FixPersianChars();
                        if (newVal == val)
                            continue;
                        property.SetValue(item.Entity, newVal, null);
                    }
                }
            }
        }

        #endregion
    }
}

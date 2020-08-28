using AutoMapper;
using DataLayer;
using DataLayer.CustomMapping;
using DataLayer.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.BaseClasses
{
    public abstract class BaseMapping<TDto, TEntity, TKey> : IMapping
        where TDto : class, new()
        where TEntity : BaseEntity<TKey>, new()
    {
        [Display(Name = "ردیف")]
        public TKey Id { get; set; }

        public TEntity ToEntity()
        {
            return Mapper.Map<TEntity>(CastToDerivedClass(this));
        }

        public TEntity ToEntity(TEntity entity)
        {
            return Mapper.Map(CastToDerivedClass(this), entity);
        }

        public static TDto FromEntity(TEntity model)
        {
            return Mapper.Map<TDto>(model);
        }

        protected TDto CastToDerivedClass(BaseMapping<TDto, TEntity, TKey> baseInstance)
        {
            return Mapper.Map<TDto>(baseInstance);
        }

        public void CreateMappings(Profile profile)
        {
            var mappingExpression = profile.CreateMap<TDto, TEntity>();

            var dtoType = typeof(TDto);
            var entityType = typeof(TEntity);
            foreach (var property in entityType.GetProperties())
            {
                if (dtoType.GetProperty(property.Name) == null)
                    mappingExpression.ForMember(property.Name, opt => opt.Ignore());
            }

            CustomMappings(mappingExpression.ReverseMap());
        }

        public virtual void CustomMappings(IMappingExpression<TEntity, TDto> mapping)
        {
        }
    }


    public abstract class BaseMapping<TDto, TEntity> : IMapping
     where TDto : class, new()
     where TEntity : BaseEntity, new()
    {

        public TEntity ToEntity()
        {
            return Mapper.Map<TEntity>(CastToDerivedClass(this));
        }

        public TEntity ToEntity(TEntity entity)
        {
            return Mapper.Map(CastToDerivedClass(this), entity);
        }

        public static TDto FromEntity(TEntity model)
        {
            return Mapper.Map<TDto>(model);
        }

        protected TDto CastToDerivedClass(BaseMapping<TDto, TEntity> baseInstance)
        {
            return Mapper.Map<TDto>(baseInstance);
        }

        public void CreateMappings(Profile profile)
        {
            var mappingExpression = profile.CreateMap<TDto, TEntity>();

            var dtoType = typeof(TDto);
            var entityType = typeof(TEntity);
            foreach (var property in entityType.GetProperties())
            {
                if (dtoType.GetProperty(property.Name) == null)
                    mappingExpression.ForMember(property.Name, opt => opt.Ignore());
            }

            CustomMappings(mappingExpression.ReverseMap());
        }

        public virtual void CustomMappings(IMappingExpression<TEntity, TDto> mapping)
        {
        }
    }


}

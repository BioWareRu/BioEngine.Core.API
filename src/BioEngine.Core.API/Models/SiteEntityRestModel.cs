using System.Threading.Tasks;
using BioEngine.Core.Interfaces;

namespace BioEngine.Core.API.Models
{
    public abstract class SiteEntityRestModel<TEntity, TEntityPk> : RestModel<TEntity, TEntityPk>
        where TEntity : class, ISiteEntity<TEntityPk>, IEntity<TEntityPk>
    {
        public int[] SiteIds { get; set; }

        protected override async Task ParseEntityAsync(TEntity entity)
        {
            await base.ParseEntityAsync(entity);
            SiteIds = entity.SiteIds;
        }

        protected override async Task<TEntity> FillEntityAsync(TEntity entity)
        {
            entity = await base.FillEntityAsync(entity);
            entity.SiteIds = SiteIds;
            return entity;
        }
    }

    public abstract class SingleSiteEntityRestModel<TEntity, TEntityPk> : RestModel<TEntity, TEntityPk>
        where TEntity : class, ISingleSiteEntity<TEntityPk>, IEntity<TEntityPk>
    {
        public int SiteId { get; set; }

        protected override async Task ParseEntityAsync(TEntity entity)
        {
            await base.ParseEntityAsync(entity);
            SiteId = entity.SiteId;
        }

        protected override async Task<TEntity> FillEntityAsync(TEntity entity)
        {
            entity = await base.FillEntityAsync(entity);
            entity.SiteId = SiteId;
            return entity;
        }
    }
}
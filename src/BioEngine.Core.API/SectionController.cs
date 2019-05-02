﻿using System.IO;
using System.Threading.Tasks;
using BioEngine.Core.API.Models;
using BioEngine.Core.DB;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.API
{
    public abstract class
        SectionController<TEntity, TData, TResponse, TRequest> : ContentEntityController<TEntity,
            TResponse
            , TRequest>
        where TEntity : Section<TData>, IEntity
        where TData : TypedData, new()
        where TResponse : class, IContentResponseRestModel<TEntity>
        where TRequest : SectionRestModel<TEntity>, IContentRequestRestModel<TEntity>
    {
        protected SectionController(BaseControllerContext<TEntity> context, BioEntityMetadataManager metadataManager,
            ContentBlocksRepository blocksRepository) : base(context, metadataManager, blocksRepository)
        {
        }


        public override async Task<ActionResult<StorageItem>> UploadAsync([FromQuery] string name)
        {
            var file = await GetBodyAsFileAsync();
            return await Storage.SaveFileAsync(file, name, Path.Combine("sections", GetUploadPath()));
        }

        protected abstract string GetUploadPath();
    }

    public abstract class
        SectionController<TEntity, TResponse> : ResponseRestController<TEntity, TResponse>
        where TEntity : Section, IEntity where TResponse : IResponseRestModel<TEntity>
    {
        protected SectionController(BaseControllerContext<TEntity> context) : base(context)
        {
        }
    }
}

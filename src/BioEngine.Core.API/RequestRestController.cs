﻿using System;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Core.Abstractions;
using BioEngine.Core.API.Models;
using BioEngine.Core.API.Response;
using BioEngine.Core.Entities;
using BioEngine.Core.Repository;
using BioEngine.Core.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Core.API
{
    public abstract class
        RequestRestController<TEntity, TRepository, TResponse, TRequest> : ResponseRestController<TEntity
            , TRepository,
            TResponse>
        where TEntity : class, IEntity
        where TResponse : class, IResponseRestModel<TEntity>
        where TRequest : class, IRequestRestModel<TEntity>
        where TRepository : IBioRepository<TEntity>
    {
        protected RequestRestController(BaseControllerContext<TEntity, TRepository> context) :
            base(context)
        {
        }

        protected virtual async Task<TEntity> MapDomainModelAsync(TRequest restModel,
            TEntity domainModel = null)
        {
            return await restModel.GetEntityAsync(domainModel);
        }


        [HttpPost]
        public virtual async Task<ActionResult<TResponse>> AddAsync(TRequest item)
        {
            var entity = await MapDomainModelAsync(item, Activator.CreateInstance<TEntity>());

            var result = await Repository.AddAsync(entity, new BioRepositoryOperationContext {User = CurrentUser});
            if (result.IsSuccess)
            {
                await AfterSaveAsync(result.Entity, result.Changes, item);
                return Created(await MapRestModelAsync(result.Entity));
            }

            return Errors(StatusCodes.Status422UnprocessableEntity,
                result.Errors.Select(e => new ValidationErrorResponse(e.PropertyName, e.ErrorMessage)));
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult<TResponse>> UpdateAsync(Guid id, TRequest item)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity = await MapDomainModelAsync(item, entity);

            var result = await Repository.UpdateAsync(entity, new BioRepositoryOperationContext {User = CurrentUser});
            if (result.IsSuccess)
            {
                await AfterSaveAsync(result.Entity, result.Changes, item);
                return Updated(await MapRestModelAsync(result.Entity));
            }

            return Errors(StatusCodes.Status422UnprocessableEntity,
                result.Errors.Select(e => new ValidationErrorResponse(e.PropertyName, e.ErrorMessage)));
        }

        [HttpPost("upload")]
        public virtual Task<ActionResult<StorageItem>> UploadAsync([FromQuery] string name)
        {
            throw new NotImplementedException();
        }

        //protected abstract TRest MapEntity(TRest entity, TRest newData);

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<TResponse>> DeleteAsync(Guid id)
        {
            var result = await Repository.DeleteAsync(id, new BioRepositoryOperationContext {User = CurrentUser});
            if (result != null)
            {
                await AfterDeleteAsync(result);
                return Deleted();
            }

            return BadRequest();
        }


        protected ActionResult<TResponse> Created(
            TResponse model)
        {
            return SaveResponse(StatusCodes.Status201Created, model);
        }

        protected ActionResult<TResponse> Updated(
            TResponse model)
        {
            return SaveResponse(StatusCodes.Status202Accepted, model);
        }

        protected ActionResult<TResponse> Deleted()
        {
            return SaveResponse(StatusCodes.Status204NoContent, null);
        }


        private ActionResult<TResponse> SaveResponse(int code,
            TResponse model)
        {
            return new ObjectResult(new SaveModelResponse<TResponse>(code, model)) {StatusCode = code};
        }

        protected virtual Task AfterSaveAsync(TEntity domainModel, PropertyChange[] changes = null,
            TRequest request = null)
        {
            return Task.CompletedTask;
        }

        protected virtual Task AfterDeleteAsync(TEntity domainModel)
        {
            return Task.CompletedTask;
        }
    }

    public abstract class
        ResponseRequestRestController<TEntity, TRepository, TRequestResponse> :
            RequestRestController<TEntity, TRepository, TRequestResponse, TRequestResponse>
        where TEntity : class, IEntity
        where TRequestResponse : class, IResponseRestModel<TEntity>, IRequestRestModel<TEntity>
        where TRepository : IBioRepository<TEntity>
    {
        protected ResponseRequestRestController(BaseControllerContext<TEntity, TRepository> context) :
            base(context)
        {
        }
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Paradise.Models;
using Paradise.WebApi.Infrastructure.Binders.Base;

namespace Paradise.WebApi.Infrastructure.Binders;

/// <summary>
/// The <see cref="IModelBinder"/> implementation for <see cref="PagedListQueryModel"/> instances.
/// </summary>
internal sealed class PagedListQueryModelBinder : ModelBinderBase<PagedListQueryModel>
{
    #region Private protected methods
    /// <inheritdoc/>
    private protected override PagedListQueryModel GetInstance(ModelBindingContext bindingContext)
    {
        var pageSize = GetInt(bindingContext, nameof(PagedListQueryModel.PageSize));
        var pageNumber = GetInt(bindingContext, nameof(PagedListQueryModel.PageNumber));
        var orderAscending = GetBool(bindingContext, nameof(PagedListQueryModel.OrderAscending));
        var orderBy = GetString(bindingContext, nameof(PagedListQueryModel.OrderBy));
        var lookupProperties = GetStrings(bindingContext, nameof(PagedListQueryModel.LookupProperties));
        var lookupValue = GetString(bindingContext, nameof(PagedListQueryModel.LookupValue));

        if (pageSize is 0)
            pageSize = 1;

        if (pageNumber is 0)
            pageNumber = 1;

        return new(pageSize, pageNumber, orderAscending, orderBy, lookupProperties, lookupValue);
    }
    #endregion
}
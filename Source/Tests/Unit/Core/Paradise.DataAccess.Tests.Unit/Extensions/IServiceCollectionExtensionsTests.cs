using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Paradise.DataAccess.Database.Interceptors;
using Paradise.DataAccess.Extensions;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.DataAccess.Repositories.Attributes;
using Paradise.DataAccess.Repositories.Domain.Identity.Users;
using Paradise.Tests.Miscellaneous;

namespace Paradise.DataAccess.Tests.Unit.Extensions;

/// <summary>
/// <see cref="IServiceCollectionExtensions"/> test class.
/// </summary>
public sealed partial class IServiceCollectionExtensionsTests
{
    #region Public methods
    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddDataAccess"/> method should
    /// register public API interfaces as scoped services.
    /// </summary>
    [Fact]
    public void AddDataAccess()
    {
        // Arrange
        var provider = Test.BuildDataAccessServiceProvider();

        // Act & Assert
        Assert.ServiceLifetime<IInfrastructureUnitOfWork>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IDomainUnitOfWork>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IEmailTemplatesRepository>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IUserRefreshTokensRepository>(provider, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddDataAccess"/> method should
    /// register two different keyed database contexts.
    /// </summary>
    [Fact]
    public void AddDataAccess_ProvidesTwoContexts()
    {
        // Arrange
        var provider = Test.BuildDataAccessServiceProvider();

        using var scope = provider.CreateScope();

        var scopedProvider = scope.ServiceProvider;

        // Act
        var infrastructureDataSource = scopedProvider
            .GetRequiredKeyedService<IDataSource>(InfrastructureContextKeyAttribute.ContextKey);

        var domainDataSource = scopedProvider
            .GetRequiredKeyedService<IDataSource>(DomainContextKeyAttribute.ContextKey);

        // Assert
        Assert.NotSame(infrastructureDataSource, domainDataSource);
    }

    /// <summary>
    /// The <see cref="IServiceCollectionExtensions.AddDataAccess"/> method should
    /// register singleton database context interceptors.
    /// </summary>
    [Fact]
    public void AddDataAccess_ProvidesInterceptors()
    {
        // Arrange
        var provider = Test.BuildDataAccessServiceProvider();

        // Act
        var interceptors = provider.GetServices<IInterceptor>();

        // Assert
        Assert.Contains(interceptors, interceptor => interceptor is OnCreatedInterceptor);
        Assert.Contains(interceptors, interceptor => interceptor is OnModifiedInterceptor);
    }
    #endregion
}
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Paradise.DataAccess.Database.Interceptors;
using Paradise.DataAccess.Extensions;
using Paradise.DataAccess.Repositories;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.DataAccess.Repositories.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Tests.Extensibility;

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
        Assert.ServiceLifetime<IUnitOfWork>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IEmailTemplatesRepository>(provider, ServiceLifetime.Scoped);
        Assert.ServiceLifetime<IUserRefreshTokensRepository>(provider, ServiceLifetime.Scoped);
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
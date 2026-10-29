using Microsoft.Extensions.DependencyInjection;
using Paradise.Models;
using Paradise.Tests.Surrogates.Web.WebApi.Base;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Lifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime;

namespace Paradise.Tests.Extensibility;

/// <summary>
/// Contains extension methods for the <see cref="Assert"/> <see langword="class"/>.
/// </summary>
[SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "False positive on extension members.")]
public static class AssertExtensions
{
    #region Constants
    private const string UnexpectedServiceLifetimeMessage = "Unexpected service lifetime value.";
    #endregion

    extension(Assert)
    {
        #region Public methods
        /// <summary>
        /// Verifies that a service of type <typeparamref name="T"/>
        /// is registered with the specified <paramref name="lifetime"/>
        /// in the given <paramref name="rootServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The service type to verify.
        /// </typeparam>
        /// <param name="rootServiceProvider">
        /// The root <see cref="IServiceProvider"/> containing the service registration.
        /// </param>
        /// <param name="lifetime">
        /// The expected <see cref="Lifetime"/> of the service.
        /// </param>
        /// <param name="assertions">
        /// Additional assertions to perform on the resolved instance.
        /// </param>
        /// <remarks>
        /// This method resolves the service and asserts instance identity according to its lifetime:
        /// <list type="bullet">
        /// <item>
        /// <see cref="Lifetime.Singleton"/>: resolving multiple times returns the same instance.
        /// </item>
        /// <item>
        /// <see cref="Lifetime.Scoped"/>: resolving within the same scope returns the same instance;
        /// across scopes returns different instances.
        /// </item>
        /// <item>
        /// <see cref="Lifetime.Transient"/>: each resolution returns a new instance.
        /// </item>
        /// </list>
        /// </remarks>
        public static void ServiceLifetime<T>(IServiceProvider rootServiceProvider,
                                          Lifetime lifetime,
                                          Action<T>? assertions = null)
        where T : class
        {
            switch (lifetime)
            {
                case Lifetime.Singleton:
                    AssertSingleton(rootServiceProvider, assertions);
                    break;
                case Lifetime.Scoped:
                    AssertScoped(rootServiceProvider, assertions);
                    break;
                case Lifetime.Transient:
                    AssertTransient(rootServiceProvider, assertions);
                    break;
                default:
                    Assert.Fail(UnexpectedServiceLifetimeMessage);
                    break;
            }
        }

        /// <summary>
        /// Verifies that a service of type <typeparamref name="T"/>
        /// is registered with the specified <paramref name="lifetime"/>
        /// using the specified <paramref name="serviceKey"/>
        /// in the given <paramref name="rootServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The service type to verify.
        /// </typeparam>
        /// <param name="rootServiceProvider">
        /// The root <see cref="IServiceProvider"/> containing the service registration.
        /// </param>
        /// <param name="lifetime">
        /// The expected <see cref="Lifetime"/> of the service.
        /// </param>
        /// <param name="serviceKey">
        /// An object that specifies the key of service object to get.
        /// </param>
        /// <param name="assertions">
        /// Additional assertions to perform on the resolved instance.
        /// </param>
        /// <remarks>
        /// This method resolves the service and asserts instance identity according to its lifetime:
        /// <list type="bullet">
        /// <item>
        /// <see cref="Lifetime.Singleton"/>: resolving multiple times returns the same instance.
        /// </item>
        /// <item>
        /// <see cref="Lifetime.Scoped"/>: resolving within the same scope returns the same instance;
        /// across scopes returns different instances.
        /// </item>
        /// <item>
        /// <see cref="Lifetime.Transient"/>: each resolution returns a new instance.
        /// </item>
        /// </list>
        /// </remarks>
        public static void ServiceLifetimeKeyed<T>(IServiceProvider rootServiceProvider,
                                                   Lifetime lifetime,
                                                   object? serviceKey,
                                                   Action<T>? assertions = null)
            where T : class
        {
            switch (lifetime)
            {
                case Lifetime.Singleton:
                    AssertSingleton(rootServiceProvider, serviceKey, assertions);
                    break;
                case Lifetime.Scoped:
                    AssertScoped(rootServiceProvider, serviceKey, assertions);
                    break;
                case Lifetime.Transient:
                    AssertTransient(rootServiceProvider, serviceKey, assertions);
                    break;
                default:
                    Assert.Fail(UnexpectedServiceLifetimeMessage);
                    break;
            }
        }

        /// <summary>
        /// Verifies that all services of type <typeparamref name="T"/>
        /// registered in the <paramref name="rootServiceProvider"/>
        /// behave according to the specified <paramref name="lifetime"/>
        /// when resolved as <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The service type to verify.
        /// </typeparam>
        /// <param name="rootServiceProvider">
        /// The root <see cref="IServiceProvider"/> containing the service registrations.
        /// </param>
        /// <param name="lifetime">
        /// The expected <see cref="Lifetime"/> of the services.
        /// </param>
        /// <param name="assertions">
        /// Additional assertions to perform on the resolved instances.
        /// </param>
        /// <remarks>
        /// This method resolves all registrations of <typeparamref name="T"/>
        /// as an <see cref="IEnumerable{T}"/> and asserts instance identity according to the lifetime:
        /// <list type="bullet">
        /// <item>
        /// <see cref="Lifetime.Singleton"/>: all resolved instances are the same across multiple resolutions.
        /// </item>
        /// <item>
        /// <see cref="Lifetime.Scoped"/>: instances are the same within a scope, different across scopes.
        /// </item>
        /// <item>
        /// <see cref="Lifetime.Transient"/>: each resolution produces new instances.
        /// </item>
        /// </list>
        /// This method assumes that registration order remains consistent for identity comparisons.
        /// </remarks>
        public static void ServiceLifetimeEnumerable<T>(IServiceProvider rootServiceProvider,
                                                        Lifetime lifetime,
                                                        Action<IEnumerable<T>>? assertions = null)
            where T : class
        {
            switch (lifetime)
            {
                case Lifetime.Singleton:
                    AssertSingleton(rootServiceProvider, assertions);
                    break;
                case Lifetime.Scoped:
                    AssertScoped(rootServiceProvider, assertions);
                    break;
                case Lifetime.Transient:
                    AssertTransient(rootServiceProvider, assertions);
                    break;
                default:
                    Assert.Fail(UnexpectedServiceLifetimeMessage);
                    break;
            }
        }

        /// <summary>
        /// Verifies that all services of type <typeparamref name="T"/>
        /// registered in the <paramref name="rootServiceProvider"/>
        /// behave according to the specified <paramref name="lifetime"/>
        /// when resolved as keyed <see cref="IEnumerable{T}"/>
        /// using the specified <paramref name="serviceKey"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The service type to verify.
        /// </typeparam>
        /// <param name="rootServiceProvider">
        /// The root <see cref="IServiceProvider"/> containing the service registrations.
        /// </param>
        /// <param name="lifetime">
        /// The expected <see cref="Lifetime"/> of the services.
        /// </param>
        /// <param name="serviceKey">
        /// An object that specifies the key of service object to get.
        /// </param>
        /// <param name="assertions">
        /// Additional assertions to perform on the resolved instances.
        /// </param>
        /// <remarks>
        /// This method resolves all registrations of <typeparamref name="T"/>
        /// as an <see cref="IEnumerable{T}"/> and asserts instance identity according to the lifetime:
        /// <list type="bullet">
        /// <item>
        /// <see cref="Lifetime.Singleton"/>: all resolved instances are the same across multiple resolutions.
        /// </item>
        /// <item>
        /// <see cref="Lifetime.Scoped"/>: instances are the same within a scope, different across scopes.
        /// </item>
        /// <item>
        /// <see cref="Lifetime.Transient"/>: each resolution produces new instances.
        /// </item>
        /// </list>
        /// This method assumes that registration order remains consistent for identity comparisons.
        /// </remarks>
        public static void ServiceLifetimeEnumerableKeyed<T>(IServiceProvider rootServiceProvider,
                                                             Lifetime lifetime,
                                                             object? serviceKey,
                                                             Action<IEnumerable<T>>? assertions = null)
            where T : class
        {
            switch (lifetime)
            {
                case Lifetime.Singleton:
                    AssertSingleton(rootServiceProvider, serviceKey, assertions);
                    break;
                case Lifetime.Scoped:
                    AssertScoped(rootServiceProvider, serviceKey, assertions);
                    break;
                case Lifetime.Transient:
                    AssertTransient(rootServiceProvider, serviceKey, assertions);
                    break;
                default:
                    Assert.Fail(UnexpectedServiceLifetimeMessage);
                    break;
            }
        }

        /// <summary>
        /// Verifies that the given <paramref name="errors"/> collection contains
        /// the specified <paramref name="code"/>.
        /// </summary>
        /// <param name="errors">
        /// The errors collection to look up the error code.
        /// </param>
        /// <param name="code">
        /// The <see cref="ErrorCode"/> value expected to be present.
        /// </param>
        /// <param name="descriptionSubString">
        /// The <see cref="string"/> value expected to be contained within expected error's description.
        /// </param>
        public static void ContainsError(IEnumerable<ApplicationError> errors, ErrorCode code, string? descriptionSubString = null)
        {
            ArgumentNullException.ThrowIfNull(errors);

            var filteredErrors = errors
                .Where(error => error.Code == code)
                .ToList();

            if (filteredErrors.Count is 0)
            {
                var details = "No matching error code was found in collection.";
                var expected = $"Expected: \"{code}\"";
                var actual = $"Actual:   \"{string.Join(", ", errors.Select(error => error.Code))}\"";

                var message = string.Join(Environment.NewLine, details, expected, actual);
                Assert.Fail(message);
            }

            if (descriptionSubString is not null)
            {
                var containsExpectedDescription = filteredErrors
                    .Any(error => error.Description.Contains(descriptionSubString, StringComparison.Ordinal));

                if (!containsExpectedDescription)
                {
                    var details = "None of the matched errors contains the specified description sub-string.";
                    var expected = $"Expected: \"{descriptionSubString}\"";
                    var actual = $"Actual:   \"{string.Join(", ", filteredErrors.Select(error => error.Description))}\"";

                    var message = string.Join(Environment.NewLine, details, expected, actual);

                    Assert.Fail(message);
                }
            }
        }

        /// <summary>
        /// Verifies that the given <paramref name="response"/> is a problem details response
        /// with the specified <paramref name="statusCode"/>, containing the specified <paramref name="code"/>.
        /// </summary>
        /// <param name="response">
        /// The <see cref="HttpResponseMessage"/> to be checked.
        /// </param>
        /// <param name="statusCode">
        /// The expected <see cref="HttpStatusCode"/>.
        /// </param>
        /// <param name="code">
        /// The <see cref="ErrorCode"/> value expected to be present.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe
        /// while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public static Task ProblemAsync(HttpResponseMessage response, HttpStatusCode statusCode, ErrorCode code,
                                        CancellationToken cancellationToken = default)
            => Assert.ProblemAsync(response, statusCode, [code], cancellationToken);

        /// <summary>
        /// Verifies that the given <paramref name="response"/> is a problem details response
        /// with the specified <paramref name="statusCode"/>, containing each of the specified <paramref name="codes"/>.
        /// </summary>
        /// <param name="response">
        /// The <see cref="HttpResponseMessage"/> to be checked.
        /// </param>
        /// <param name="statusCode">
        /// The expected <see cref="HttpStatusCode"/>.
        /// </param>
        /// <param name="codes">
        /// The <see cref="ErrorCode"/> values expected to be present.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe
        /// while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        public static async Task ProblemAsync(HttpResponseMessage response, HttpStatusCode statusCode, IEnumerable<ErrorCode> codes,
                                              CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(codes);

            var problemDetails = await Assert.BodyAsync<TestProblemDetailsMetadata>(response, statusCode, cancellationToken)
                .ConfigureAwait(false);

            foreach (var code in codes)
                Assert.ContainsError(problemDetails.Errors, code);
        }

        /// <summary>
        /// Verifies that the given <paramref name="response"/> has the specified <paramref name="statusCode"/>
        /// and its body can be deserialized into an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type to deserialize the response body into.
        /// </typeparam>
        /// <param name="response">
        /// The <see cref="HttpResponseMessage"/> to be checked.
        /// </param>
        /// <param name="statusCode">
        /// The expected <see cref="HttpStatusCode"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> to observe
        /// while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the deserialized response body.
        /// </returns>
        public static async Task<T> BodyAsync<T>(HttpResponseMessage response, HttpStatusCode statusCode,
                                                 CancellationToken cancellationToken = default)
        {
            // The status code is checked first, since unexpected responses
            // may not contain a body that can be deserialized.
            Assert.StatusCode(response, statusCode);

            var body = await response.Content.ReadFromJsonAsync<T>(cancellationToken)
                .ConfigureAwait(false);

            Assert.NotNull(body);

            return body;
        }

        /// <summary>
        /// Verifies that the given <paramref name="response"/>
        /// has the specified <paramref name="statusCode"/>.
        /// </summary>
        /// <param name="response">
        /// The <see cref="HttpResponseMessage"/> to be checked.
        /// </param>
        /// <param name="statusCode">
        /// The expected <see cref="HttpStatusCode"/>.
        /// </param>
        public static void StatusCode(HttpResponseMessage response, HttpStatusCode statusCode)
        {
            ArgumentNullException.ThrowIfNull(response);

            Assert.Equal(statusCode, response.StatusCode);
        }
        #endregion
    }

    #region Private methods
    /// <summary>
    /// Verifies that resolving <typeparamref name="T"/>
    /// multiple times from the same root <paramref name="provider"/>
    /// returns the same instance.
    /// </summary>
    /// <typeparam name="T">
    /// The service type to verify.
    /// </typeparam>
    /// <param name="provider">
    /// The <see cref="IServiceProvider"/> used to resolve the service.
    /// </param>
    /// <param name="assertions">
    /// Additional assertions to perform on the resolved instance.
    /// </param>
    private static void AssertSingleton<T>(IServiceProvider provider, Action<T>? assertions = null)
        where T : class
    {
        var first = provider.GetRequiredService<T>();
        var second = provider.GetRequiredService<T>();

        Assert.Same(first, second);
        assertions?.Invoke(first);
    }

    /// <summary>
    /// Verifies that resolving keyed instances of <typeparamref name="T"/>
    /// multiple times from the same root <paramref name="provider"/>
    /// returns the same instance.
    /// </summary>
    /// <typeparam name="T">
    /// The service type to verify.
    /// </typeparam>
    /// <param name="provider">
    /// The <see cref="IServiceProvider"/> used to resolve the service.
    /// </param>
    /// <param name="serviceKey">
    /// The key associated with the service registration.
    /// </param>
    /// <param name="assertions">
    /// Additional assertions to perform on the resolved instance.
    /// </param>
    private static void AssertSingleton<T>(IServiceProvider provider, object? serviceKey, Action<T>? assertions = null)
        where T : class
    {
        var first = provider.GetRequiredKeyedService<T>(serviceKey);
        var second = provider.GetRequiredKeyedService<T>(serviceKey);

        Assert.Same(first, second);
        assertions?.Invoke(first);
    }

    /// <summary>
    /// Verifies that all resolved instances of <typeparamref name="T"/>
    /// remain identical across multiple resolutions from the same root
    /// <paramref name="provider"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The service type to verify.
    /// </typeparam>
    /// <param name="provider">
    /// The <see cref="IServiceProvider"/> used to resolve the services.
    /// </param>
    /// <param name="assertions">
    /// Additional assertions to perform on the resolved instances.
    /// </param>
    private static void AssertSingleton<T>(IServiceProvider provider, Action<IEnumerable<T>>? assertions = null)
    {
        var firstServices = provider.GetServices<T>().ToArray();
        var secondServices = provider.GetServices<T>().ToArray();

        Assert.Equal(firstServices.Length, secondServices.Length);

        for (var i = 0; i < firstServices.Length; i++)
            Assert.Same(firstServices[i], secondServices[i]);

        assertions?.Invoke(firstServices);
    }

    /// <summary>
    /// Verifies that all keyed instances of <typeparamref name="T"/>
    /// remain identical across multiple resolutions from the same root
    /// <paramref name="provider"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The service type to verify.
    /// </typeparam>
    /// <param name="provider">
    /// The <see cref="IServiceProvider"/> used to resolve the services.
    /// </param>
    /// <param name="serviceKey">
    /// The key associated with the service registration.
    /// </param>
    /// <param name="assertions">
    /// Additional assertions to perform on the resolved instances.
    /// </param>
    private static void AssertSingleton<T>(IServiceProvider provider, object? serviceKey, Action<IEnumerable<T>>? assertions = null)
    {
        var firstServices = provider.GetKeyedServices<T>(serviceKey).ToArray();
        var secondServices = provider.GetKeyedServices<T>(serviceKey).ToArray();

        Assert.Equal(firstServices.Length, secondServices.Length);

        for (var i = 0; i < firstServices.Length; i++)
            Assert.Same(firstServices[i], secondServices[i]);

        assertions?.Invoke(firstServices);
    }

    /// <summary>
    /// Verifies that resolving <typeparamref name="T"/>
    /// within the same scope returns the same instance,
    /// while resolutions across different scopes
    /// return different instances.
    /// </summary>
    /// <typeparam name="T">
    /// The service type to verify.
    /// </typeparam>
    /// <param name="provider">
    /// The <see cref="IServiceProvider"/> used to create scopes and resolve the service.
    /// </param>
    /// <param name="assertions">
    /// Additional assertions to perform on the resolved instance.
    /// </param>
    private static void AssertScoped<T>(IServiceProvider provider, Action<T>? assertions = null)
        where T : class
    {
        using var firstScope = provider.CreateScope();
        using var secondScope = provider.CreateScope();

        var first = firstScope.ServiceProvider.GetRequiredService<T>();
        var firstMirror = firstScope.ServiceProvider.GetRequiredService<T>();
        var second = secondScope.ServiceProvider.GetRequiredService<T>();

        Assert.Same(first, firstMirror);
        Assert.NotSame(first, second);
        assertions?.Invoke(first);
    }

    /// <summary>
    /// Verifies that resolving keyed instances of <typeparamref name="T"/>
    /// within the same scope returns the same instance,
    /// while resolutions across different scopes
    /// return different instances.
    /// </summary>
    /// <typeparam name="T">
    /// The service type to verify.
    /// </typeparam>
    /// <param name="provider">
    /// The <see cref="IServiceProvider"/> used to create scopes and resolve the service.
    /// </param>
    /// <param name="serviceKey">
    /// The key associated with the service registration.
    /// </param>
    /// <param name="assertions">
    /// Additional assertions to perform on the resolved instance.
    /// </param>
    private static void AssertScoped<T>(IServiceProvider provider, object? serviceKey, Action<T>? assertions = null)
        where T : class
    {
        using var firstScope = provider.CreateScope();
        using var secondScope = provider.CreateScope();

        var first = firstScope.ServiceProvider.GetRequiredKeyedService<T>(serviceKey);
        var firstMirror = firstScope.ServiceProvider.GetRequiredKeyedService<T>(serviceKey);
        var second = secondScope.ServiceProvider.GetRequiredKeyedService<T>(serviceKey);

        Assert.Same(first, firstMirror);
        Assert.NotSame(first, second);
        assertions?.Invoke(first);
    }

    /// <summary>
    /// Verifies that all resolved instances of <typeparamref name="T"/>
    /// remain identical within the same scope
    /// and differ across separate scopes.
    /// </summary>
    /// <typeparam name="T">
    /// The service type to verify.
    /// </typeparam>
    /// <param name="provider">
    /// The <see cref="IServiceProvider"/> used to create scopes and resolve the services.
    /// </param>
    /// <param name="assertions">
    /// Additional assertions to perform on the resolved instances.
    /// </param>
    private static void AssertScoped<T>(IServiceProvider provider, Action<IEnumerable<T>>? assertions = null)
    {
        using var firstScope = provider.CreateScope();
        using var secondScope = provider.CreateScope();

        var firstServices = firstScope.ServiceProvider.GetServices<T>().ToArray();
        var firstServicesMirror = firstScope.ServiceProvider.GetServices<T>().ToArray();
        var secondServices = secondScope.ServiceProvider.GetServices<T>().ToArray();

        Assert.Equal(firstServices.Length, firstServicesMirror.Length);
        Assert.Equal(firstServices.Length, secondServices.Length);

        for (var i = 0; i < firstServices.Length; i++)
        {
            Assert.Same(firstServices[i], firstServicesMirror[i]);
            Assert.NotSame(firstServices[i], secondServices[i]);
        }

        assertions?.Invoke(firstServices);
    }

    /// <summary>
    /// Verifies that all keyed instances of <typeparamref name="T"/>
    /// remain identical within the same scope
    /// and differ across separate scopes.
    /// </summary>
    /// <typeparam name="T">
    /// The service type to verify.
    /// </typeparam>
    /// <param name="provider">
    /// The <see cref="IServiceProvider"/> used to create scopes and resolve the services.
    /// </param>
    /// <param name="serviceKey">
    /// The key associated with the service registration.
    /// </param>
    /// <param name="assertions">
    /// Additional assertions to perform on the resolved instances.
    /// </param>
    private static void AssertScoped<T>(IServiceProvider provider, object? serviceKey, Action<IEnumerable<T>>? assertions = null)
    {
        using var firstScope = provider.CreateScope();
        using var secondScope = provider.CreateScope();

        var firstServices = firstScope.ServiceProvider.GetKeyedServices<T>(serviceKey).ToArray();
        var firstServicesMirror = firstScope.ServiceProvider.GetKeyedServices<T>(serviceKey).ToArray();
        var secondServices = secondScope.ServiceProvider.GetKeyedServices<T>(serviceKey).ToArray();

        Assert.Equal(firstServices.Length, firstServicesMirror.Length);
        Assert.Equal(firstServices.Length, secondServices.Length);

        for (var i = 0; i < firstServices.Length; i++)
        {
            Assert.Same(firstServices[i], firstServicesMirror[i]);
            Assert.NotSame(firstServices[i], secondServices[i]);
        }

        assertions?.Invoke(firstServices);
    }

    /// <summary>
    /// Verifies that each resolution of <typeparamref name="T"/>
    /// from the specified <paramref name="provider"/>
    /// produces a different instance.
    /// </summary>
    /// <typeparam name="T">
    /// The service type to verify.
    /// </typeparam>
    /// <param name="provider">
    /// The <see cref="IServiceProvider"/> used to resolve the service.
    /// </param>
    /// <param name="assertions">
    /// Additional assertions to perform on the resolved instance.
    /// </param>
    private static void AssertTransient<T>(IServiceProvider provider, Action<T>? assertions = null)
        where T : class
    {
        var first = provider.GetRequiredService<T>();
        var second = provider.GetRequiredService<T>();

        Assert.NotSame(first, second);
        assertions?.Invoke(first);
    }

    /// <summary>
    /// Verifies that each keyed resolution of <typeparamref name="T"/>
    /// from the specified <paramref name="provider"/>
    /// produces a different instance.
    /// </summary>
    /// <typeparam name="T">
    /// The service type to verify.
    /// </typeparam>
    /// <param name="provider">
    /// The <see cref="IServiceProvider"/> used to resolve the service.
    /// </param>
    /// <param name="serviceKey">
    /// The key associated with the service registration.
    /// </param>
    /// <param name="assertions">
    /// Additional assertions to perform on the resolved instance.
    /// </param>
    private static void AssertTransient<T>(IServiceProvider provider, object? serviceKey, Action<T>? assertions = null)
        where T : class
    {
        var first = provider.GetRequiredKeyedService<T>(serviceKey);
        var second = provider.GetRequiredKeyedService<T>(serviceKey);

        Assert.NotSame(first, second);
        assertions?.Invoke(first);
    }

    /// <summary>
    /// Verifies that each resolution of all registered instances
    /// of <typeparamref name="T"/> produces different instances.
    /// </summary>
    /// <typeparam name="T">
    /// The service type to verify.
    /// </typeparam>
    /// <param name="provider">
    /// The <see cref="IServiceProvider"/> used to resolve the services.
    /// </param>
    /// <param name="assertions">
    /// Additional assertions to perform on the resolved instances.
    /// </param>
    private static void AssertTransient<T>(IServiceProvider provider, Action<IEnumerable<T>>? assertions = null)
    {
        var firstServices = provider.GetServices<T>().ToArray();
        var secondServices = provider.GetServices<T>().ToArray();

        Assert.Equal(firstServices.Length, secondServices.Length);

        for (var i = 0; i < firstServices.Length; i++)
            Assert.NotSame(firstServices[i], secondServices[i]);

        assertions?.Invoke(firstServices);
    }

    /// <summary>
    /// Verifies that each keyed resolution of all registered instances
    /// of <typeparamref name="T"/> produces different instances.
    /// </summary>
    /// <typeparam name="T">
    /// The service type to verify.
    /// </typeparam>
    /// <param name="provider">
    /// The <see cref="IServiceProvider"/> used to resolve the services.
    /// </param>
    /// <param name="serviceKey">
    /// The key associated with the service registration.
    /// </param>
    /// <param name="assertions">
    /// Additional assertions to perform on the resolved instances.
    /// </param>
    private static void AssertTransient<T>(IServiceProvider provider, object? serviceKey, Action<IEnumerable<T>>? assertions = null)
    {
        var firstServices = provider.GetKeyedServices<T>(serviceKey).ToArray();
        var secondServices = provider.GetKeyedServices<T>(serviceKey).ToArray();

        Assert.Equal(firstServices.Length, secondServices.Length);

        for (var i = 0; i < firstServices.Length; i++)
            Assert.NotSame(firstServices[i], secondServices[i]);

        assertions?.Invoke(firstServices);
    }
    #endregion
}
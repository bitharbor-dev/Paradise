using Microsoft.Extensions.DependencyInjection;
using Paradise.Models;
using Xunit;
using Lifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime;

namespace Paradise.Tests.Miscellaneous;

/// <summary>
/// Contains extension methods for the <see cref="Assert"/> <see langword="class"/>.
/// </summary>
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
        /// Additional assertions to be made on the target instance.
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
            void AssertSingleton()
            {
                var first = rootServiceProvider.GetRequiredService<T>();
                var second = rootServiceProvider.GetRequiredService<T>();

                Assert.Same(first, second);
                assertions?.Invoke(first);
            }

            void AssertScoped()
            {
                using var firstScope = rootServiceProvider.CreateScope();
                using var secondScope = rootServiceProvider.CreateScope();

                var first = firstScope.ServiceProvider.GetRequiredService<T>();
                var firstMirror = firstScope.ServiceProvider.GetRequiredService<T>();
                var second = secondScope.ServiceProvider.GetRequiredService<T>();

                Assert.Same(first, firstMirror);
                Assert.NotSame(first, second);
                assertions?.Invoke(first);
            }

            void AssertTransient()
            {
                var first = rootServiceProvider.GetRequiredService<T>();
                var second = rootServiceProvider.GetRequiredService<T>();

                Assert.NotSame(first, second);
                assertions?.Invoke(first);
            }

            switch (lifetime)
            {
                case Lifetime.Singleton:
                    AssertSingleton();
                    break;
                case Lifetime.Scoped:
                    AssertScoped();
                    break;
                case Lifetime.Transient:
                    AssertTransient();
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
        /// Additional assertions to be made on the target instance.
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
            void AssertSingleton()
            {
                var first = rootServiceProvider.GetRequiredKeyedService<T>(serviceKey);
                var second = rootServiceProvider.GetRequiredKeyedService<T>(serviceKey);

                Assert.Same(first, second);
                assertions?.Invoke(first);
            }

            void AssertScoped()
            {
                using var firstScope = rootServiceProvider.CreateScope();
                using var secondScope = rootServiceProvider.CreateScope();

                var first = firstScope.ServiceProvider.GetRequiredKeyedService<T>(serviceKey);
                var firstMirror = firstScope.ServiceProvider.GetRequiredKeyedService<T>(serviceKey);
                var second = secondScope.ServiceProvider.GetRequiredKeyedService<T>(serviceKey);

                Assert.Same(first, firstMirror);
                Assert.NotSame(first, second);
                assertions?.Invoke(first);
            }

            void AssertTransient()
            {
                var first = rootServiceProvider.GetRequiredKeyedService<T>(serviceKey);
                var second = rootServiceProvider.GetRequiredKeyedService<T>(serviceKey);

                Assert.NotSame(first, second);
                assertions?.Invoke(first);
            }

            switch (lifetime)
            {
                case Lifetime.Singleton:
                    AssertSingleton();
                    break;
                case Lifetime.Scoped:
                    AssertScoped();
                    break;
                case Lifetime.Transient:
                    AssertTransient();
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
        /// Additional assertions to be made on the target instances.
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
        /// Assumes registration order is consistent for identity comparisons.
        /// </remarks>
        public static void ServiceLifetimeEnumerable<T>(IServiceProvider rootServiceProvider,
                                                        Lifetime lifetime,
                                                        Action<IEnumerable<T>>? assertions = null)
            where T : class
        {
            void AssertSingleton()
            {
                var firstServices = rootServiceProvider.GetServices<T>().ToArray();
                var secondServices = rootServiceProvider.GetServices<T>().ToArray();

                Assert.Equal(firstServices.Length, secondServices.Length);

                for (var i = 0; i < firstServices.Length; i++)
                    Assert.Same(firstServices[i], secondServices[i]);

                assertions?.Invoke(firstServices);
            }

            void AssertScoped()
            {
                using var firstScope = rootServiceProvider.CreateScope();
                using var secondScope = rootServiceProvider.CreateScope();

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

            void AssertTransient()
            {
                var firstServices = rootServiceProvider.GetServices<T>().ToArray();
                var secondServices = rootServiceProvider.GetServices<T>().ToArray();

                Assert.Equal(firstServices.Length, secondServices.Length);

                for (var i = 0; i < firstServices.Length; i++)
                    Assert.NotSame(firstServices[i], secondServices[i]);

                assertions?.Invoke(firstServices);
            }

            switch (lifetime)
            {
                case Lifetime.Singleton:
                    AssertSingleton();
                    break;
                case Lifetime.Scoped:
                    AssertScoped();
                    break;
                case Lifetime.Transient:
                    AssertTransient();
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
        /// Additional assertions to be made on the target instances.
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
        /// Assumes registration order is consistent for identity comparisons.
        /// </remarks>
        public static void ServiceLifetimeEnumerableKeyed<T>(IServiceProvider rootServiceProvider,
                                                             Lifetime lifetime,
                                                             object? serviceKey,
                                                             Action<IEnumerable<T>>? assertions = null)
            where T : class
        {
            void AssertSingleton()
            {
                var firstServices = rootServiceProvider.GetKeyedServices<T>(serviceKey).ToArray();
                var secondServices = rootServiceProvider.GetKeyedServices<T>(serviceKey).ToArray();

                Assert.Equal(firstServices.Length, secondServices.Length);

                for (var i = 0; i < firstServices.Length; i++)
                    Assert.Same(firstServices[i], secondServices[i]);

                assertions?.Invoke(firstServices);
            }

            void AssertScoped()
            {
                using var firstScope = rootServiceProvider.CreateScope();
                using var secondScope = rootServiceProvider.CreateScope();

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

            void AssertTransient()
            {
                var firstServices = rootServiceProvider.GetKeyedServices<T>(serviceKey).ToArray();
                var secondServices = rootServiceProvider.GetKeyedServices<T>(serviceKey).ToArray();

                Assert.Equal(firstServices.Length, secondServices.Length);

                for (var i = 0; i < firstServices.Length; i++)
                    Assert.NotSame(firstServices[i], secondServices[i]);

                assertions?.Invoke(firstServices);
            }

            switch (lifetime)
            {
                case Lifetime.Singleton:
                    AssertSingleton();
                    break;
                case Lifetime.Scoped:
                    AssertScoped();
                    break;
                case Lifetime.Transient:
                    AssertTransient();
                    break;
                default:
                    Assert.Fail(UnexpectedServiceLifetimeMessage);
                    break;
            }
        }

        /// <summary>
        /// Verifies that the given <paramref name="result"/> contains
        /// the specified <paramref name="code"/> in it's errors collection.
        /// </summary>
        /// <param name="result">
        /// The <see cref="ResultBase"/> instance containing the error code.
        /// </param>
        /// <param name="code">
        /// The <see cref="ErrorCode"/> value expected to be present.
        /// </param>
        /// <param name="descriptionSubstring">
        /// The <see cref="string"/> value expected to be contained within expected error's description.
        /// </param>
        public static void ContainsErrorCode(ResultBase result, ErrorCode code, string? descriptionSubstring = null)
        {
            ArgumentNullException.ThrowIfNull(result);

            var errors = result
                .Errors
                .Where(error => error.Code == code)
                .ToList();

            if (errors.Count is 0)
            {
                var details = "No matching error code was found in collection.";
                var expected = $"Expected: \"{code}\"";
                var actual = $"Actual:   \"{string.Join(", ", result.Errors.Select(error => error.Code))}\"";

                var message = string.Join(Environment.NewLine, details, expected, actual);
                Assert.Fail(message);
            }

            if (descriptionSubstring is not null)
            {
                var containsExpectedDescription = errors
                    .Any(error => error.Description.Contains(descriptionSubstring, StringComparison.Ordinal));

                if (!containsExpectedDescription)
                {
                    var details = "None of the matched errors contains the specified description sub-string.";
                    var expected = $"Expected: \"{descriptionSubstring}\"";
                    var actual = $"Actual:   \"{string.Join(", ", errors.Select(error => error.Description))}\"";

                    var message = string.Join(Environment.NewLine, details, expected, actual);

                    Assert.Fail(message);
                }
            }
        }
        #endregion
    }
}
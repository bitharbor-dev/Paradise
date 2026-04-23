using Microsoft.AspNetCore.Identity;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.ApplicationLogic.Infrastructure.Seed.Implementation;
using Paradise.ApplicationLogic.Infrastructure.Services;
using Paradise.Domain.Identity.Roles;
using Paradise.Domain.Identity.Users;
using Paradise.Models;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.Identity;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.ApplicationLogic.Infrastructure.Services;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Core.DataAccess;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.Microsoft.Extensions.Logging;
using Paradise.Tests.Miscellaneous.TestDoubles.Fakes.System;
using System.Globalization;

namespace Paradise.ApplicationLogic.Infrastructure.Tests.Unit.Seed;

public sealed partial class DatabaseSeederTests
{
    #region Properties
    /// <summary>
    /// Test environment.
    /// </summary>
    private TestEnvironment Test { get; } = new();

    /// <summary>
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </summary>
    public CancellationToken Token { get; } = TestContext.Current.CancellationToken;
    #endregion

    #region Nested types
    /// <summary>
    /// Provides setup and behavior check methods for the <see cref="DatabaseSeederTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Fields
        private readonly FakeLogger<DatabaseSeeder> _logger;
        private readonly FakeDataSource _domainDataSource;
        private readonly FakeDataSource _infrastructureDataSource;
        private readonly FakeRoleManager _roleManager;
        private readonly FakeUserManager _userManager;
        private readonly FakeEmailTemplateService _emailTemplateService;

        private readonly List<Exception> _loggedExceptions = [];
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestEnvironment"/> class.
        /// </summary>
        public TestEnvironment()
        {
            var timeProvider = new FakeTimeProvider();

            _logger = new();

            _domainDataSource = new(timeProvider);
            _infrastructureDataSource = new(timeProvider);

            _roleManager = new FakeRoleManager(_domainDataSource);
            _userManager = new FakeUserManager(timeProvider, _domainDataSource, new IdentityOptions());

            _emailTemplateService = new FakeEmailTemplateService(_infrastructureDataSource);

            Target = new(_logger, _roleManager, _userManager, _domainDataSource, _infrastructureDataSource, _emailTemplateService);

            _logger.MessageLogged += OnMessageLogged;
            _domainDataSource.PersistenceStoragePreparedAsync += OnDomainPersistenceStoragePreparedAsync;
            _infrastructureDataSource.PersistenceStoragePreparedAsync += OnInfrastructurePersistenceStoragePreparedAsync;
        }
        #endregion

        #region Properties
        /// <summary>
        /// System under test.
        /// </summary>
        public DatabaseSeeder Target { get; }

        /// <summary>
        /// Indicates whether the domain storage is prepared and ready to be used.
        /// </summary>
        public bool DomainStoragePrepared { get; private set; }

        /// <summary>
        /// Indicates whether the infrastructure storage is prepared and ready to be used.
        /// </summary>
        public bool InfrastructureStoragePrepared { get; private set; }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates a <see cref="Role"/> instance
        /// and saves its data into the test persistence storage.
        /// </summary>
        /// <param name="name">
        /// Role name.
        /// </param>
        /// <param name="isDefault">
        /// Indicates whether the role is default and should be
        /// assigned automatically when a user has been created.
        /// </param>
        public void AddRole(string name, bool isDefault = false)
        {
            var role = new Role(name, isDefault)
            {
                NormalizedName = name
            };

            _domainDataSource.Add(role);
            _domainDataSource.SaveChanges();
        }

        /// <summary>
        /// Checks if the <see cref="Role"/> with the given
        /// <paramref name="roleName"/> and <paramref name="isDefault"/> values
        /// exists in the persistence storage.
        /// </summary>
        /// <param name="roleName">
        /// Role name.
        /// </param>
        /// <param name="isDefault">
        /// Indicates whether the role is default and should be
        /// assigned automatically when a user has been created.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <see cref="Role"/> with the given
        /// <paramref name="roleName"/> and <paramref name="isDefault"/> values
        /// exists in the persistence storage,
        /// otherwise - <see langword="false"/>.
        /// </returns>
        public bool RoleExists(string roleName, bool isDefault = false)
        {
            return _domainDataSource
                .GetQueryable<Role>()
                .Any(role => role.Name == roleName
                          && role.IsDefault == isDefault);
        }

        /// <summary>
        /// Creates a <see cref="User"/> instance
        /// and saves its data into the test persistence storage.
        /// </summary>
        /// <param name="emailAddress">
        /// Email address.
        /// </param>
        /// <param name="userName">
        /// User-name.
        /// </param>
        public void AddUser(string emailAddress, string userName)
        {
            var user = new User(emailAddress, userName)
            {
                NormalizedEmail = emailAddress,
                NormalizedUserName = userName
            };

            _domainDataSource.Add(user);
            _domainDataSource.SaveChanges();
        }

        /// <summary>
        /// Checks if the <see cref="User"/> with the given
        /// <paramref name="userName"/> and <paramref name="emailAddress"/> values
        /// exists in the persistence storage.
        /// </summary>
        /// <param name="emailAddress">
        /// Email address.
        /// </param>
        /// <param name="userName">
        /// User-name.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <see cref="Role"/> with the given
        /// <paramref name="userName"/> and <paramref name="emailAddress"/> values
        /// exists in the persistence storage,
        /// otherwise - <see langword="false"/>.
        /// </returns>
        public bool UserExists(string emailAddress, string userName)
        {
            return _domainDataSource
                .GetQueryable<User>()
                .Any(user => user.Email == emailAddress
                          && user.UserName == userName);
        }

        /// <summary>
        /// Creates an <see cref="EmailTemplate"/> instance
        /// and saves its data into the test persistence storage.
        /// </summary>
        /// <param name="templateName">
        /// Template name.
        /// </param>
        /// <param name="cultureId">
        /// Template culture language code identifier.
        /// </param>
        /// <param name="templateText">
        /// Template text.
        /// </param>
        /// <param name="subject">
        /// Email subject.
        /// </param>
        public void AddEmailTemplate(string templateName, int? cultureId, string templateText, string subject)
        {
            var culture = cultureId.HasValue
                ? CultureInfo.GetCultureInfo(cultureId.Value)
                : null;

            var template = new EmailTemplate(templateName, culture, templateText, subject);

            _infrastructureDataSource.Add(template);
            _infrastructureDataSource.SaveChanges();
        }

        /// <summary>
        /// Checks if the <see cref="EmailTemplate"/> with the given
        /// <paramref name="templateName"/>, <paramref name="subject"/>,
        /// <paramref name="templateText"/> and <paramref name="cultureId"/> values
        /// exists in the persistence storage.
        /// </summary>
        /// <param name="templateName">
        /// Template name.
        /// </param>
        /// <param name="cultureId">
        /// Template culture language code identifier.
        /// </param>
        /// <param name="templateText">
        /// Template text.
        /// </param>
        /// <param name="subject">
        /// Email subject.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="EmailTemplate"/> with the given
        /// <paramref name="templateName"/>, <paramref name="subject"/>,
        /// <paramref name="templateText"/> and <paramref name="cultureId"/> values
        /// exists in the persistence storage,
        /// otherwise - <see langword="false"/>.
        /// </returns>
        public bool EmailTemplateExists(string templateName, int? cultureId, string templateText, string subject)
        {
            var culture = cultureId.HasValue
                ? CultureInfo.GetCultureInfo(cultureId.Value)
                : null;

            return _infrastructureDataSource
                .GetQueryable<EmailTemplate>()
                .Any(template => template.TemplateName == templateName
                              && template.Subject == subject
                              && template.TemplateText == templateText
                              && template.Culture == culture);
        }

        /// <summary>
        /// Intercepts the internal <see cref="IUserManager{TUser}.CreateAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserManagerCreateAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userManager.CreateAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IUserManager{TUser}.DeleteAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserManagerDeleteAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userManager.DeleteAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IUserManager{TUser}.AddToRolesAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetUserManagerAddToRolesAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _userManager.AddToRolesAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IRoleManager{TRole}.CreateAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetRoleManagerCreateAsyncResult(Func<Task<IdentityResult>> resultingDelegate)
            => _roleManager.CreateAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IEmailTemplateService.CreateAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetEmailTemplateServiceCreateAsyncResult(Func<Task<Result<EmailTemplateModel>>> resultingDelegate)
            => _emailTemplateService.CreateAsyncResult = resultingDelegate;

        /// <summary>
        /// Intercepts the internal <see cref="IEmailTemplateService.UpdateAsync"/>
        /// method call and makes it return the result using the given <paramref name="resultingDelegate"/>.
        /// </summary>
        /// <param name="resultingDelegate">
        /// A resulting delegate to intercept the target method.
        /// </param>
        public void SetEmailTemplateServiceUpdateAsyncResult(Func<Task<Result<EmailTemplateModel>>> resultingDelegate)
            => _emailTemplateService.UpdateAsyncResult = resultingDelegate;

        /// <summary>
        /// Checks if an exception of type <typeparamref name="TException"/>
        /// was logged during the test, and if such exception message
        /// is equal to <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="TException">
        /// Expected exception type.
        /// </typeparam>
        /// <param name="message">
        /// Expected exception message.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if an exception of type <typeparamref name="TException"/>
        /// was logged during the test, and if such exception message
        /// is equal to <paramref name="message"/>,
        /// otherwise - <see langword="false"/>.
        /// </returns>
        public bool ExceptionLogged<TException>(string? message = null)
            where TException : Exception
        {
            return _loggedExceptions
                .OfType<TException>()
                .Any(exception => exception.Message == message);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _logger.MessageLogged -= OnMessageLogged;
            _domainDataSource.PersistenceStoragePreparedAsync -= OnDomainPersistenceStoragePreparedAsync;
            _infrastructureDataSource.PersistenceStoragePreparedAsync -= OnInfrastructurePersistenceStoragePreparedAsync;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// The <see cref="FakeLogger{T}.MessageLogged"/> event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="MessageLoggedEventArgs"/> instance containing the event data.
        /// </param>
        private void OnMessageLogged(object? sender, MessageLoggedEventArgs e)
        {
            if (e.Exception is not null)
                _loggedExceptions.Add(e.Exception);
        }

        /// <summary>
        /// The <see cref="FakeDataSource.PersistenceStoragePreparedAsync"/> event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        private void OnDomainPersistenceStoragePreparedAsync(object? sender, EventArgs e)
            => DomainStoragePrepared = true;

        /// <summary>
        /// The <see cref="FakeDataSource.PersistenceStoragePreparedAsync"/> event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        private void OnInfrastructurePersistenceStoragePreparedAsync(object? sender, EventArgs e)
            => InfrastructureStoragePrepared = true;
        #endregion
    }
    #endregion
}
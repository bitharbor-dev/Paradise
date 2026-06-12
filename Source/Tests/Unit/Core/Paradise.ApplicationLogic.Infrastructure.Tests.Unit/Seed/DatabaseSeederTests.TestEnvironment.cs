using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Time.Testing;
using Paradise.ApplicationLogic.Infrastructure.Domain.Identity;
using Paradise.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.ApplicationLogic.Infrastructure.Identity;
using Paradise.ApplicationLogic.Infrastructure.Seed.Implementation;
using Paradise.ApplicationLogic.Infrastructure.Services.MessageTemplates;
using Paradise.Models;
using Paradise.Models.ApplicationLogic.Infrastructure.Domain.MessageTemplates;
using Paradise.Tests.Doubles.Fakes.Core.ApplicationLogic.Infrastructure.Identity;
using Paradise.Tests.Doubles.Fakes.Core.ApplicationLogic.Infrastructure.Services.MessageTemplates;
using Paradise.Tests.Doubles.Fakes.DataAccess;
using Paradise.Tests.Doubles.Fakes.Microsoft.Extensions.Logging;
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
    /// Provides setup and behavior-check methods for the <see cref="DatabaseSeederTests"/> class.
    /// </summary>
    private sealed class TestEnvironment : IDisposable
    {
        #region Fields
        private readonly FakeLogger<DatabaseSeeder> _logger;
        private readonly FakeDataSource _dataSource;
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

            _dataSource = new(timeProvider);

            _roleManager = new FakeRoleManager(_dataSource);
            _userManager = new FakeUserManager(timeProvider, _dataSource, new IdentityOptions());

            _emailTemplateService = new FakeEmailTemplateService(_dataSource);

            Target = new(_logger, _roleManager, _userManager, _dataSource, _emailTemplateService);

            _logger.MessageLogged += OnMessageLogged;
            _dataSource.PersistenceStoragePreparedAsync += OnDomainPersistenceStoragePreparedAsync;
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
        public bool StoragePrepared { get; private set; }
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

            _dataSource.Add(role);
            _dataSource.SaveChanges();
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
            return _dataSource
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

            _dataSource.Add(user);
            _dataSource.SaveChanges();
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
            return _dataSource
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

            _dataSource.Add(template);
            _dataSource.SaveChanges();
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

            return _dataSource
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
            _dataSource.PersistenceStoragePreparedAsync -= OnDomainPersistenceStoragePreparedAsync;
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
            => StoragePrepared = true;
        #endregion
    }
    #endregion
}
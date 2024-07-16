using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Extensions;
using Paradise.Localization.ExceptionsHandling;
using Paradise.Maintenance.Options.Base;
using System.Text.Json;

namespace Paradise.Maintenance.Workers.Base;

/// <summary>
/// Provides repeatable action execution capabilities.
/// </summary>
/// <typeparam name="TOptions">
/// Worker options type.
/// </typeparam>
internal abstract class WorkerBase<TOptions> : IHostedService, IDisposable
    where TOptions : WorkerOptionsBase
{
    #region Fields
    private protected readonly ILogger _logger;

    private CancellationTokenSource? _cancellationTokenSource;

    private readonly IServiceProvider _serviceProvider;
    private readonly IDisposable? _optionsReloadToken;
    private readonly Timer _executionTimer;

    private readonly JsonSerializerOptions? _jsonSerializerOptions;
    private TOptions _options;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkerBase{TOptions}"/> class.
    /// </summary>
    /// <param name="serviceProvider">
    /// Service provider to retrieve registered services.
    /// </param>
    protected WorkerBase(IServiceProvider serviceProvider)
    {
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger(GetType().Name);

        _serviceProvider = serviceProvider;

        var monitor = serviceProvider.GetRequiredService<IOptionsMonitor<TOptions>>();

        _optionsReloadToken = monitor.OnChange(UpdateOptions);
        _executionTimer = new(DoWorkInternal, null, Timeout.Infinite, Timeout.Infinite);

        _jsonSerializerOptions = _serviceProvider.GetService<IOptions<JsonSerializerOptions>>()?.Value;

        _options = monitor.CurrentValue;

        _logger.LogWorkerOptionsInitialState(Options, _jsonSerializerOptions);
    }
    #endregion

    #region Properties
    /// <summary>
    /// Worker options.
    /// </summary>
    public TOptions Options
    {
        get => _options;
        private set
        {
            var oldValue = _options;
            var newValue = value;

            _options = newValue;

            OnOptionsChanged(oldValue, newValue);
        }
    }
    #endregion

    #region Public methods
    /// <summary>
    /// This method is called when the <see cref="Timer"/> starts and when each
    /// invocation interval exceeding.
    /// </summary>
    /// <param name="provider">
    /// Service provider to retrieve registered services.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe
    /// while waiting for the task to complete.
    /// </param>
    public abstract Task ExecuteAsync(IServiceProvider provider, CancellationToken cancellationToken);

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        ChangeTimer(Options.Delay, Options.Interval);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        ChangeTimer(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
        _executionTimer.Dispose();

        _cancellationTokenSource?.Dispose();

        GC.SuppressFinalize(this);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Calls the <see cref="ExecuteAsync"/> method
    /// and logs the information about the execution.
    /// </summary>
    /// <param name="state">
    /// An object containing information to be used by the callback method, or <see langword="null"/>.
    /// </param>
    private async void DoWorkInternal(object? state)
    {
        try
        {
            var cancellationToken = _cancellationTokenSource?.Token ?? default;

            _logger.LogWorkerRunning(GetType());

            await using var scope = _serviceProvider.CreateAsyncScope();
            await ExecuteAsync(scope.ServiceProvider, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogWorkerExecutionFailure(e);
        }
    }

    /// <summary>
    /// Updates the <see cref="Options"/>.
    /// </summary>
    /// <param name="newValue">
    /// New value.
    /// </param>
    private void UpdateOptions(TOptions newValue)
    {
        if (Options != newValue)
            Options = newValue;
    }

    /// <summary>
    /// <see cref="OptionsChanged"/> event raiser.
    /// </summary>
    /// <param name="oldValue">
    /// Old options value.
    /// </param>
    /// <param name="newValue">
    /// New options value.
    /// </param>
    private void OnOptionsChanged(TOptions oldValue, TOptions newValue)
    {
        ChangeTimer(newValue.Delay, newValue.Interval);

        OptionsChanged?.Invoke(this, new(oldValue, newValue));

        _logger.LogWorkerOptionsChangedState(newValue, _jsonSerializerOptions);
    }

    /// <summary>
    /// Changes the <see cref="_executionTimer"/> delay and interval values.
    /// </summary>
    /// <param name="delay">
    /// Timer delay.
    /// </param>
    /// <param name="interval">
    /// Timer interval.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Occurs when timer update operation fails.
    /// </exception>
    private void ChangeTimer(TimeSpan delay, TimeSpan interval)
    {
        if (!_executionTimer.Change(delay, interval))
        {
            var message = ExceptionMessagesProvider.GetChangeTimerFailedMessage();

            throw new InvalidOperationException(message);
        }
    }
    #endregion

    #region Events
    /// <summary>
    /// Occurs when worker options were updated.
    /// </summary>
    public event EventHandler<WorkerOptionsChangedEventArgs<TOptions>>? OptionsChanged;
    #endregion
}
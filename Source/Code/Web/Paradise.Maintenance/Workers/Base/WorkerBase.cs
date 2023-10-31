using Microsoft.Extensions.Options;
using Paradise.ApplicationLogic.Extensions;
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
    /// <param name="logger">
    /// Logger.
    /// </param>
    /// <param name="optionsMonitor">
    /// Worker options.
    /// </param>
    protected WorkerBase(ILogger logger, IServiceProvider serviceProvider, IOptionsMonitor<TOptions> optionsMonitor)
    {
        _logger = logger;

        _serviceProvider = serviceProvider;
        _optionsReloadToken = optionsMonitor.OnChange(UpdateOptions);
        _executionTimer = new(DoWorkInternal, null, Timeout.Infinite, Timeout.Infinite);

        _jsonSerializerOptions = _serviceProvider.GetService<IOptions<JsonSerializerOptions>>()?.Value;

        _options = optionsMonitor.CurrentValue;

        OptionsChanged += OnOptionsChanged;

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
            _options = value;
            OptionsChanged?.Invoke(this, new(oldValue, _options));
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
    public abstract Task DoWorkAsync(IServiceProvider provider, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _executionTimer.Change(Options.Delay, Options.Interval);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _executionTimer.Change(Timeout.Infinite, Timeout.Infinite);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        OptionsChanged -= OnOptionsChanged;

        _optionsReloadToken?.Dispose();
        _executionTimer.Dispose();

        GC.SuppressFinalize(this);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Calls the <see cref="DoWorkAsync"/> method
    /// and logs the information about the execution.
    /// </summary>
    /// <param name="state">
    /// An object containing information to be used by the callback method, or null.
    /// </param>
    private async void DoWorkInternal(object? state)
    {
        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            _logger.LogWorkerRunning(GetType());

            await DoWorkAsync(scope.ServiceProvider)
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
    /// <see cref="OptionsChanged"/> event handler.
    /// </summary>
    /// <param name="source">
    /// The source of the event.
    /// </param>
    /// <param name="args">
    /// An object that contains <see cref="OptionsChanged"/> event data.
    /// </param>
    private void OnOptionsChanged(object? source, WorkerOptionsChangedEventArgs<TOptions> args)
    {
        _executionTimer.Change(Options.Delay, Options.Interval);

        _logger.LogWorkerOptionsChangedState(Options, _jsonSerializerOptions);
    }
    #endregion

    #region Events
    /// <summary>
    /// Occurs when worker options were updated.
    /// </summary>
    public event EventHandler<WorkerOptionsChangedEventArgs<TOptions>> OptionsChanged;
    #endregion
}
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using shcome.core.Abstractions;
using shcome.core.Diagnostics;
using shcome.core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace shcome.core.Infrastructure.Hosting
{
    /// <summary>
    /// A background service class which syncs with application startup event from host
    /// </summary>
    public abstract class StartupCoordinatedBackgroundService : IHostedService, IDisposable
    {
        private Task _executingTask;
        private bool _disposed = false;
        private readonly Lazy<ILogger> _logger;
        private readonly IHostApplicationLifetime _appLifeTime;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly TaskCompletionSource<bool> _startupCompletedTcs = new TaskCompletionSource<bool>();
        private readonly ILogEnricher _logEnrichter;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="appLifeTime">Application lifetime access to register events</param>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="logEnricher">Log enricher instance</param>
        protected StartupCoordinatedBackgroundService(IHostApplicationLifetime appLifeTime, ILoggerFactory loggerFactory, ILogEnricher logEnricher)
        {
            appLifeTime.CheckArgumentNull(nameof(appLifeTime));
            loggerFactory.CheckArgumentNull(nameof(loggerFactory));

            _appLifeTime = appLifeTime;
            _loggerFactory = loggerFactory;
            //_logger = loggerFactory.CreateLogger(GetType().FullName);
            _logger = new Lazy<ILogger>(CreateLogger, true);
            _logEnrichter = logEnricher;


        }
        /// <summary>
        /// Finalizer
        /// </summary>
        ~StartupCoordinatedBackgroundService() => Dispose(false);
        /// <summary>
        /// Indicating if the instance has been disposed
        /// </summary>
        public bool IsDisposed => _disposed;
        /// <summary>
        /// Access generated logger
        /// </summary>
        protected ILogger Logger => _logger.Value;
        /// <summary>
        /// Access logger factory
        /// </summary>
        /// <remarks>May be needed if you override the default <see cref="CreateLogger"/> method and create your own one!</remarks>
        protected ILoggerFactory LoggerFactory => _loggerFactory;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual ILogger CreateLogger()
        {
            return LoggerFactory.CreateLogger(GetType().FullName);
        }

        /// <summary>
        /// Override this to implement your backbround execution
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        protected abstract Task ExecuteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Push a property onto the context, returning an System.IDisposable that must later
        /// be used to remove the property, along with any others that may have been pushed
        /// on top of it and not yet popped. The property must be popped from the same thread/logical
        /// call context.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="destructureObjects">If true, and the value is a non-primitive, non-array type, then the value will
        /// be converted to a structure; otherwise, unknown types will be converted to scalars, which are generally stored as strings.</param>
        /// <returns></returns>
        protected void EnrichLogger(string name, object value, bool destructureObjects = false)
        {
            var disposable = _logEnrichter.PushProperty(name, value, destructureObjects, Logger);
            if (disposable != null)
                _disposables.Add(disposable);
        }
        /// <summary>
        /// Called before task execution
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        protected virtual Task Initialize(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Start the background service
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // on application started event - we trigger the startup complete task completion source
            if (_appLifeTime.ApplicationStarted.IsCancellationRequested)
                _startupCompletedTcs.TrySetResult(true);
            else
                _appLifeTime.ApplicationStarted.Register(() => _startupCompletedTcs.TrySetResult(true));

            _executingTask = Task.Factory.StartNew(() => StartInternalAsync(_stoppingCts.Token), TaskCreationOptions.LongRunning);

            // If the task is completed then return it,
            // this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }
        /// <summary>
        /// Internal startup synching task execution with startup completion
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        private async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            try
            {
                EnrichLogger("HostedServiceContext", this.GetType().FullName);
                // waiting for startup to finish
                Logger.LogTrace("Waiting for finished startup for instance '{instanceId}'.", this.GetHashCode());

                // Klaus: Change 2023-09-18: While implementing Kafka consumers we recognized that
                // there may be a timing issue between Consumer HostService instantiation and ApplicationStarted event.
                // The issue was, that the application startet could be fired at a time so that the completion signal for 
                // _startupCompletedTcs.Task would have never been catched.
                // for this, we have created a retry cycle with a small wait delay including direct checking
                // if the ApplicationStarted token has been triggered in the meanwhile.

                var startupCompleted = false;

                if (_appLifeTime.ApplicationStarted.IsCancellationRequested)
                    _startupCompletedTcs.TrySetResult(true);

                if (_startupCompletedTcs.Task.IsCompleted)
                    startupCompleted = _startupCompletedTcs.Task.Result;

                if (!startupCompleted)
                {
                    var retry = 30;
                    var retryCnt = 0;

                    var completedTask = await Task.WhenAny(_startupCompletedTcs.Task,
                                                            Task.Delay(TimeSpan.FromSeconds(1),
                                                            cancellationToken));
                    startupCompleted = _appLifeTime.ApplicationStarted.IsCancellationRequested || (_startupCompletedTcs.Task.IsCompleted && _startupCompletedTcs.Task.Result);

                    while (!startupCompleted && retryCnt < retry)
                    {
                        completedTask = await Task.WhenAny(_startupCompletedTcs.Task,
                                                            Task.Delay(TimeSpan.FromSeconds(1),
                                                            cancellationToken));
                        startupCompleted = _appLifeTime.ApplicationStarted.IsCancellationRequested || (_startupCompletedTcs.Task.IsCompleted && _startupCompletedTcs.Task.Result);

                        retryCnt++;
                    }
                }

                if (startupCompleted)
                {
                    Logger.LogTrace("Received finished startup '{instanceId}'.", this.GetHashCode());
                    // initialize service before execution
                    await Initialize(cancellationToken);
                    // Store the task we're executing
                    await ExecuteAsync(cancellationToken);
                }
                else
                {
                    Logger.LogTrace("Startup for instance timed out '{instanceId}'.", this.GetHashCode());
                }
            }
            catch (OperationCanceledException te)
            {
                Logger.LogWarning(te, "Task cancelled. Exiting.");
            }
            catch (Exception ex)
            {
                if (ex.ContainsExceptionOfType<OutOfMemoryException>())
                {
                    Logger.LogCritical("Memory pressure identified due to OutOfMemory exception. Reporting unhealthy state!");
                    // report fatal state since background service will shut down after the exception
                    AppServiceProvider.SetOutOfMemoryDetected("Handled in Background service!", true);
                }
                else
                    throw;
            }
        }
        /// <summary>
        /// Stop the service
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();

                if (!_startupCompletedTcs.Task.IsCompleted)
                    _startupCompletedTcs.SetCanceled();

                await ShutdownAsync(cancellationToken);
            }
            catch (OperationCanceledException te)
            {
                Logger.LogWarning(te, "Task cancelled. Exiting.");
            }
            catch (Exception ex)
            {
                if (ex.ContainsExceptionOfType<OutOfMemoryException>())
                {
                    Logger.LogCritical("Memory pressure identified due to OutOfMemory exception. Reporting unhealthy state!");
                    AppServiceProvider.SetOutOfMemoryDetected("Handled in Background service!", true);
                }
                else
                    throw;
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                                                              cancellationToken));
            }
        }

        /// <summary>
        /// Shutdown actions during service stop
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        protected virtual Task ShutdownAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// An internal method to check if this instance has been disposed. 
        /// </summary>
        /// <remarks>
        /// Use this method in every method which interacts with the data source.
        /// </remarks>
        protected void CheckObjectDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        /// <summary>
        /// Dispose the service
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Override this on derived classes to implement additional disposal logic
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (!_startupCompletedTcs.Task.IsCompleted)
                _startupCompletedTcs.SetCanceled();

            if (disposing)
            {
                if (_disposables.Any())
                {
                    _disposables.ForEach(x => x.Dispose());
                    _disposables.Clear();
                }
            }

            _disposed = true;
        }
    }
}

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using synosscamera.core.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace synosscamera.core.Infrastructure.Hosting
{
    /// <summary>
    /// A background service class which syncs with application startup event from host
    /// </summary>
    public abstract class StartupCoordinatedBackgroundService : IHostedService, IDisposable
    {
        private Task _executingTask;
        private bool _disposed = false;
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifeTime;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        private readonly TaskCompletionSource<bool> _startupCompletedTcs = new TaskCompletionSource<bool>();

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="appLifeTime">Application lifetime access to register events</param>
        /// <param name="loggerFactory">Logger factory</param>
        protected StartupCoordinatedBackgroundService(IHostApplicationLifetime appLifeTime, ILoggerFactory loggerFactory)
        {
            appLifeTime.CheckArgumentNull(nameof(appLifeTime));
            loggerFactory.CheckArgumentNull(nameof(loggerFactory));

            _appLifeTime = appLifeTime;
            _logger = loggerFactory.CreateLogger(GetType().FullName);
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
        protected ILogger Logger => _logger;
        /// <summary>
        /// Override this to implement your backbround execution
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        protected abstract Task ExecuteAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Start the background service
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // on application started event - we trigger the startup complete task completion source
            _appLifeTime.ApplicationStarted.Register(() => _startupCompletedTcs.SetResult(true));

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
                // waiting for startup to finish
                var completedTask = await Task.WhenAny(_startupCompletedTcs.Task, Task.Delay(Timeout.Infinite, cancellationToken));
                if (completedTask == _startupCompletedTcs.Task && _startupCompletedTcs.Task.Result)
                {
                    // Store the task we're executing
                    await ExecuteAsync(cancellationToken);
                }
            }
            catch (TaskCanceledException te)
            {
                Logger.LogWarning(te, "Task cancelled. Exiting.");
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

            _disposed = true;
        }
    }
}

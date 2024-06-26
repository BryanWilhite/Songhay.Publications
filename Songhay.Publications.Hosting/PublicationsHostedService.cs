using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Songhay.Abstractions;

namespace Songhay.Publications.Hosting;

public class PublicationsHostedService: IHostedService
{
    public PublicationsHostedService(IActivityWithTask activity, IHostApplicationLifetime hostApplicationLifetime, ILogger<PublicationsHostedService> logger)
    {
        _activity = activity;
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // ReSharper disable once AsyncVoidLambda
        _hostApplicationLifetime.ApplicationStarted.Register(async () =>
        {
            try
            {
                await _activity.StartAsync();

                _exitCode = 0;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in {ClassName}.{MethodName}:", nameof(PublicationsHostedService), nameof(StartAsync));

                _exitCode = -1;
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("Stopping `{Name}`...", nameof(PublicationsHostedService));

        Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
        // FUNKYKB: exit code may be null should use enter Ctrl+c/SIGTERM.

        return Task.CompletedTask;
    }

    readonly IActivityWithTask _activity;
    readonly IHostApplicationLifetime _hostApplicationLifetime;
    readonly ILogger<PublicationsHostedService> _logger;

    int? _exitCode;
}

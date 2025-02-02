using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Songhay.Abstractions;

namespace Songhay.Publications.Hosting;

public class PublicationsHostedService(IActivityTask activity, IHostApplicationLifetime hostApplicationLifetime, ILogger<PublicationsHostedService> logger) : IHostedService
{
    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // ReSharper disable once AsyncVoidLambda
        hostApplicationLifetime.ApplicationStarted.Register(async () =>
        {
            try
            {
                await activity.StartAsync();

                _exitCode = 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in {ClassName}.{MethodName}:", nameof(PublicationsHostedService), nameof(StartAsync));

                _exitCode = -1;
            }
            finally
            {
                hostApplicationLifetime.StopApplication();
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
        logger.LogWarning("Stopping `{Name}`...", nameof(PublicationsHostedService));

        Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
        // FUNKYKB: exit code may be null should use enter Ctrl+c/SIGTERM.

        return Task.CompletedTask;
    }

    int? _exitCode;
}

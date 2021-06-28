using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Peeb.Bot.Clients.XivApi;
using Peeb.Bot.Clients.XivApi.Handlers;
using Peeb.Bot.Settings;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Peeb.Bot.Startup
{
    public static class XivApiStartup
    {
        public static IServiceCollection AddXivApi(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHttpClient<IXivApiClient, XivApiClient>()
                .AddHttpMessageHandler<AuthenticationHandler>()
                .AddTransientHttpErrorPolicy(c => c.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)));

            return services
                .AddTransient<AuthenticationHandler>()
                .Configure<XivApiSettings>(configuration.GetSection("XivApi"));
        }
    }
}

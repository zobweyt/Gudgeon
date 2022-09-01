using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using Gudgeon.Services;

namespace Gudgeon;

internal class Program
{
    private static async Task Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureDiscordHost((context, config) =>
            {
                config.SocketConfig = new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Debug,
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 200,
                    GatewayIntents = GatewayIntents.All
                };

                string? token = context.Configuration["Token"];

                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("The token is null or empty", nameof(token));

                config.Token = token;
            })
            .UseInteractionService((context, config) =>
            {
                config.LogLevel = LogSeverity.Debug;
                config.UseCompiledLambda = true;
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<InteractionHandler>();
            })
            .Build();

        await host.RunAsync();
    }
}
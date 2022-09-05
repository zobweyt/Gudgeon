using System.Reflection;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Services;

internal sealed partial class InteractionHandler : DiscordClientService
{
    private readonly IServiceProvider _provider;
    private readonly InteractionService _service;
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public InteractionHandler(DiscordSocketClient client, ILogger<DiscordClientService> logger, IServiceProvider provider, InteractionService service, IHostEnvironment environment, IConfiguration configuration)
        : base(client, logger)
    {
        _provider = provider;
        _service = service;
        _environment = environment;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Client.InteractionCreated += InteractionCreatedAsync;
        _service.InteractionExecuted += InteractionExecutedAsync;

        await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

        await Client.WaitForReadyAsync(stoppingToken);
        await Client.SetGameAsync("swiming", type: ActivityType.Competing);
        await RegisterCommandsAsync();
    }

    private async Task RegisterCommandsAsync()
    {
        var devGuildId = _configuration.GetValue<ulong>("DevGuild");
        var devGuild = Client.GetGuild(devGuildId);

        if (devGuild == null || !devGuild.Users.Any(x => x.Id == Client.CurrentUser.Id))
            throw new ArgumentException("Invalid guild! The guild does not exist or bot missing access to it.", nameof(devGuild));

        if (_environment.IsDevelopment())
        {
            await Client.Rest.DeleteAllGlobalCommandsAsync();
            await _service.RegisterCommandsToGuildAsync(devGuildId);
            return;
        }

        await Client.Rest.BulkOverwriteGuildCommands(Array.Empty<ApplicationCommandProperties>(), devGuildId);
        await _service.RegisterCommandsGloballyAsync();
    }
}
using System.Reflection;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Interactions;
using Discord.WebSocket;

namespace Gudgeon.Services;

internal sealed partial class InteractionHandler : DiscordClientService
{
    private static List<ulong>? _limitedGuilds = new();
    public static List<ulong>? LimitedGuilds
    {
        get { return _limitedGuilds; }
        private set { _limitedGuilds = value; }
    }

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
        Client.InteractionCreated += InteractionCreated;
        _service.InteractionExecuted += InteractionExecuted;

        await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

        await Client.WaitForReadyAsync(stoppingToken);
        await Client.SetGameAsync("swiming", type: ActivityType.Competing);
        await RegisterCommandsAsync();
    }

    private async Task RegisterCommandsAsync()
    {
        ulong? devGuild = _configuration.GetValue<ulong>("DevGuild");

        if (devGuild == null || devGuild == 0)
            throw new ArgumentException("The devGuild is null or zero", nameof(devGuild));

        if (_environment.IsDevelopment())
        {
            await Client.Rest.DeleteAllGlobalCommandsAsync();
            await _service.RegisterCommandsToGuildAsync(devGuild.Value);
            return;
        }

        await Client.Rest.BulkOverwriteGuildCommands(Array.Empty<ApplicationCommandProperties>(), devGuild.Value);
        await _service.RegisterCommandsGloballyAsync();
    }
}
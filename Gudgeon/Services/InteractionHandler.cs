using System.Reflection;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Interactions;
using Discord.WebSocket;
using Gudgeon.Common.Styles;

namespace Gudgeon.Services;

internal sealed class InteractionHandler : DiscordClientService
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

        if (_environment.IsDevelopment())
        {
            await Client.Rest.DeleteAllGlobalCommandsAsync();
            await _service.RegisterCommandsToGuildAsync(devGuildId);
            return;
        }
        
        if (devGuildId != 0)
            await Client.Rest.BulkOverwriteGuildCommands(Array.Empty<ApplicationCommandProperties>(), devGuildId);
        else
            Logger.LogWarning("Application commands for development guild could be duplicated due not to DevGuild specified.");

        await _service.RegisterCommandsGloballyAsync();
    }

    private async Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(Client, interaction);
            await _service.ExecuteCommandAsync(context, _provider);
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Exception occurred whilst attempting to handle interaction.");
        }
    }
    
    private async Task InteractionExecutedAsync(ICommandInfo commandInfo, IInteractionContext context, IResult result)
    {
        if (string.IsNullOrEmpty(result.ErrorReason) || result?.Error == InteractionCommandError.UnknownCommand)
            return;

        var embed = new EmbedBuilder()
            .WithStyle(result.IsSuccess ? new SuccessStyle() : new ErrorStyle())
            .WithDescription(result.ErrorReason)
            .Build();

        if (context.Interaction.HasResponded)
            await context.Interaction.FollowupAsync(embed: embed);
        else
            await context.Interaction.RespondAsync(embed: embed);
    }
}
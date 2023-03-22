using System.Reflection;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using Gudgeon.Styles;

namespace Gudgeon.Services;

internal sealed class InteractionHandler : DiscordClientService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly InteractionService _interactionService;
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public InteractionHandler(DiscordSocketClient client, ILogger<DiscordClientService> logger, IServiceProvider serviceProvider, InteractionService interactionService, IHostEnvironment environment, IConfiguration configuration)
        : base(client, logger)
    {
        _serviceProvider = serviceProvider;
        _interactionService = interactionService;
        _environment = environment;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Client.InteractionCreated += InteractionCreatedAsync;
        _interactionService.InteractionExecuted += InteractionExecutedAsync;

        var scope = _serviceProvider.CreateScope();
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), scope.ServiceProvider);

        await Client.WaitForReadyAsync(cancellationToken);
        await Client.SetGameAsync("swiming", type: ActivityType.Competing);
        await RegisterCommandsAsync();
    }

    private async Task RegisterCommandsAsync()
    {
        var devGuildId = _configuration.GetValue<ulong>("DevGuild");

        if (_environment.IsDevelopment())
        {
            await Client.Rest.DeleteAllGlobalCommandsAsync();
            await _interactionService.RegisterCommandsToGuildAsync(devGuildId);
            return;
        }
        
        if (devGuildId != 0)
            await Client.Rest.BulkOverwriteGuildCommands(Array.Empty<ApplicationCommandProperties>(), devGuildId);
        else
            Logger.LogWarning("Application commands for development guild could be duplicated due not to DevGuild specified.");

        await _interactionService.RegisterCommandsGloballyAsync();
    }

    private async Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(Client, interaction);
            await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
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
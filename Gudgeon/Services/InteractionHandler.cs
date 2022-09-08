using System.Reflection;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Interactions;
using Discord.WebSocket;
using Gudgeon.Common.Styles;
using static Gudgeon.RateLimitAttribute;

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

        await Client.Rest.BulkOverwriteGuildCommands(Array.Empty<ApplicationCommandProperties>(), devGuildId);
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
        if (!result.IsSuccess && result.Error == InteractionCommandError.UnknownCommand || result.Error == InteractionCommandError.Exception)
            return;

        if (!string.IsNullOrEmpty(result.ErrorReason))
            await HandleInterationAsync(context.Interaction, result);
        
        var contextId = commandInfo.Module.Name + "//" + commandInfo.MethodName + "//" + commandInfo.Name;
        ulong id = context.Guild?.Id ?? context.User.Id;
        if (DisableConcurrentExecutionAttribute.Items.Any(x => x.Key == id && x.Value == contextId))
            DisableConcurrentExecutionAttribute.Items.Remove(id, out contextId);
    }

    private async Task HandleInterationAsync(IDiscordInteraction interaction, IResult result)
    {
        EmbedStyle style = result.IsSuccess ? new SuccessStyle() : new ErrorStyle();
        var embed = new EmbedBuilder()
            .WithStyle(style)
            .WithDescription(result.ErrorReason)
            .Build();

        bool ephemeral = result is not GudgeonResult gudgeonResult || gudgeonResult.IsEphemeral;

        if (interaction.HasResponded)
        {
            await interaction.FollowupAsync(embed: embed, ephemeral: ephemeral);
            return;
        }
        await interaction.RespondAsync(embed: embed, ephemeral: ephemeral);
    }
}
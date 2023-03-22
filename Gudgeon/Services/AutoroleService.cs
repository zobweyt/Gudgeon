using Discord.WebSocket;
using Discord.Addons.Hosting;
using Gudgeon.Data.Models;

namespace Gudgeon.Services;

public class AutoroleService : DiscordClientService
{
    private readonly IServiceProvider _serviceProvider;

    public AutoroleService(DiscordSocketClient client, ILogger<DiscordClientService> logger, IServiceProvider serviceProvider)
        : base(client, logger)
    {
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Client.UserJoined += UserJoinedAsync;
        Client.RoleDeleted += RoleDeletedAsync;

        return Task.CompletedTask;
    }

    private async Task UserJoinedAsync(SocketGuildUser user)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GudgeonDbContext>();

        GuildUsersType target = user.IsBot ? GuildUsersType.Bots : GuildUsersType.Users;
        Autorole? autorole = dbContext.Autoroles.FirstOrDefault(x => x.GuildId == user.Guild.Id && x.UsersType == target);

        if (autorole == null)
            return;

        IRole role = user.Guild.GetRole(autorole.RoleId);
        await user.AddRoleAsync(role);
    }

    private async Task RoleDeletedAsync(SocketRole role)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GudgeonDbContext>();

        IQueryable<Autorole>? autoroles = dbContext.Autoroles.Where(x => x.GuildId == role.Guild.Id && x.RoleId == role.Id);

        if (autoroles == null)
            return;

        foreach (Autorole autorole in autoroles)
            dbContext.Remove(autorole);

        await dbContext.SaveChangesAsync();
    }
}
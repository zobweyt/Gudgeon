namespace Gudgeon;

public sealed class DoHierarchyCheckAttribute : ParameterPreconditionAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
    {
        if (context.User is not IGuildUser user)
            return PreconditionResult.FromError("This command cannot be used outside of a guild.");

        int targetHieararchy = value switch
        {
            IRole role => context.Guild.EveryoneRole.Id == role.Id ? int.MaxValue : role.IsManaged ? int.MaxValue : role.Position,
            IGuildUser guildUser => guildUser.Hierarchy,
            IUser => int.MinValue,
            _ => throw new ArgumentOutOfRangeException(nameof(value), "Attribute cannot be added to this parameter.")
        };

        if (targetHieararchy >= user.Hierarchy)
            return PreconditionResult.FromError("You cannot target anyone else whose roles are higher than yours.");

        IGuildUser bot = await context.Guild.GetCurrentUserAsync().ConfigureAwait(false);
        if (targetHieararchy >= bot.Hierarchy)
            return PreconditionResult.FromError("The bot's role is lower than the targeted entity.");

        return PreconditionResult.FromSuccess();
    }
}
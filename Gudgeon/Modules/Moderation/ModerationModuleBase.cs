using Discord;
using Discord.Interactions;
using Fergun.Interactive;

namespace Gudgeon.Modules.Moderation;

[RequireUserPermission(GuildPermission.Administrator)]
public abstract class ModerationModuleBase : GudgeonModuleBase
{
    protected ModerationModuleBase(InteractiveService interactive) 
        : base(interactive)
    {
    }
}
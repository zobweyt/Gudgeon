using Discord;
using Discord.Interactions;
using Fergun.Interactive;

namespace Gudgeon;

[RequireBotPermission(ChannelPermission.UseExternalEmojis)]
public abstract class GameModuleBase : GudgeonModuleBase
{
    protected GameModuleBase(InteractiveService interactive) 
        : base(interactive)
    {
    }
}
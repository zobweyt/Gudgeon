﻿//Originally made by jalaljaleh for Dynastio.Discord
//Read the original file at https://github.com/jalaljaleh/Dynastio.Discord/blob/master/Dynastio.Bot/Interactions/Preconditions/RateLimit.cs
//Originally licensed under Apache 2.0 https://github.com/jalaljaleh/Dynastio.Discord/blob/master/LICENSE.txt
//
//Modifications:
// 

using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Collections.Concurrent;

namespace Gudgeon;

public sealed class RateLimit : PreconditionAttribute
{
    private static readonly ConcurrentDictionary<ulong, List<RateLimitItem>> Items = new();
    private readonly RateLimitType _context;
    private readonly RateLimitBaseType _baseType;
    private readonly int _requests;
    private readonly int _seconds;
    public RateLimit(int seconds = 4, int requests = 1, RateLimitType context = RateLimitType.User, RateLimitBaseType baseType = RateLimitBaseType.BaseOnCommandInfo)
    {
        _context = context;
        _requests = requests;
        _seconds = seconds;
        _baseType = baseType;
    }

    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        ulong id = _context switch
        {
            RateLimitType.User => context.User.Id,
            RateLimitType.Channel => context.Channel.Id,
            RateLimitType.Guild => context.Guild.Id,
            _ => throw new ArgumentOutOfRangeException(nameof(_context), $"Cannot find {typeof(RateLimitType)} case for this context.")
        };
        string? contextId = _baseType switch
        {
            RateLimitBaseType.BaseOnCommandInfo => commandInfo.Module + "//" + commandInfo.MethodName + "//" + commandInfo.Name,
            RateLimitBaseType.BaseOnMessageComponentCustomId => (context.Interaction as SocketMessageComponent).Data.CustomId,
            _ => throw new ArgumentOutOfRangeException(nameof(_baseType), $"Cannot find {typeof(RateLimitBaseType)} case for this base type.")
        };

        DateTime now = DateTime.UtcNow;
        DateTime expireAt = now.AddSeconds(_seconds);
        var rateLimits = Items.GetOrAdd(id, new List<RateLimitItem>());
        var baseRateLimits = rateLimits.FindAll(a => a.ContextId == contextId);

        foreach (var rateLimit in baseRateLimits)
            if (now >= rateLimit.ExpireAt)
                rateLimits.Remove(rateLimit);

        if (baseRateLimits.Count > _requests)
            return Task.FromResult(PreconditionResult.FromError($"You could use this command again at <t:{((DateTimeOffset)expireAt).ToUnixTimeSeconds()}:T>."));

        rateLimits.Add(new RateLimitItem(contextId, expireAt));
        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}
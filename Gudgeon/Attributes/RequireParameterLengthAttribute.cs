using Discord;
using Discord.Interactions;

namespace Gudgeon;

public sealed class RequireParameterLengthAttribute : ParameterPreconditionAttribute
{
    private readonly int _min = 0;
    private readonly int _max;
    public RequireParameterLengthAttribute(int max = int.MaxValue)
        => _max = max;
    public RequireParameterLengthAttribute(int min = 0, int max = int.MaxValue) : this(max)
        => _min = min;

    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, IParameterInfo parameterInfo, object value, IServiceProvider services)
    {
        if (value is null)
            return Task.FromResult(PreconditionResult.FromSuccess());

        int parameterLenght = value switch
        {
            int lenght => lenght,
            string parameter => parameter.Length,
            _ => throw new ArgumentOutOfRangeException(nameof(value), "Attribute cannot be added to this parameter.")
        };

        if (parameterLenght < _min || parameterLenght > _max)
            return Task.FromResult(PreconditionResult.FromError($"Parameter {parameterInfo.Name} should be less or equal {_max}{(_min == 0 ? string.Empty : $" and not less than {_min}")}."));
        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}
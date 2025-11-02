using FluentValidation;
using PermitManagement.Core.Entities;
using PermitManagement.Shared;
using static PermitManagement.Shared.ValidationRules;

namespace PermitManagement.Api;

public class PermitValidator : AbstractValidator<Permit>
{
    public PermitValidator()
    {
        RuleFor(p => p.Vehicle.Registration)
            .Matches(RegPattern)
            .WithMessage(InvalidRegistrationMessage);

        RuleFor(p => p.Zone.Name)
            .Must(ZoneInfo.IsValid)
            .WithMessage($"Zone must be {ZoneInfo.RangeDescription()}.");

        RuleFor(p => p.StartDate)
            .LessThan(p => p.EndDate)
            .WithMessage("Start date must be before end date.");
    }
}
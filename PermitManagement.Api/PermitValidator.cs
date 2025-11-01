using FluentValidation;
using PermitManagement.Core.Entities;

namespace PermitManagement.Api;

public class PermitValidator : AbstractValidator<Permit>
{
    public PermitValidator()
    {
        RuleFor(p => p.Vehicle.Registration)
            .Matches(@"^[A-Z]{1,3}\d{1,3}[A-Z]{0,3}$")
            .WithMessage("Invalid vehicle registration format.");

        RuleFor(p => p.Zone.Name)
            .Matches(@"^[A-K]$")
            .WithMessage("Zone must be A through K.");

        RuleFor(p => p.StartDate)
            .LessThan(p => p.EndDate)
            .WithMessage("Start date must be before end date.");
    }
}
﻿using FluentValidation;
using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Validators;

public class SportValidator : AbstractValidator<Sport>
{
    public SportValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name)
            .NotEmpty().MinimumLength(1).MaximumLength(30)
            .WithMessage("'Name' must be between 1 and 30 characters long.");
        RuleFor(x => x.Description)
            .NotEmpty().MinimumLength(1).MaximumLength(200)
            .WithMessage("'Description' must be between 1 and 200 characters long.");
        RuleFor(x => x.MinTeamSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("'MinTeamSize' must be greater than or equal to 1")
            .LessThan(x => x.MaxTeamSize)
            .WithMessage("'MinTeamSize' must be less than 'MaxTeamSize'");
        RuleFor(x => x.MaxTeamSize)
            .GreaterThan(x => x.MinTeamSize)
            .WithMessage("'MaxTeamSize' must be greater than 'MinTeamSize'");
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("'StartDate' must not be empty.")
            .LessThan(x => x.EndDate)
            .WithMessage("'StartDate' must be less than than 'EndDate'.");
        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("'EndDate' must not be empty")
            .GreaterThan(x => x.StartDate)
            .WithMessage("'EndDate' must be greater than 'StartDate'");
    }
}
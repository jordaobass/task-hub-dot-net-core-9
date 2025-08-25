// Validators/BoardValidators.cs
using FluentValidation;
using TaskHub.DTOs;

namespace TaskHub.Validators;

public class BoardCreateValidator : AbstractValidator<BoardCreateRequest>
{
    public BoardCreateValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(1024);
    }
}

public class BoardUpdateValidator : AbstractValidator<BoardUpdateRequest>
{
    public BoardUpdateValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(1024);
    }
}
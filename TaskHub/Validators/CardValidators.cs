// Validators/CardValidators.cs
using FluentValidation;
using TaskHub.DTOs;

namespace TaskHub.Validators;

public class ColumnCreateValidator : AbstractValidator<ColumnCreateRequest>
{
    public ColumnCreateValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(120);
    }
}

public class CardCreateValidator : AbstractValidator<CardCreateRequest>
{
    public CardCreateValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(180);
        RuleFor(x => x.Description).MaximumLength(2048);
    }
}

public class CommentCreateValidator : AbstractValidator<CommentCreateRequest>
{
    public CommentCreateValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
    }
}
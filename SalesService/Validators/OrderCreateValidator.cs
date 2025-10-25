using FluentValidation;
using SalesService.Models.DTOs;

namespace SalesService.Validators
{
    public class OrderCreateValidator : AbstractValidator<OrderCreateDto>
    {
        public OrderCreateValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("O pedido deve conter pelo menos um item.")
                .Must(items => items.Select(i => i.ProductId).Distinct().Count() == items.Count)
                .WithMessage("O pedido contém produtos duplicados.");

            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ProductId)
                    .GreaterThan(0).WithMessage("ProductId deve ser maior que zero.");

                items.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
            });
        }
    }
}
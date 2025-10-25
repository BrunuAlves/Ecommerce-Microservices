using FluentValidation;
using StockService.Models;

namespace StockService.Validators
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(3).WithMessage("Nome deve ter pelo menos 3 caracteres");

            RuleFor(p => p.Description)
                .MaximumLength(200).WithMessage("Descrição pode ter no máximo 200 caracteres");

            RuleFor(p => p.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Preço deve ser positivo")
                .LessThanOrEqualTo(10000).WithMessage("Preço não pode ultrapassar R$10.000");

            RuleFor(p => p.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantidade deve ser positiva");
        }
    }
}
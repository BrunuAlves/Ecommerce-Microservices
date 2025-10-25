namespace StockService.Tests.Validators
{
    public class ProductValidatorTests
    {
        private readonly ProductValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var product = new Product { Name = "", Description = "desc", Price = 10, Quantity = 1 };
            var result = _validator.TestValidate(product);
            result.ShouldHaveValidationErrorFor(p => p.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Too_Short()
        {
            var product = new Product { Name = "ab", Description = "desc", Price = 10, Quantity = 1 };
            var result = _validator.TestValidate(product);
            result.ShouldHaveValidationErrorFor(p => p.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Too_Long()
        {
            var longDesc = new string('x', 201);
            var product = new Product { Name = "Produto", Description = longDesc, Price = 10, Quantity = 1 };
            var result = _validator.TestValidate(product);
            result.ShouldHaveValidationErrorFor(p => p.Description);
        }

        [Fact]
        public void Should_Have_Error_When_Price_Is_Negative()
        {
            var product = new Product { Name = "Produto", Description = "desc", Price = -1, Quantity = 1 };
            var result = _validator.TestValidate(product);
            result.ShouldHaveValidationErrorFor(p => p.Price);
        }

        [Fact]
        public void Should_Have_Error_When_Price_Is_Too_High()
        {
            var product = new Product { Name = "Produto", Description = "desc", Price = 10001, Quantity = 1 };
            var result = _validator.TestValidate(product);
            result.ShouldHaveValidationErrorFor(p => p.Price);
        }

        [Fact]
        public void Should_Have_Error_When_Quantity_Is_Negative()
        {
            var product = new Product { Name = "Produto", Description = "desc", Price = 10, Quantity = -5 };
            var result = _validator.TestValidate(product);
            result.ShouldHaveValidationErrorFor(p => p.Quantity);
        }

        [Fact]
        public void Should_Not_Have_Error_For_Valid_Product()
        {
            var product = new Product
            {
                Name = "Produto válido",
                Description = "Descrição ok",
                Price = 99.99m,
                Quantity = 10
            };
            var result = _validator.TestValidate(product);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
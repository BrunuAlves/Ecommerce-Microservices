namespace SalesService.Tests.Validators
{
    public class OrderCreateValidatorTests
    {
        private readonly OrderCreateValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Items_Is_Empty()
        {
            var dto = new OrderCreateDto { Items = new List<OrderItemCreateDto>() };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Items);
        }

        [Fact]
        public void Should_Have_Error_When_ProductId_Is_Duplicated()
        {
            var dto = new OrderCreateDto
            {
                Items = new List<OrderItemCreateDto>
                {
                    new OrderItemCreateDto { ProductId = 1, Quantity = 1 },
                    new OrderItemCreateDto { ProductId = 1, Quantity = 2 }
                }
            };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.Items);
        }

        [Fact]
        public void Should_Have_Error_When_ProductId_Is_Zero()
        {
            var dto = new OrderCreateDto
            {
                Items = new List<OrderItemCreateDto>
                {
                    new OrderItemCreateDto { ProductId = 0, Quantity = 1 }
                }
            };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor("Items[0].ProductId");
        }

        [Fact]
        public void Should_Have_Error_When_Quantity_Is_Zero()
        {
            var dto = new OrderCreateDto
            {
                Items = new List<OrderItemCreateDto>
                {
                    new OrderItemCreateDto { ProductId = 1, Quantity = 0 }
                }
            };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor("Items[0].Quantity");
        }

        [Fact]
        public void Should_Not_Have_Error_For_Valid_DTO()
        {
            var dto = new OrderCreateDto
            {
                Items = new List<OrderItemCreateDto>
                {
                    new OrderItemCreateDto { ProductId = 1, Quantity = 2 },
                    new OrderItemCreateDto { ProductId = 2, Quantity = 1 }
                }
            };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Quantity_Is_Negative()
        {
            var dto = new OrderCreateDto
            {
                Items = new List<OrderItemCreateDto>
                {
                    new OrderItemCreateDto { ProductId = 1, Quantity = -5 }
                }
            };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor("Items[0].Quantity");
        }

        [Fact]
        public void Should_Have_Error_When_ProductId_Is_Negative()
        {
            var dto = new OrderCreateDto
            {
                Items = new List<OrderItemCreateDto>
                {
                    new OrderItemCreateDto { ProductId = -1, Quantity = 2 }
                }
            };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor("Items[0].ProductId");
        }

        [Fact]
        public void Should_Not_Have_Error_With_Large_Valid_Order()
        {
            var items = Enumerable.Range(1, 50)
                .Select(i => new OrderItemCreateDto { ProductId = i, Quantity = i })
                .ToList();

            var dto = new OrderCreateDto { Items = items };
            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Have_Error_When_Only_One_Item_Is_Invalid()
        {
            var dto = new OrderCreateDto
            {
                Items = new List<OrderItemCreateDto>
                {
                    new OrderItemCreateDto { ProductId = 1, Quantity = 2 },
                    new OrderItemCreateDto { ProductId = 2, Quantity = 0 } // inválido
                }
            };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor("Items[1].Quantity");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SalesService.Models;
using SalesService.Models.DTOs;
using SalesService.Repositories;
using SalesService.Services;
using FluentValidation;

namespace SalesService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _repo;
        private readonly RabbitMQPublisher _publisher;
        private readonly IMapper _mapper;
        private readonly IOrderValidator _validator;
        private readonly IValidator<OrderCreateDto> _dtoValidator;
        private readonly ILogger<RabbitMQConsumer> _logger;

        public OrdersController(IOrderRepository repo, RabbitMQPublisher publisher, IMapper mapper, IOrderValidator validator, IValidator<OrderCreateDto> dtoValidator, ILogger<RabbitMQConsumer> logger = null)
        {
            _repo = repo;
            _publisher = publisher;
            _mapper = mapper;
            _validator = validator;
            _dtoValidator = dtoValidator;
            _logger = logger;
        }

        private int GetUserIdFromHeader()
        {
            var header = Request.Headers["User-Id"].FirstOrDefault();
            if (!int.TryParse(header, out var userId))
                throw new UnauthorizedAccessException("ID do usuário não encontrado ou inválido.");
            return userId;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
        {
            var userId = GetUserIdFromHeader();
            var order = _mapper.Map<Order>(dto);
            order.UserId = userId;
            
            var dtoValidation = await _dtoValidator.ValidateAsync(dto);
            if (!dtoValidation.IsValid)
                return BadRequest(dtoValidation.Errors.Select(e => e.ErrorMessage));

            (bool isValid, string error) = await _validator.ValidateStockAsync(order);
            if (!isValid)
                return BadRequest(error);

            var saved = await _repo.AddAsync(order);

            await _publisher.PublishOrderCreatedAsync(new OrderEvent
            {
                Id = saved.Id,
                Status = saved.Status,
                Items = saved.Items.Select(i => new OrderItemEvent
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToArray()
            });
            _logger?.LogInformation($"📨 Pedido enviado para StockService - ID [{order.Id}]");
            var result = _mapper.Map<OrderReadDto>(saved);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserIdFromHeader();
            var orders = await _repo.GetByUserIdAsync(userId);
            var result = _mapper.Map<IEnumerable<OrderReadDto>>(orders);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserIdFromHeader();
            var order = await _repo.GetByIdAsync(id, userId);
            if (order == null) return NotFound();
            var result = _mapper.Map<OrderReadDto>(order);
            return Ok(result);
        }
    }
}
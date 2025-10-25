using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using StockService.Models;
using StockService.Models.DTOs;
using StockService.Repositories;

namespace StockService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        private int GetSellerIdFromHeader()
        {
            var header = Request.Headers["User-Id"].FirstOrDefault();
            if (!int.TryParse(header, out var sellerId))
                throw new UnauthorizedAccessException("ID do vendedor não encontrado ou inválido.");
            return sellerId;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _repo.GetAllAsync();
            var result = _mapper.Map<IEnumerable<ProductReadDto>>(products);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            
            if (p == null) return NotFound();
        
            var result = _mapper.Map<ProductReadDto>(p);
            return Ok(result);
        }

        [HttpGet("{id:int}/availability")]
        public async Task<IActionResult> Availability(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return NotFound();
            return Ok(new { productId = p.Id, quantity = p.Quantity, price = p.Price });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var sellerId = GetSellerIdFromHeader();

            var product = _mapper.Map<Product>(dto);
            product.SellerId = sellerId;

            await _repo.AddAsync(product);

            var result = _mapper.Map<ProductReadDto>(product);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductCreateDto dto)
        {
            var sellerId = GetSellerIdFromHeader();

            var updated = _mapper.Map<Product>(dto);
            updated.Id = id;

            var success = await _repo.UpdateAsync(updated, sellerId);
            if (!success) return StatusCode(403, "Você não tem permissão para excluir este produto.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sellerId = GetSellerIdFromHeader();

            var success = await _repo.DeleteAsync(id, sellerId);
            if (!success) return StatusCode(403, "Você não tem permissão para excluir este produto.");

            return NoContent();
        }
    }
}
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SalesService.Models.DTOs;

namespace SalesService.Services
{
    public class InventoryClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<InventoryClient> _logger;

        public InventoryClient(HttpClient http, ILogger<InventoryClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<ProductAvailabilityDto> GetAvailabilityAsync(int productId)
        {
            try
            {
                var response = await _http.GetAsync($"/api/products/{productId}/availability");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("❌ Erro ao consultar estoque do produto {ProductId}: {StatusCode}", productId, response.StatusCode);
                    return null;
                }

                var availability = await response.Content.ReadFromJsonAsync<ProductAvailabilityDto>();
                if (availability == null)
                    _logger.LogWarning("⚠️ Resposta nula ao consultar estoque do produto {ProductId}", productId);

                return availability;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Falha ao consultar estoque do produto {ProductId}", productId);
                return null;
            }
        }
    }
}
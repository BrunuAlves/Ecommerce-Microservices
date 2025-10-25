using AutoMapper;
using StockService.Models;
using StockService.Models.DTOs;

namespace StockService.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductCreateDto, Product>();
            CreateMap<Product, ProductReadDto>();
        }
    }
}
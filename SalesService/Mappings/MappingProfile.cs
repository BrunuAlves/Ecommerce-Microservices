using AutoMapper;
using SalesService.Models;
using SalesService.Models.DTOs;

namespace SalesService.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderCreateDto, Order>();
            CreateMap<OrderItemCreateDto, OrderItem>();

            CreateMap<Order, OrderReadDto>();
            CreateMap<OrderItem, OrderItemReadDto>();
        }
    }
}
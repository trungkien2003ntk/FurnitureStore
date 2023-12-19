using AutoMapper;
using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Shared;

namespace FurnitureStore.Server.Utils
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<ProductDTO, ProductDocument>();

            CreateMap<CategoryDTO, CategoryDocument>();

            CreateMap<StaffDTO, StaffDocument>();

            CreateMap<OrderDTO, OrderDocument>();

            CreateMap<ProductDocument, ProductDTO>();
            
            CreateMap<CategoryDocument, CategoryDTO>();
            
            CreateMap<StaffDocument, StaffDTO>();
            
            CreateMap<OrderDocument, OrderDTO>();
        }
    }
}

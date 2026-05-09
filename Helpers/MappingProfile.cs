using AutoMapper;
using MilkCollector.API.Models.Entities;
using MilkCollector.API.DTOs.Users;
using MilkCollector.API.DTOs.Farmers;

namespace MilkCollector.API.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            
            CreateMap<Farmer, FarmerDto>()
                .ForMember(dest => dest.FatRateKind, opt => opt.Ignore());
        }
    }
}

using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace CompanyEmployees
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                //.ForCtorParam("FullAddress",
                  .ForMember(c => c.FullAddress,
                    opt => opt.MapFrom(x => String.Join(' ', x.Address, x.Country)));

            CreateMap<Employee, EmployeeDto>();
        }
    }
}

using AutoMapper;
using Fixy.Application.Common.Helpers;
using Fixy.Domain.Enums;

namespace Fixy.Application.Mapping.LocalizedEnums;

public class LocalizedEnumsProfile : Profile
{
    public LocalizedEnumsProfile()
    {
        CreateMap<ServiceBookingStatus, string>()
            .ConvertUsing<EnumToLocalizedStringConverter<ServiceBookingStatus>>();

        CreateMap<ServiceRequestStatus, string>()
            .ConvertUsing<EnumToLocalizedStringConverter<ServiceRequestStatus>>();

        CreateMap<TechnicianStatus, string>()
            .ConvertUsing<EnumToLocalizedStringConverter<TechnicianStatus>>();
    }
}

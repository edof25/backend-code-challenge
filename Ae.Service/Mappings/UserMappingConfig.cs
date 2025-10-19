using Ae.Domain.DTOs;
using Ae.Domain.Entities;
using Mapster;

namespace Ae.Service.Mappings;

public static class UserMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<User, UserResponse>.NewConfig()
            .Map(dest => dest.RoleName, src => src.Role != null ? src.Role.Name : string.Empty)
            .Map(dest => dest.RecordStatusName, src => src.RecordStatus != null ? src.RecordStatus.Name : string.Empty);
    }
}

using Ae.Domain.Entities;
using Ae.Infrastructure.Models;
using Mapster;

namespace Ae.Infrastructure.Mappings;

public static class UserMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<UserResult, User>.NewConfig()
            .Map(dest => dest.Role, src => src.RoleName != null ? new Role { Id = src.RoleId, Name = src.RoleName } : null)
            .Map(dest => dest.RecordStatus, src => src.RecordStatusName != null ? new RecordStatus { Id = src.RecordStatusId, Name = src.RecordStatusName } : null);
    }
}

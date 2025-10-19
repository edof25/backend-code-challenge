using Ae.Domain.Entities;
using Ae.Infrastructure.Models;
using Mapster;

namespace Ae.Infrastructure.Mappings;

public static class ShipMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<ShipResult, Ship>.NewConfig()
            .Map(dest => dest.RecordStatus, src => src.RecordStatusName != null
                ? new RecordStatus { Id = src.RecordStatusId, Name = src.RecordStatusName }
                : null);
    }
}

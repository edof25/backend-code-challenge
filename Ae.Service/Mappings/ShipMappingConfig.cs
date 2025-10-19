using Ae.Domain.DTOs.Ship;
using Ae.Domain.Entities;
using Mapster;

namespace Ae.Service.Mappings;

public static class ShipMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Ship, ShipResponse>.NewConfig()
            .Map(dest => dest.RecordStatusName, src => src.RecordStatus != null ? src.RecordStatus.Name : string.Empty);
    }
}

using Ae.Domain.Entities;
using Ae.Infrastructure.Models;
using Mapster;

namespace Ae.Infrastructure.Mappings;

public static class UserShipMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<UserShipResult, CrewServiceHistory>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.RankId, src => src.RankId ?? (byte)0)
            .Map(dest => dest.ShipId, src => src.ShipId)
            .Map(dest => dest.SignOnDate, src => src.SignOnDate ?? src.AssignedDate)
            .Map(dest => dest.SignOffDate, src => src.SignOffDate)
            .Map(dest => dest.EndOfContractDate, src => src.EndOfContractDate ?? DateTime.MinValue)
            .Map(dest => dest.RecordStatusId, src => src.RecordStatusId)
            .Map(dest => dest.User, src => src.UserFirstName != null || src.UserLastName != null
                ? new User
                {
                    Id = src.UserId,
                    CrewMemberId = src.CrewMemberId,
                    FirstName = src.UserFirstName ?? string.Empty,
                    LastName = src.UserLastName ?? string.Empty,
                    BirthDate = src.BirthDate ?? DateTime.MinValue,
                    Nationality = src.Nationality ?? string.Empty
                }
                : null)
            .Map(dest => dest.Rank, src => src.RankName != null
                ? new Rank { Id = src.RankId ?? 0, Name = src.RankName }
                : null)
            .Map(dest => dest.Ship, src => src.ShipCode != null
                ? new Ship { Id = src.ShipId, Code = src.ShipCode, Name = src.ShipName ?? string.Empty }
                : null)
            .Map(dest => dest.RecordStatus, src => src.RecordStatusName != null
                ? new RecordStatus { Id = src.RecordStatusId, Name = src.RecordStatusName }
                : null);
    }
}

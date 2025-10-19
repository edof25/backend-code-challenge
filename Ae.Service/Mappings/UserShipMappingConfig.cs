using Ae.Domain.DTOs.UserShip;
using Ae.Domain.Entities;
using Mapster;

namespace Ae.Service.Mappings;

public static class UserShipMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<CrewServiceHistory, UserShipResponse>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.ShipId, src => src.ShipId)
            .Map(dest => dest.AssignedDate, src => src.SignOnDate)
            .Map(dest => dest.CrewMemberId, src => src.User != null ? src.User.CrewMemberId : null)
            .Map(dest => dest.UserFirstName, src => src.User != null ? src.User.FirstName : string.Empty)
            .Map(dest => dest.UserLastName, src => src.User != null ? src.User.LastName : string.Empty)
            .Map(dest => dest.BirthDate, src => src.User != null ? src.User.BirthDate : DateTime.MinValue)
            .Map(dest => dest.Nationality, src => src.User != null ? src.User.Nationality : string.Empty)
            .Map(dest => dest.RankId, src => src.RankId)
            .Map(dest => dest.RankName, src => src.Rank != null ? src.Rank.Name : string.Empty)
            .Map(dest => dest.SignOnDate, src => src.SignOnDate)
            .Map(dest => dest.SignOffDate, src => src.SignOffDate)
            .Map(dest => dest.EndOfContractDate, src => src.EndOfContractDate)
            .Map(dest => dest.ShipCode, src => src.Ship != null ? src.Ship.Code : string.Empty)
            .Map(dest => dest.ShipName, src => src.Ship != null ? src.Ship.Name : string.Empty)
            .Map(dest => dest.RecordStatusId, src => src.RecordStatusId)
            .Map(dest => dest.RecordStatusName, src => src.RecordStatus != null ? src.RecordStatus.Name : string.Empty)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.CreatedBy, src => src.CreatedBy);
    }
}

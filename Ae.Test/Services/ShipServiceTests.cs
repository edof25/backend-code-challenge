using Ae.Domain.DTOs.Common;
using Ae.Domain.DTOs.Ship;
using Ae.Domain.Entities;
using Ae.Infrastructure.Interfaces;
using Ae.Service.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Ae.Test.Services;

public class ShipServiceTests
{
    private readonly Mock<IShipRepository> _mockShipRepository;
    private readonly Mock<ILogger<ShipService>> _mockLogger;
    private readonly ShipService _shipService;

    public ShipServiceTests()
    {
        _mockShipRepository = new Mock<IShipRepository>();
        _mockLogger = new Mock<ILogger<ShipService>>();
        _shipService = new ShipService(_mockShipRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllShipsAsync_ShouldReturnPagedResult()
    {
        // Arrange
        var paginationRequest = new PaginationRequest { PageNumber = 1, PageSize = 10 };
        var ships = new List<Ship>
        {
            new Ship { Id = 1, Code = "SHIP001", Name = "USS Enterprise", FiscalYear = "0112" },
            new Ship { Id = 2, Code = "SHIP002", Name = "HMS Victory", FiscalYear = "0803" }
        };
        var totalCount = 2;

        _mockShipRepository
            .Setup(x => x.GetAllAsync(paginationRequest))
            .ReturnsAsync((ships, totalCount));

        // Act
        var result = await _shipService.GetAllShipsAsync(paginationRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(totalCount, result.TotalCount);
        Assert.Equal(paginationRequest.PageNumber, result.PageNumber);
        Assert.Equal(paginationRequest.PageSize, result.PageSize);
        _mockShipRepository.Verify(x => x.GetAllAsync(paginationRequest), Times.Once);
    }

    [Fact]
    public async Task GetShipByIdAsync_WhenShipExists_ShouldReturnShipResponse()
    {
        // Arrange
        var shipId = 1;
        var ship = new Ship
        {
            Id = shipId,
            Code = "SHIP001",
            Name = "USS Enterprise",
            FiscalYear = "0112"
        };

        _mockShipRepository
            .Setup(x => x.GetByIdAsync(shipId))
            .ReturnsAsync(ship);

        // Act
        var result = await _shipService.GetShipByIdAsync(shipId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ship.Id, result.Id);
        Assert.Equal(ship.Code, result.Code);
        Assert.Equal(ship.Name, result.Name);
        Assert.Equal(ship.FiscalYear, result.FiscalYear);
        _mockShipRepository.Verify(x => x.GetByIdAsync(shipId), Times.Once);
    }

    [Fact]
    public async Task GetShipByIdAsync_WhenShipDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var shipId = 999;
        _mockShipRepository
            .Setup(x => x.GetByIdAsync(shipId))
            .ReturnsAsync((Ship?)null);

        // Act
        var result = await _shipService.GetShipByIdAsync(shipId);

        // Assert
        Assert.Null(result);
        _mockShipRepository.Verify(x => x.GetByIdAsync(shipId), Times.Once);
    }

    [Fact]
    public async Task GetShipByCodeAsync_WhenShipExists_ShouldReturnShipResponse()
    {
        // Arrange
        var shipCode = "SHIP001";
        var ship = new Ship
        {
            Id = 1,
            Code = shipCode,
            Name = "USS Enterprise",
            FiscalYear = "0112"
        };

        _mockShipRepository
            .Setup(x => x.GetByCodeAsync(shipCode))
            .ReturnsAsync(ship);

        // Act
        var result = await _shipService.GetShipByCodeAsync(shipCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ship.Id, result.Id);
        Assert.Equal(ship.Code, result.Code);
        Assert.Equal(ship.Name, result.Name);
        Assert.Equal(ship.FiscalYear, result.FiscalYear);
        _mockShipRepository.Verify(x => x.GetByCodeAsync(shipCode), Times.Once);
    }

    [Fact]
    public async Task GetShipByCodeAsync_WhenShipDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var shipCode = "NOTEXIST";
        _mockShipRepository
            .Setup(x => x.GetByCodeAsync(shipCode))
            .ReturnsAsync((Ship?)null);

        // Act
        var result = await _shipService.GetShipByCodeAsync(shipCode);

        // Assert
        Assert.Null(result);
        _mockShipRepository.Verify(x => x.GetByCodeAsync(shipCode), Times.Once);
    }

    [Fact]
    public async Task GetShipsByUserIdAsync_ShouldReturnListOfShips()
    {
        // Arrange
        var userId = 1;
        var ships = new List<Ship>
        {
            new Ship { Id = 1, Code = "SHIP001", Name = "USS Enterprise", FiscalYear = "0112" },
            new Ship { Id = 2, Code = "SHIP002", Name = "HMS Victory", FiscalYear = "0803" }
        };

        _mockShipRepository
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(ships);

        // Act
        var result = await _shipService.GetShipsByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockShipRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetShipsByUserIdAsync_WhenNoShipsFound_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = 999;
        _mockShipRepository
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<Ship>());

        // Act
        var result = await _shipService.GetShipsByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockShipRepository.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task CreateShipAsync_ShouldReturnCreatedShip()
    {
        // Arrange
        var createRequest = new CreateShipRequest
        {
            Code = "SHIP003",
            Name = "Titanic",
            FiscalYear = "0512"
        };

        var createdBy = "admin";
        var createdShip = new Ship
        {
            Id = 3,
            Code = createRequest.Code,
            Name = createRequest.Name,
            FiscalYear = createRequest.FiscalYear
        };

        _mockShipRepository
            .Setup(x => x.CreateAsync(It.IsAny<Ship>(), createdBy))
            .ReturnsAsync(createdShip);

        // Act
        var result = await _shipService.CreateShipAsync(createRequest, createdBy);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdShip.Id, result.Id);
        Assert.Equal(createRequest.Code, result.Code);
        Assert.Equal(createRequest.Name, result.Name);
        Assert.Equal(createRequest.FiscalYear, result.FiscalYear);
        _mockShipRepository.Verify(x => x.CreateAsync(It.Is<Ship>(s =>
            s.Code == createRequest.Code &&
            s.Name == createRequest.Name &&
            s.FiscalYear == createRequest.FiscalYear
        ), createdBy), Times.Once);
    }

    [Fact]
    public async Task UpdateShipAsync_ShouldReturnUpdatedShip()
    {
        // Arrange
        var shipId = 1;
        var updateRequest = new UpdateShipRequest
        {
            Code = "SHIP001",
            Name = "Updated Ship Name",
            FiscalYear = "0110"
        };

        var updatedBy = "admin";
        var updatedShip = new Ship
        {
            Id = shipId,
            Code = updateRequest.Code,
            Name = updateRequest.Name,
            FiscalYear = updateRequest.FiscalYear
        };

        _mockShipRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Ship>(), updatedBy))
            .ReturnsAsync(updatedShip);

        // Act
        var result = await _shipService.UpdateShipAsync(shipId, updateRequest, updatedBy);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedShip.Id, result.Id);
        Assert.Equal(updateRequest.Code, result.Code);
        Assert.Equal(updateRequest.Name, result.Name);
        Assert.Equal(updateRequest.FiscalYear, result.FiscalYear);
        _mockShipRepository.Verify(x => x.UpdateAsync(It.Is<Ship>(s => s.Id == shipId), updatedBy), Times.Once);
    }

    [Fact]
    public async Task DeleteShipAsync_ShouldReturnTrue_WhenDeleteIsSuccessful()
    {
        // Arrange
        var shipId = 1;
        var deletedBy = "admin";

        _mockShipRepository
            .Setup(x => x.DeleteAsync(shipId, deletedBy))
            .ReturnsAsync(true);

        // Act
        var result = await _shipService.DeleteShipAsync(shipId, deletedBy);

        // Assert
        Assert.True(result);
        _mockShipRepository.Verify(x => x.DeleteAsync(shipId, deletedBy), Times.Once);
    }

    [Fact]
    public async Task DeleteShipAsync_ShouldReturnFalse_WhenDeleteFails()
    {
        // Arrange
        var shipId = 999;
        var deletedBy = "admin";

        _mockShipRepository
            .Setup(x => x.DeleteAsync(shipId, deletedBy))
            .ReturnsAsync(false);

        // Act
        var result = await _shipService.DeleteShipAsync(shipId, deletedBy);

        // Assert
        Assert.False(result);
        _mockShipRepository.Verify(x => x.DeleteAsync(shipId, deletedBy), Times.Once);
    }
}

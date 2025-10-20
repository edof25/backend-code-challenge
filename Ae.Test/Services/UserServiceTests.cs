using Ae.Domain.Configuration;
using Ae.Domain.DTOs;
using Ae.Domain.DTOs.Common;
using Ae.Domain.Entities;
using Ae.Infrastructure.Interfaces;
using Ae.Service.Interfaces;
using Ae.Service.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Ae.Test.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IOptions<JwtSettings>> _mockJwtSettings;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;
    private readonly JwtSettings _jwtSettings;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockTokenService = new Mock<ITokenService>();
        _mockJwtSettings = new Mock<IOptions<JwtSettings>>();
        _mockLogger = new Mock<ILogger<UserService>>();

        _jwtSettings = new JwtSettings
        {
            Secret = "test-secret-key-for-jwt-token-generation",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpirationInMinutes = 60
        };

        _mockJwtSettings.Setup(x => x.Value).Returns(_jwtSettings);

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockTokenService.Object,
            _mockJwtSettings.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnPagedResult()
    {
        // Arrange
        var paginationRequest = new PaginationRequest { PageNumber = 1, PageSize = 10 };
        var users = new List<User>
        {
            new User { Id = 1, Username = "user1", FirstName = "John", LastName = "Doe", RoleId = 1 },
            new User { Id = 2, Username = "user2", FirstName = "Jane", LastName = "Smith", RoleId = 2 }
        };
        var totalCount = 2;

        _mockUserRepository
            .Setup(x => x.GetAllAsync(paginationRequest))
            .ReturnsAsync((users, totalCount));

        // Act
        var result = await _userService.GetAllUsersAsync(paginationRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(totalCount, result.TotalCount);
        Assert.Equal(paginationRequest.PageNumber, result.PageNumber);
        Assert.Equal(paginationRequest.PageSize, result.PageSize);
        _mockUserRepository.Verify(x => x.GetAllAsync(paginationRequest), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUserResponse()
    {
        // Arrange
        var userId = 1;
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            FirstName = "Test",
            LastName = "User",
            RoleId = 1,
            BirthDate = new DateTime(1990, 1, 1),
            Nationality = "USA"
        };

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.FirstName, result.FirstName);
        Assert.Equal(user.LastName, result.LastName);
        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.Null(result);
        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldHashPasswordAndReturnCreatedUser()
    {
        // Arrange
        var createRequest = new CreateUserRequest
        {
            Username = "newuser",
            Password = "password123",
            FirstName = "New",
            LastName = "User",
            RoleId = 1,
            BirthDate = new DateTime(1995, 5, 15),
            Nationality = "USA"
        };

        var createdBy = "admin";
        var createdUser = new User
        {
            Id = 1,
            Username = createRequest.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(createRequest.Password),
            FirstName = createRequest.FirstName,
            LastName = createRequest.LastName,
            RoleId = createRequest.RoleId,
            BirthDate = createRequest.BirthDate,
            Nationality = createRequest.Nationality
        };

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>(), createdBy))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _userService.CreateUserAsync(createRequest, createdBy);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdUser.Id, result.Id);
        Assert.Equal(createRequest.Username, result.Username);
        _mockUserRepository.Verify(x => x.CreateAsync(It.Is<User>(u =>
            u.Username == createRequest.Username
        ), createdBy), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnUpdatedUser()
    {
        // Arrange
        var userId = 1;
        var updateRequest = new UpdateUserRequest
        {
            FirstName = "Updated",
            LastName = "User",
            BirthDate = new DateTime(1990, 1, 1),
            Nationality = "Canada"
        };

        var updatedBy = "admin";
        var updatedUser = new User
        {
            Id = userId,
            Username = "testuser",
            FirstName = updateRequest.FirstName,
            LastName = updateRequest.LastName,
            RoleId = 1,
            BirthDate = updateRequest.BirthDate,
            Nationality = updateRequest.Nationality
        };

        _mockUserRepository
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), updatedBy))
            .ReturnsAsync(updatedUser);

        // Act
        var result = await _userService.UpdateUserAsync(userId, updateRequest, updatedBy);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedUser.Id, result.Id);
        Assert.Equal(updateRequest.FirstName, result.FirstName);
        Assert.Equal(updateRequest.LastName, result.LastName);
        _mockUserRepository.Verify(x => x.UpdateAsync(It.Is<User>(u => u.Id == userId), updatedBy), Times.Once);
    }

    [Fact]
    public async Task UpdatePasswordAsync_ShouldHashPasswordAndUpdateSuccessfully()
    {
        // Arrange
        var userId = 1;
        var updatePasswordRequest = new UpdatePasswordRequest
        {
            Password = "newpassword123"
        };
        var updatedBy = "admin";

        _mockUserRepository
            .Setup(x => x.UpdatePasswordAsync(userId, It.IsAny<string>(), updatedBy))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdatePasswordAsync(userId, updatePasswordRequest, updatedBy);

        // Assert
        Assert.True(result);
        _mockUserRepository.Verify(x => x.UpdatePasswordAsync(
            userId,
            It.IsAny<string>(),
            updatedBy
        ), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnTrue_WhenDeleteIsSuccessful()
    {
        // Arrange
        var userId = 1;
        var deletedBy = "admin";

        _mockUserRepository
            .Setup(x => x.DeleteAsync(userId, deletedBy))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync(userId, deletedBy);

        // Assert
        Assert.True(result);
        _mockUserRepository.Verify(x => x.DeleteAsync(userId, deletedBy), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnFalse_WhenDeleteFails()
    {
        // Arrange
        var userId = 999;
        var deletedBy = "admin";

        _mockUserRepository
            .Setup(x => x.DeleteAsync(userId, deletedBy))
            .ReturnsAsync(false);

        // Act
        var result = await _userService.DeleteUserAsync(userId, deletedBy);

        // Assert
        Assert.False(result);
        _mockUserRepository.Verify(x => x.DeleteAsync(userId, deletedBy), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnLoginResponse()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Username = "testuser",
            Password = "password123"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginRequest.Password);
        var user = new User
        {
            Id = 1,
            Username = loginRequest.Username,
            Password = hashedPassword,
            FirstName = "Test",
            LastName = "User",
            RoleId = 1
        };

        var expectedToken = "generated-jwt-token";

        _mockUserRepository
            .Setup(x => x.GetByUsernameAsync(loginRequest.Username))
            .ReturnsAsync(user);

        _mockTokenService
            .Setup(x => x.GenerateToken(user))
            .Returns(expectedToken);

        // Act
        var result = await _userService.LoginAsync(loginRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedToken, result.Token);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.FirstName, result.FirstName);
        Assert.Equal(user.LastName, result.LastName);
        Assert.Equal(user.RoleId, result.RoleId);
        _mockUserRepository.Verify(x => x.GetByUsernameAsync(loginRequest.Username), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(user), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidUsername_ShouldReturnNull()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Username = "invaliduser",
            Password = "password123"
        };

        _mockUserRepository
            .Setup(x => x.GetByUsernameAsync(loginRequest.Username))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.LoginAsync(loginRequest);

        // Assert
        Assert.Null(result);
        _mockUserRepository.Verify(x => x.GetByUsernameAsync(loginRequest.Username), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Username = "testuser",
            Password = "wrongpassword"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new User
        {
            Id = 1,
            Username = loginRequest.Username,
            Password = hashedPassword,
            FirstName = "Test",
            LastName = "User",
            RoleId = 1
        };

        _mockUserRepository
            .Setup(x => x.GetByUsernameAsync(loginRequest.Username))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.LoginAsync(loginRequest);

        // Assert
        Assert.Null(result);
        _mockUserRepository.Verify(x => x.GetByUsernameAsync(loginRequest.Username), Times.Once);
        _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }
}

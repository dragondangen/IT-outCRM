using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using IT_outCRM.Application.Services;
using IT_outCRM.Domain.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace IT_outCRM.Tests.Services;

public class JwtServiceTests
{
    private Mock<IConfiguration> CreateConfig(string? key = null, string? issuer = null, string? audience = null, string? hours = null)
    {
        var config = new Mock<IConfiguration>();
        config.Setup(c => c["Jwt:Key"]).Returns(key);
        config.Setup(c => c["Jwt:Issuer"]).Returns(issuer);
        config.Setup(c => c["Jwt:Audience"]).Returns(audience);
        config.Setup(c => c["Jwt:ExpirationHours"]).Returns(hours);
        return config;
    }

    [Fact]
    public void GenerateToken_WithValidConfig_ReturnsToken()
    {
        var config = CreateConfig(
            key: "ThisIsAVeryLongSecretKeyForTesting1234567890!",
            issuer: "TestIssuer",
            audience: "TestAudience",
            hours: "24");

        var sut = new JwtService(config.Object, NullLogger<JwtService>.Instance);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@test.com",
            Role = "Admin"
        };

        var token = sut.GenerateToken(user);

        token.Should().NotBeNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "testuser");
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == "test@test.com");
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier);
        jwt.Issuer.Should().Be("TestIssuer");
        jwt.Audiences.Should().Contain("TestAudience");
    }

    [Fact]
    public void GenerateToken_WithoutKey_ThrowsInvalidOperationException()
    {
        var config = CreateConfig(key: null);
        var sut = new JwtService(config.Object, NullLogger<JwtService>.Instance);
        var user = new User { Id = Guid.NewGuid(), Username = "test", Email = "t@t.com", Role = "User" };

        var act = () => sut.GenerateToken(user);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*JWT Key*");
    }

    [Fact]
    public void GetUserIdFromToken_WithValidToken_ReturnsUserId()
    {
        var config = CreateConfig(
            key: "ThisIsAVeryLongSecretKeyForTesting1234567890!",
            issuer: "TestIssuer",
            audience: "TestAudience",
            hours: "24");

        var sut = new JwtService(config.Object, NullLogger<JwtService>.Instance);
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Username = "test", Email = "t@t.com", Role = "User" };

        var token = sut.GenerateToken(user);
        var result = sut.GetUserIdFromToken(token);

        result.Should().Be(userId);
    }

    [Fact]
    public void GetUserIdFromToken_WithInvalidToken_ReturnsNull()
    {
        var config = CreateConfig();
        var sut = new JwtService(config.Object, NullLogger<JwtService>.Instance);

        var result = sut.GetUserIdFromToken("not-a-valid-token");

        result.Should().BeNull();
    }

    [Fact]
    public void GetUserIdFromToken_WithEmptyString_ReturnsNull()
    {
        var config = CreateConfig();
        var sut = new JwtService(config.Object, NullLogger<JwtService>.Instance);

        var result = sut.GetUserIdFromToken("");

        result.Should().BeNull();
    }

    [Fact]
    public void GenerateToken_TokenExpiresInConfiguredHours()
    {
        var config = CreateConfig(
            key: "ThisIsAVeryLongSecretKeyForTesting1234567890!",
            issuer: "Test",
            audience: "Test",
            hours: "48");

        var sut = new JwtService(config.Object, NullLogger<JwtService>.Instance);
        var user = new User { Id = Guid.NewGuid(), Username = "u", Email = "e@e.com", Role = "User" };

        var token = sut.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var expectedExpiration = DateTime.UtcNow.AddHours(48);
        jwt.ValidTo.Should().BeCloseTo(expectedExpiration, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void GenerateToken_DefaultExpirationIs24Hours()
    {
        var config = CreateConfig(
            key: "ThisIsAVeryLongSecretKeyForTesting1234567890!",
            issuer: "Test",
            audience: "Test",
            hours: null);

        var sut = new JwtService(config.Object, NullLogger<JwtService>.Instance);
        var user = new User { Id = Guid.NewGuid(), Username = "u", Email = "e@e.com", Role = "User" };

        var token = sut.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var expectedExpiration = DateTime.UtcNow.AddHours(24);
        jwt.ValidTo.Should().BeCloseTo(expectedExpiration, TimeSpan.FromMinutes(1));
    }
}

using FluentAssertions;
using MassTransit;
using Moq;
using FGC.Users.Application.Contracts.Events;
using FGC.Users.Application.DTOs;
using FGC.Users.Application.Services;
using FGC.Users.Domain.Entities;
using FGC.Users.Domain.Repositories;
using FGC.Users.Domain.ValueObjects;

namespace FGC.Users.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _userService = new UserService(_userRepositoryMock.Object, _publishEndpointMock.Object);
    }

    [Fact]
    public async Task Dado_RequestValido_Quando_CreateAsync_Entao_RetornaGuid()
    {
        // Dado
        var request = new CreateUserRequest
        {
            Name = "João Silva",
            Email = "joao@email.com",
            Password = "Senha@123"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        // Quando
        var result = await _userService.CreateAsync(request);

        // Então
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Dado_RequestValido_Quando_CreateAsync_Entao_PublicaUserCreatedEvent()
    {
        // Dado
        var request = new CreateUserRequest
        {
            Name = "Maria Souza",
            Email = "maria@email.com",
            Password = "Senha@123"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        // Quando
        await _userService.CreateAsync(request);

        // Então
        _publishEndpointMock.Verify(
            p => p.Publish(It.Is<UserCreatedEvent>(e =>
                e.UserEmail == request.Email &&
                e.UserName == request.Name),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_EmailJaCadastrado_Quando_CreateAsync_Entao_LancaArgumentException()
    {
        // Dado
        var request = new CreateUserRequest
        {
            Name = "Duplicado",
            Email = "existente@email.com",
            Password = "Senha@123"
        };

        var userExistente = new User("Existente", new Email("existente@email.com"), "hash", Domain.Enums.UserRole.User);
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(userExistente);

        // Quando
        var act = () => _userService.CreateAsync(request);

        // Então
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Email já cadastrado");
    }

    [Fact]
    public async Task Dado_SenhaInvalida_Quando_CreateAsync_Entao_LancaArgumentException()
    {
        // Dado
        var request = new CreateUserRequest
        {
            Name = "Pedro",
            Email = "pedro@email.com",
            Password = "fraca"
        };

        // Quando
        var act = () => _userService.CreateAsync(request);

        // Então
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Senha inválida");
    }

    [Fact]
    public async Task Dado_EmailInvalido_Quando_CreateAsync_Entao_LancaArgumentException()
    {
        // Dado
        var request = new CreateUserRequest
        {
            Name = "Ana",
            Email = "email-invalido",
            Password = "Senha@123"
        };

        // Quando
        var act = () => _userService.CreateAsync(request);

        // Então
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Dado_UsuarioCriado_Quando_CreateAsync_Entao_SalvaNoRepositorio()
    {
        // Dado
        var request = new CreateUserRequest
        {
            Name = "Carlos",
            Email = "carlos@email.com",
            Password = "Senha@123"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        // Quando
        await _userService.CreateAsync(request);

        // Então
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }
}

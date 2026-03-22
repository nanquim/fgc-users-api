using FGC.Users.Domain.Entities;
using FGC.Users.Domain.Enums;
using FGC.Users.Domain.Repositories;
using FGC.Users.Domain.ValueObjects;
using FGC.Users.Application.DTOs;
using FGC.Users.Application.Validators;
using FGC.Users.Application.Security;
using FGC.Users.Application.Contracts.Events;
using MassTransit;

namespace FGC.Users.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UserService(IUserRepository userRepository, IPublishEndpoint publishEndpoint)
    {
        _userRepository = userRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> CreateAsync(CreateUserRequest request)
    {
        var email = new Email(request.Email);

        if (!PasswordValidator.IsValid(request.Password))
            throw new ArgumentException("Senha inválida");

        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new ArgumentException("Email já cadastrado");

        var passwordHash = PasswordHasher.Hash(request.Password);

        var user = new User(request.Name, email, passwordHash, UserRole.User);

        await _userRepository.AddAsync(user);

        await _publishEndpoint.Publish(new UserCreatedEvent(
            user.Id,
            user.Name,
            user.Email.Value,
            DateTime.UtcNow));

        return user.Id;
    }
}

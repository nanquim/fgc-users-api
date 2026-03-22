using FGC.Users.Domain.Entities;
using FGC.Users.Domain.Enums;
using FGC.Users.Domain.Repositories;
using FGC.Users.Domain.ValueObjects;
using FGC.Users.Application.DTOs;
using FGC.Users.Application.Validators;
using FGC.Users.Application.Security;

namespace FGC.Users.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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

        return user.Id;
    }
}

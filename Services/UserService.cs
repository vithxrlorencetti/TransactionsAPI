using System.ComponentModel.DataAnnotations;
using AutoMapper;
using TransactionsAPI.Authentication;
using TransactionsAPI.DTOs;
using TransactionsAPI.Helpers;
using TransactionsAPI.Interfaces;
using TransactionsAPI.Models;

namespace TransactionsAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IViaCepService _viaCepService;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, IViaCepService viaCepService, IMapper mapper)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _viaCepService = viaCepService;
        _mapper = mapper;
    }

    public async Task<UserInfoDTO?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<UserInfoDTO>(user);
    }

    public async Task<PagedResult<UserInfoDTO>> GetAllAsync(int page, int pageSize)
    {
        var pagedUsers = await _userRepository.GetAllAsync(page, pageSize);

        var userDtos = _mapper.Map<List<UserInfoDTO>>(pagedUsers.Items);

        return new PagedResult<UserInfoDTO>
        {
            Items = userDtos,
            TotalCount = pagedUsers.TotalCount,
            Page = pagedUsers.Page,
            PageSize = pagedUsers.PageSize
        };
    }

    public async Task<string> RegisterAsync(string name, string email, string password, string postalCode)
    {
        var emailValidator = new EmailAddressAttribute();
        if (!emailValidator.IsValid(email))
            throw new BadRequestException("Invalid email format");

        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
            throw new ConflictException("Email already in use");

        // Caso o CEP não exista na hora do cadastro do usuário, deverá ser 
        // retornado um erro e a criação do usuário não pode ser realizada no sistema

        var address = await _viaCepService.GetAddressByCepAsync(postalCode) 
            ?? throw new BadRequestException("Invalid postal code");

        var user = new User
        {
            Name = name,
            Email = email,
            PostalCode = postalCode,
            Street = address.Street,
            Complement = address.Complement,
            Neighborhood = address.Neighborhood,
            City = address.City,
            State = address.State,
            Balance = 0,
            PasswordHash = HashPassword(password),
            CreatedAt = DateTime.UtcNow,
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return $"{address.Street}, {address.Neighborhood}, {address.City} - {address.State}";
        
    }

    public async Task<string> DisableByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id)
                   ?? throw new NotFoundException("User not found");

        if (user.DisabledAt != null)
            throw new ConflictException("User is already disabled");

        // No endpoint que exclui um usuário, adicionar a funcionalidade que não será
        // possível excluir um usuário que tenha qualquer tipo de movimentação;

        if (user.SentTransactions.Count > 0 || user.ReceivedTransactions.Count > 0)
            throw new BadRequestException("Can't disable user with transactions");

        user.DisabledAt = DateTime.UtcNow;

        await _userRepository.SaveChangesAsync();

        return user.Name;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email)
                   ?? throw new NotFoundException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new UnauthorizedException("Invalid Credentials");

        if (user.DisabledAt != null)
            throw new ConflictException("User is disabled");

        return _jwtTokenGenerator.GenerateToken(user);
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}

﻿using AutoMapper;
using USSC.Dto;
using USSC.Entities;
using USSC.Helpers;

namespace USSC.Services;

// create Entity and db

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _mapper = mapper;
    }

    // сделать хэш и проверку на хэш, а не просто строка
    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _userRepository
            .GetAll()
            .FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);

        if (user == null)
        {
            // todo: need to add logger
            return null;
        }

        var token = _configuration.GenerateJwtToken(user);

        return new AuthenticateResponse(user, token);
    }

    public async Task<AuthenticateResponse> Register(UserModel userModel)
    {
        userModel.Role = "User";
        userModel.RefreshToken = "1";
        var user = _mapper.Map<UsersEntity>(userModel);

        var addedUser = await _userRepository.Add(user);

        var response = Authenticate(new AuthenticateRequest
        {
            Email = user.Email,
            Password = user.Password
        });
            
        return response;
    }

    public async Task<SuccessResponse> CreateAdmin(string userEmail)
    {
        var user = await _userRepository.GetByUserEmail(userEmail);
        if (user == null)
            return new SuccessResponse(false);
        user.Role = "Admin";
        var id = await _userRepository.Update(user);
        return new SuccessResponse(id == user.Id);
    }

    public async Task<Guid> Update(UserModel entity)
    {
        var a = _mapper.Map<UsersEntity>(entity);
        await _userRepository.Update(a);
        return entity.Id;
    }

    public IEnumerable<UsersEntity> GetAll()
    {
        return _userRepository.GetAll();
    }

    public UsersEntity GetById(Guid id)
    {
        return _userRepository.GetById(id);
    }
}
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shared.Features.Users;
using Shared.Persistence.Core.Features.Documents.Single;
using Shared.Persistence.Core.Features.Exceptions;
using Shared.Persistence.Core.Features.Repository;

namespace Shared.Persistence.Identity.Features.Users.Single;

public class SingleUserService : SingleDocumentService<UserDocument, string, UserDto>, ISingleUserService
{
    private readonly ILogger<SingleUserService> _logger;
    private readonly IMapper                    _mapper;
    private readonly UserManager<UserDocument>  _userManager;

    public SingleUserService(
        IRepository<UserDocument, string> repository,
        UserManager<UserDocument>         userManager,
        IMapper                           mapper,
        ILogger<SingleUserService>        logger) : base(repository, mapper)
    {
        _userManager = userManager;
        _mapper      = mapper;
        _logger      = logger;
    }

    public new async Task UpdateAsync<TUpdateDto>(string id, TUpdateDto dto)
    {
        var updateUserDto = dto as UpdateUserDto;
        _logger.LogTrace("{Action}({Id}, {Dto}", nameof(UpdateAsync), id, dto!);
        var user       = await _userManager.FindByIdAsync(id);
        if (user == null) throw new DocumentNotFoundException($"User with id {id} not found");

        var roles                                                 = updateUserDto!.Roles.ToList();
        if (updateUserDto.UserName is { } userName) user.UserName = userName;
        if (updateUserDto.Email is { } email) user.Email          = email;
        if (roles.Any()) user.Roles                               = roles;

        await _userManager.UpdateSecurityStampAsync(user);
        await _userManager.UpdateAsync(user);
    }

    public new async Task<UserDto> DeleteAsync(string id)
    {
        _logger.LogTrace("{Action}({Id})", nameof(DeleteAsync), id);
        var existing = await _userManager.FindByIdAsync(id);
        if (existing == null) throw new DocumentNotFoundException($"User with id {id} not found");
        await _userManager.DeleteAsync(existing);
        return _mapper.Map<UserDto>(existing);
    }

    public new Task<UserDto> CreateAsync<TCreateDocDto>(TCreateDocDto request) =>
        CreateAsync(request as CreateUserDto ??
                    throw new InvalidOperationException($"Parameter must be of type {typeof(TCreateDocDto).Name}"));
    public async Task<UserDto> CreateAsync(CreateUserDto request)
    {
        _logger.LogTrace("{Action}({Request})", nameof(CreateAsync), request);
        var doc = _mapper.Map<UserDocument>(request);

        var existing = await _userManager.FindByNameAsync(request.UserName);
        if (existing is { } user) return _mapper.Map<UserDto>(user);

        var result = await _userManager.CreateAsync(doc, request.Password);
        if (!result.Succeeded) throw new CreateUserException(result.Errors.Select(e => e.Description).ToList());

        doc = await _userManager.FindByNameAsync(doc.UserName);
        return _mapper.Map<UserDto>(doc);
    }

    public new Task<TField?> GetFieldAsync<TField>(
        Expression<Func<UserDocument, bool>>   predicate,
        Expression<Func<UserDocument, TField>> field)
    {
        _logger.LogTrace("{Action}", nameof(GetFieldAsync));
        return Task.FromResult(_userManager.Users.Where(predicate).Select(field).FirstOrDefault());
    }

    public new async Task<UserDto?> GetByIdAsync(string id)
    {
        _logger.LogTrace("{Action}({Id})", nameof(GetByIdAsync), id);
        var doc = await _userManager.FindByIdAsync(id);
        return doc is null ? default : _mapper.Map<UserDto>(doc);
    }

    public new Task<UserDto?> GetByPredicateAsync(Expression<Func<UserDocument, bool>> predicate)
    {
        _logger.LogTrace("{Action}", nameof(GetByPredicateAsync));
        var doc = _userManager.Users.SingleOrDefault(predicate);
        return doc is null ? Task.FromResult<UserDto?>(null) : Task.FromResult(_mapper.Map<UserDto>(doc));
    }

    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        _logger.LogTrace("{Action}({Username})", nameof(GetByUsernameAsync), username);
        var doc = await _userManager.FindByNameAsync(username);
        return doc is null ? null : _mapper.Map<UserDto>(doc);
    }

    
}
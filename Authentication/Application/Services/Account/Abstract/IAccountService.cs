using Authentication.Application.Users.Dtos;
using Shared.Results;

namespace Authentication.Application.Services.Account.Abstract
{
    public interface IAccountService
    {
        Task<Result<UserDto>> LoginAsync(string username, string password);
    }
}


using Authentication.Application.User.Dto;
using Shared.Results;

namespace Authentication.Services.Account.Abstract
{
    public interface IAccountService
    {
        Task<Result<UserDto>> LoginAsync(string username, string password);
    }
}

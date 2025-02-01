
using Authentication.Application.User.Dto;

namespace Authentication.Services.Account.Abstract
{
    public interface IAccountService
    {
        Task<UserDto> LoginAsync(string username, string password);
    }
}

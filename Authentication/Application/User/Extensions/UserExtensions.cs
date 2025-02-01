using Authentication.Application.User.Dto;

namespace Authentication.Application.User.Extensions
{
    public static class UserExtensions
    {
        public static UserDto ToDto(this Domain.User user) => new(
            user.Id,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Gender,
            user.Image,
            user.AccessToken,
            user.RefreshToken
        );
    }

}

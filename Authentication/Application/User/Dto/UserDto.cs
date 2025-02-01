namespace Authentication.Application.User.Dto
{
    public record UserDto(
        int Id,
        string Username,
        string Email,
        string FirstName,
        string LastName,
        string Gender,
        string Image,
        string AccessToken,
        string RefreshToken
    );


}

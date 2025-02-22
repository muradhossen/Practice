namespace Authentication.Application.Users.Dtos
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

using Authentication.Application.Services.Account.Abstract;
using Authentication.Application.Users.Dtos;
using Authentication.Application.User.Extensions;
using Shared.Results;
using System.Text;
using System.Text.Json;

namespace Authentication.Application.Services.Account;

public class AccountService : IAccountService
{
    private readonly HttpClient _httpClient;

    public AccountService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<Result<UserDto>> LoginAsync(string username, string password)
    {

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return Result<UserDto>.Failure(["Username and password required"]);
        }

        string endpoint = "https://dummyjson.com/auth/login";

        var loginData = new
        {
            Username = username,
            Password = password
        };

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var requestBody = JsonSerializer.Serialize(loginData, options);


        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(endpoint, content);
        string responseJson = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var user = JsonSerializer.Deserialize<Domain.User>(responseJson, options);

            return Result<UserDto>.Success(user.ToDto());
        }

        return (Result<UserDto>)Result.Failure([responseJson]);

    }
}

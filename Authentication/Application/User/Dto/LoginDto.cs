using System.Text.Json.Serialization;

namespace Authentication.Application.User.Dto
{
    public class LoginDto
    { 
        public string? Username { get; set; } 
        public string? Password { get; set; }
    }
}

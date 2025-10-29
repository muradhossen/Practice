using Authentication.Application.Services.Account;
using Authentication.Application.Services.Account.Abstract;
using Authentication.Application.Users.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Shared.Results;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My Microservice API", Version = "v1" });
});
builder.Services.AddOpenApi();

builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddHttpClient<AccountService>(client =>
{
    client.BaseAddress = new Uri("https://dummyjson.com");
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});


builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests; 

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            IsSuccess = false,
            Errors = "Rate limit exceeded. Try again later.",
        }, token);
    };

    // Sliding window limiter
    options.AddPolicy("IpSlidingPolicy", context =>
    {
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                 ?? context.Connection.RemoteIpAddress?.ToString()
                 ?? "unknown";

        return RateLimitPartition.GetSlidingWindowLimiter(
            ip,
            _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,
                QueueLimit = 0
            });
    });
});

var app = builder.Build();


//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Microservice API v1");
    });
    app.UseDeveloperExceptionPage();
//}

app.UseHttpsRedirection();
app.UseRouting();

app.UseRateLimiter();

app.MapGet("/auth/hello", () =>
{

    return Results.Ok("Hello from auth");
})
.WithName("GetAutHello");

app.MapPost("/auth/login", async ([FromBody] LoginDto loginDto, [FromServices] IAccountService accountService) =>
{
    if (loginDto == null)
    {
        return Results.BadRequest(Result<UserDto>.Failure(["Invalid username or password"]));
    }

    var user = await accountService.LoginAsync(loginDto.Username, loginDto.Password);
    return Results.Ok(user);

}).RequireRateLimiting("IpSlidingPolicy");

app.MapPost("/", async (HttpRequest request) =>
{
    try
    {
        var person = await request.ReadFromJsonAsync<Person>();

        return Results.Ok(person);

    }
    catch (Exception ex)
    {

        throw;
    }
}); 

app.Run();


record Person(string Name, int Age);

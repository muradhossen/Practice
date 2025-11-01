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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Authentication API", Version = "v1" });
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
    // Configure the rejection response for ALL rate-limited requests
    options.OnRejected = async (context, token) =>
    {
        // Get the time until the limit resets (the countdown)
        var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var value)
            ? (TimeSpan?)value
            : null;



        // Check if RetryAfter has a value
        if (retryAfter.HasValue)
        {
            var seconds = (int)retryAfter.Value.TotalSeconds;

            // Set the response status code and content
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = "text/plain";
            await context.HttpContext.Response.WriteAsync(
                $"API limit exceed, try after {seconds} second{(seconds != 1 ? "s" : "")}.",
                cancellationToken: token);
        }
        else
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = "text/plain";
            await context.HttpContext.Response.WriteAsync(
                "API limit exceed. Please try again later.",
                cancellationToken: token);
        }
    };

    // 2. Define the IP-Based, Fixed Window Policy
    options.AddPolicy("IpLoginLimitPolicy", httpContext =>
    {
        // Get the IP address for partitioning
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Create a Fixed Window Limiter for this IP
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ipAddress, // The partition key is the IP
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10, // Max 10 requests
                Window = TimeSpan.FromMinutes(1), // within 1 minute
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0 // No queueing, reject immediately
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
    var instance = Environment.GetEnvironmentVariable("INSTANCE_NAME") ?? "Unknown";

    return Results.Ok($"Hello from auth : {instance}");
})
.WithName("GetAutHello");
//.RequireRateLimiting("IpLoginLimitPolicy");

app.MapPost("/auth/login", async ([FromBody] LoginDto loginDto, [FromServices] IAccountService accountService) =>
{
    if (loginDto == null)
    {
        return Results.BadRequest(Result<UserDto>.Failure(["Invalid username or password"]));
    }

    var user = await accountService.LoginAsync(loginDto.Username, loginDto.Password);
    return Results.Ok(user);

}).RequireRateLimiting("IpLoginLimitPolicy");

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

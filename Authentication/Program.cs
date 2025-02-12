using Authentication.Application.User.Dto;
using Authentication.Services.Account;
using Authentication.Services.Account.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

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

app.MapGet("/auth/hello", () =>
{

    return Results.Ok("Hello from auth");
})
.WithName("GetAutHello");

app.MapPost("/auth/login", async ([FromBody] LoginDto loginDto, [FromServices] IAccountService accountService) =>
{
    if (loginDto == null)
    {
        return Results.BadRequest("Invalid login data.");
    }

    var user = await accountService.LoginAsync(loginDto.Username, loginDto.Password);
    return Results.Ok(user);

});

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

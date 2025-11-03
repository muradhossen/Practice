using Catalog.Application.Services.Categories;
using Catalog.Application.Services.Categories.Abstraction;
using Microsoft.AspNetCore.Mvc;
using NRedisStack.RedisStackCommands;
using Shared.Results;
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryCachingService, CategoryCachingService>();

builder.Services.AddHttpClient<CategoryService>(client =>
{
    client.BaseAddress = new Uri("https://dummyjson.com");
});

#region Configure Redis
var redisConnection = builder.Configuration.GetConnectionString("Redis");
var redis = ConnectionMultiplexer.Connect(redisConnection);
builder.Services.AddSingleton(redis);
#endregion

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.MapGet("/catalog/categories", async ([FromServices] ICategoryCachingService categoryService) =>
{
    try
    {
        return await categoryService.GetCategoriesAsync();
    }
    catch (Exception ex)
    {
        return Result.Failure([ex.Message]);
    }
})
.WithName("GetCategories")
.WithOpenApi();

app.MapGet("/catalog/test", async ([FromServices] ICategoryCachingService categoryService) =>
{
    string environment = Environment.GetEnvironmentVariable("INSTANCE_NAME");

     return Results.Ok($"Catalog Service {environment} is up and running");
})
.WithName("TestCatalog")
.WithOpenApi();

app.MapGet("/catalog/test-redis", async ([FromServices] ICategoryService categoryService) =>
{
    try
    {
        var data = await run();

        return Result<string>.Success(data);
    }
    catch (Exception ex)
    {
        return Result.Failure([ex.Message]);
    }
})
.WithName("TestRedis")
.WithOpenApi();

app.Run();



async Task<string> run()
{
    var db = redis.GetDatabase();


    var user1 = new
    {
        name = "Paul John",
        email = "paul.john@example.com",
        age = 42,
        city = "London"
    };

    if (await db.HashExistsAsync("categories", "all"))
    {
        return await db.HashGetAsync("categories", "all");
    }

    db.HashSet("categories",
       [
            new("all", JsonSerializer.Serialize(user1)),
       ]);

    return await db.HashGetAsync("categories", "all");




}


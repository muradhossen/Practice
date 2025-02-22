using Catalog.Application.Categories.Dtos;
using Catalog.Application.Services.Categories;
using Catalog.Application.Services.Categories.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddHttpClient<CategoryService>(client =>
{
    client.BaseAddress = new Uri("https://dummyjson.com");
});

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.MapGet("/catalog/categories", async ([FromServices] ICategoryService categoryService) =>
{ 
	try
	{ 
       return await categoryService.GetCategories(); 
    }
	catch (Exception ex)
	{
        return Result.Failure(new[] { ex.Message }); 
	} 
})
.WithName("GetCategories")
.WithOpenApi();

app.Run();


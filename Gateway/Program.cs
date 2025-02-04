using Gateway.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region Ocelot configuration

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("Ocelot.Development.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);
}


builder.Services.AddOcelot(builder.Configuration);

#endregion

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
//app.UseMiddleware<ResponseMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await app.UseOcelot();

app.Run();


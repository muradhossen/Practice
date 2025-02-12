using Gateway.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;


var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
//app.UseMiddleware<ResponseMiddleware>();

//if (app.Environment.IsDevelopment())
//{
app.MapOpenApi();
app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
});
//}

app.UseHttpsRedirection();

await app.UseOcelot();

app.Run();


using Auth_Server;
using Auth_Server.Repository;
using Auth_Server.Services;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

// Add services to the container.
builder.Services.Configure<DBConfig>(configuration.GetSection(nameof(DBConfig)));

builder.Services.AddTransient<IAccountDB, AccountDB>();
builder.Services.AddTransient<IAuthService, AuthService>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Run();

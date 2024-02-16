using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using API_Game_Server;
using API_Game_Server.Repository;
using API_Game_Server.Services;
using StackExchange.Redis;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

// DB 연결 설정을 종속성 주입으로 넣어주기 위함
builder.Services.Configure<DBConfig>(configuration.GetSection(nameof(DBConfig)));

// Add services to the container.
builder.Services.AddTransient<AccountDB>();
builder.Services.AddTransient<GameDB>();
// services 종속성 주입 추가
builder.Services.AddTransient<ValidationService>();
builder.Services.AddTransient<FriendRequestService>();
builder.Services.AddTransient<GameResultService>();
builder.Services.AddTransient<AttendanceService>();
builder.Services.AddTransient<RankService>();
// Add services about redis
builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));
builder.Services.AddScoped<RedisDB>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

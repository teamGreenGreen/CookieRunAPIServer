using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using API_Game_Server;
using API_Game_Server.Repository;
using API_Game_Server.Services;
using StackExchange.Redis;
using System.Net;
using API_Game_Server.Repository.Interface;
using API_Game_Server.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

// DB 연결 설정을 종속성 주입으로 넣어주기 위함
builder.Services.Configure<DBConfig>(configuration.GetSection(nameof(DBConfig)));

// Add services to the container.
builder.Services.AddTransient<IGameDB, GameDB>();
// services 종속성 주입 추가
builder.Services.AddTransient<IValidationService, ValidationService>();
builder.Services.AddTransient<IFriendRequestService, FriendRequestService>();
builder.Services.AddTransient<IFriendRequestAcceptService, FriendRequestAcceptService>();
builder.Services.AddTransient<IFriendRequestListService, FriendRequestListService>();
builder.Services.AddTransient<IFriendRequestDenyService, FriendRequestDenyService>();
builder.Services.AddTransient<IFriendListService, FriendListService>();
builder.Services.AddTransient<IFriendDeleteService, FriendDeleteService>();
builder.Services.AddTransient<IGameResultService, GameResultService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IAttendanceService, AttendanceService>();
builder.Services.AddTransient<IRankService, RankService>();
// Add services about redis
builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));
builder.Services.AddTransient<IRedisDB, RedisDB>();
builder.Services.AddControllers();

var app = builder.Build();

// 미들웨어 추가
app.UseMiddleware<API_Game_Server.Middleware.VerifyUserAuth>();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Controllers;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using API_Game_Server;
using API_Game_Server.Repository;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

// DB ���� ������ ���Ӽ� �������� �־��ֱ� ����
builder.Services.Configure<DBConfig>(configuration.GetSection(nameof(DBConfig)));

// Add services to the container.
builder.Services.AddTransient<AccountDB>();
// Add services about redis
builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));
builder.Services.AddScoped<RedisDB>();
builder.Services.AddControllers();
builder.Services.AddScoped<QueryFactory>(provider => {
    return new QueryFactory(Database.GetMySqlConnetion().Result, new MySqlCompiler());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// DB �ʱ�ȭ (DB�� ������ �� ����ϴ� mysql connection string�� ����)
Database.Init(app.Configuration);

app.Run();

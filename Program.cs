using Controllers;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using Web_API_Server;
using Web_API_Server.Repository;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

// DB 연결 설정을 종속성 주입으로 넣어주기 위함
builder.Services.Configure<DBConfig>(configuration.GetSection(nameof(DBConfig)));

// Add services to the container.
builder.Services.AddTransient<AccountDB>();
builder.Services.AddControllers();
builder.Services.AddScoped<QueryFactory>(provider => {
    return new QueryFactory(Database.GetMySqlConnetion().Result, new MySqlCompiler());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// DB 초기화 (DB에 연결할 때 사용하는 mysql connection string을 설정)
Database.Init(app.Configuration);

app.Run();

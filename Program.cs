using Controllers;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using Web_API_Server;
using Web_API_Server.Repository;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

// DB ���� ������ ���Ӽ� �������� �־��ֱ� ����
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

// DB �ʱ�ȭ (DB�� ������ �� ����ϴ� mysql connection string�� ����)
Database.Init(app.Configuration);

app.Run();

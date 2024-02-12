using Controllers;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

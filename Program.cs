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

// DB �ʱ�ȭ (DB�� ������ �� ����ϴ� mysql connection string�� ����)
Database.Init(app.Configuration);

app.Run();

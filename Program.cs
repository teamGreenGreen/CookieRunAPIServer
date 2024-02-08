var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// DB �ʱ�ȭ (DB�� ������ �� ����ϴ� mysql connection string�� ����)
Database.Init(app.Configuration);

app.Run();

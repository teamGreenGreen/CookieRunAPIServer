var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// DB 초기화 (DB에 연결할 때 사용하는 mysql connection string을 설정)
Database.Init(app.Configuration);

app.Run();

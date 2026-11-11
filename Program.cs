using Microsoft.EntityFrameworkCore;
using QuestLog.Data;

using QuestLog.Filters;
using QuestLog.Repositories;
using QuestLog.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(options => options.Filters.Add<BusinessExceptionFilter>());
builder.Services.AddScoped<IMisionRepository, MisionSqlRepository>();
builder.Services.AddScoped<IMisionService, MisionService>();
builder.Services.AddProblemDetails();
//Configuracion del SQL SERVER
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("SqlServerConnection")
    )
);

var app = builder.Build();
app.UseExceptionHandler();
app.MapControllers();
app.Run();


//ORM
//dotnet add package Microsoft.EntityFrameworkCore.SqlServer
//dotnet add package Microsoft.EntityFrameworkCore.Design
//dotnet tool install --global dotnet-ef --version 10.0.*
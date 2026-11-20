using Microsoft.EntityFrameworkCore;
using QuestLog.Data;

using QuestLog.Filters;
using QuestLog.Repositories;
using QuestLog.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(options => options.Filters.Add<BusinessExceptionFilter>());
builder.Services.AddScoped<IMisionRepository, MisionSqlRepository>();
builder.Services.AddScoped<IMisionService, MisionService>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Configuracion del SQL SERVER
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("SqlServerConnection")
    )
);

// Configuracion del JWT
var jwt = builder.Configuration.GetSection("Jwt");
var key = Convert.FromBase64String(
   jwt["Key"] ?? throw new InvalidOperationException(
       "Falta configurar Jwt:Key"));
builder.Services
   .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters =
           new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidIssuer = jwt["Issuer"],
               ValidateAudience = true,
               ValidAudience = jwt["Audience"],
               ValidateLifetime = true,
               ClockSkew = TimeSpan.Zero,
               ValidateIssuerSigningKey = true,
               IssuerSigningKey =
                   new SymmetricSecurityKey(key)
           };
   });

//Configurar politica de cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("QuestLogFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5500",
                "http://127.0.0.1:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddAuthorization();

// CORS: el frontend (Live Server en el puerto 5500) es un origen distinto al de la API (5050).
// Sin esta política el navegador bloquea el preflight (OPTIONS) y el login nunca se envía.
// Se incluyen 127.0.0.1 y localhost porque el navegador los trata como orígenes diferentes.
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy => policy
    .WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
    .AllowAnyHeader()   // permite Content-Type y Authorization (el token Bearer)
    .AllowAnyMethod())); // permite GET, POST, PUT, DELETE y OPTIONS

var app = builder.Build();
app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("QuestLogFronted");
// UseCors debe ir antes de autenticación/autorización para que el preflight
// (que no lleva token) no sea rechazado con 401.
app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();


//ORM
//dotnet add package Microsoft.EntityFrameworkCore.SqlServer
//dotnet add package Microsoft.EntityFrameworkCore.Design
//dotnet tool install --global dotnet-ef --version 10.0.*

//dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version "10.0.*"       

//Si devuelve 500 al consultar 
//Start-Service 'MSSQL$SQLEXPRESS'
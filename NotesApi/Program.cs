using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;
using NotesApi.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key required. Set in appsettings or .env (Jwt__Key).");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Notes API", Version = "v1" });
});
var dbProvider = (builder.Configuration["Database:Provider"] ?? "PostgreSQL").Trim();
if (string.Equals(dbProvider, "SqlServer", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<INoteRepository, NoteRepositorySqlServer>();
    builder.Services.AddScoped<IUserRepository, UserRepositorySqlServer>();
}
else
{
    builder.Services.AddScoped<INoteRepository, NoteRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
}
builder.Services.AddSingleton<JwtService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "https://rithyvong22.github.io")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

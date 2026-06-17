using apiTempo.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var compilador = WebApplication.CreateBuilder(args);
compilador.Services.AddControllers();
compilador.Services.AddEndpointsApiExplorer();
compilador.Services.AddSwaggerGen();
compilador.Services.AddHttpClient<ClimaService>();
compilador.Services.AddScoped<FavoritoService>();

compilador.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(compilador.Configuration.GetConnectionString("DefaultConnection")));

var app = compilador.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Conf. de autenticação JWT
var jwtKey = compilador.Configuration["Jwt:Key"];
compilador.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
}).AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = compilador.Configuration["Jwt:Issuer"],
        ValidAudience = compilador.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

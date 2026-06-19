using apiTempo.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var compilador = WebApplication.CreateBuilder(args);
compilador.Services.AddControllers();
compilador.Services.AddEndpointsApiExplorer();
compilador.Services.AddHttpClient<ClimaService>();
compilador.Services.AddScoped<FavoritoService>();

    var jwtKey = compilador.Configuration["Jwt:Key"];
    compilador.Services.AddSwaggerGen(c =>
    {
      c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
      {
          Description = "JWT Authorization header usando Bearer. Ex: \"Bearer {token}\"",
          Name = "Authorization",
          In = Microsoft.OpenApi.Models.ParameterLocation.Header,
          Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
          Scheme = "bearer",
          BearerFormat = "JWT"
      });

      c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
      {
          {
              new Microsoft.OpenApi.Models.OpenApiSecurityScheme
              {
                  Reference = new Microsoft.OpenApi.Models.OpenApiReference
                  {
                      Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                      Id = "Bearer"
                  }
              },
              Array.Empty<string>()
          }
      });
   });

    compilador.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
   {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });
    
    compilador.Services.AddCors(options =>
    {
        options.AddPolicy("AngularApp", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
    
    compilador.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(compilador.Configuration.GetConnectionString("DefaultConnection")));

var app = compilador.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.UseCors("AngularApp");
app.Run();

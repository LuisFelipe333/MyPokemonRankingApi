
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyPokemonRankingApi.Data;
using MyPokemonRankingApi.Interfaces;
using MyPokemonRankingApi.Repositories;
using MyPokemonRankingApi.Services;
using System.Text;

namespace MyPokemonRankingApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:4200"; //Se obtiene el url del front y se coloca el local host 4200 por defecto

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins(frontendUrl) 
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<PokemonDbContext>(options => //Se crea el dbcontext y se le pasa la cadena de conexión que está en appsettings.json
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // Configuración de las políticas de contraseña para que sean más simples y fáciles de usar.
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<PokemonDbContext>() // Conectado a tu contexto general
            .AddDefaultTokenProviders();

            var jwtSecret = "ClaveMuySecretaYMuyLargaDeMasDe32CaracteresParaElRankingPokemon2026";
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            builder.Services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Cambiar a true en producción
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });


            builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddHttpClient<IPokemonService, PokemonService>(); //Registramos el servicio de PokemonService para que pueda ser inyectado en los controladores y otros servicios. 

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAngular"); //Usar regla para permitir Angular

            app.UseAuthentication(); //Se identifica al usuario antes de autorizarlo
            app.UseAuthorization(); //Se autoriza al usuario para acceder a los recursos


            app.MapControllers();

            app.Run();
        }
    }
}

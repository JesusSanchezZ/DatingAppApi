using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using API.Helpers;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config){
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));             // Configuracion de Cloudinary
            services.AddScoped<ITokenService, TokenService>();                                           // Este servicio después de ejecutarse tarda un tiempo en destruirse, se utiliza en peticiones http
            services.AddScoped<IPhotoService, PhotoService>();                                           // Servicio Cloudinary
            services.AddScoped<IUserRepository, UserRepository>();                                       // Uso del repositorio
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddDbContext<DataContext>(options => {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}
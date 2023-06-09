using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configuration;
using SharedLibrary.Extensions;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UdemyAuthServer.Core.Configuration;
using UdemyAuthServer.Core.Models;
using UdemyAuthServer.Core.Repositories;
using UdemyAuthServer.Core.Services;
using UdemyAuthServer.Core.UnitOfWork;
using UdemyAuthServer.Data;
using UdemyAuthServer.Data.Repositories;
using UdemyAuthServer.Service;
using UdemyAuthServer.Service.Services;

namespace UdemyAuthServer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // DI Register

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService,TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            // araya virg�l koymam�z�n sebebi 2 tane generic type alamas�
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"), sqlOptions =>
                {
                    //UdemyAuthServer.Data katman�nda migration i�lemi yap�caz o y�zden bu ismi kulland�k
                    sqlOptions.MigrationsAssembly("UdemyAuthServer.Data");
                });
            });

            services.AddIdentity<UserApp, IdentityRole>(Opt =>
            {
                Opt.User.RequireUniqueEmail = true;
                Opt.Password.RequireNonAlphanumeric = false;

            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            // �ifre s�rf�rlama i�lemlerinde default token sa�lamak i�in .AddDefaultTokenProviders ekledik.


            // CustomTokenOption TokenOptions daki parametreleri doldurup bize bir nesne �rne�i verecek.

            services.Configure<CustomTokenOption>(Configuration.GetSection("TokenOption"));           

            // Client Clients deki parametreleri doldurup bize bir nesne �rne�i verecek.
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));

            services.AddAuthentication(options =>
            {
                // Bayiler ve normal m��teriler i�in ayr� �yelik sistemi olabilir buna '�ema' diyoruz.

                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey=true,
                    ValidateAudience=true,
                    ValidateIssuer=true,
                    ValidateLifetime=true,
                    ClockSkew=TimeSpan.Zero
                };
            });

            

            services.AddControllers().AddFluentValidation(options=> 
            {
                // AbstractValidator nesnesini miras alan assembly leri bul.
                options.RegisterValidatorsFromAssemblyContaining<Startup>();
            
            });

            services.UseCustomValidationResponse();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UdemyAuthServer.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UdemyAuthServer.API v1"));
            }

            // �lk �nce exception metoda girsin
            app.UseCustomException();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

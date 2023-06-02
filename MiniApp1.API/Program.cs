using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniApp1.API.Requirements;
using SharedLibrary.Configuration;
using SharedLibrary.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOption"));
var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>();

builder.Services.AddCustomTokenAuth(tokenOptions);

// BirthdayRequirementHandler sýnýfýndan nesne türet
// veritabaný veya dinamik deðilse singleton kullanmak mantýklý
builder.Services.AddSingleton<IAuthorizationHandler,BirthDayRequirementHandler>();

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("AnkaraPolicy", policy =>
    {
        policy.RequireClaim("city", "ankara");
    });

    opts.AddPolicy("AgePolicy", policy =>
    {
        policy.Requirements.Add(new BirthDayRequirement(18));
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

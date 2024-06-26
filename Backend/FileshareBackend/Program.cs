using DAL.Context;
using FileshareBackend;
using FileshareBackend.Models.Settings;
using FileshareBackend.Services;
using FileshareBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<GeneralSettings>(builder.Configuration.GetSection("GeneralSettings"));
builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection("FrontendSettings"));

var config = builder.Configuration;
Utils.InitiateMeta(config);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        x =>
        {
            x.WithOrigins("http://localhost:5173")
                .WithMethods("GET","POST","PUT","DELETE","OPTIONS")
                .AllowAnyHeader()
                .AllowCredentials();
        });
});
// Add services to the container.
/*

*/


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IFileSystemService, FileSystemService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddDbContext<FileShareContext>(o=>
    o.UseSqlite(builder.Configuration.GetValue<string>("GeneralSettings:DatabaseConnectionString"))
);

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = int.MaxValue;
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});


builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();
/*
app.Use((context, next) =>
{
    context.Response.Headers["Access-Control-Allow-Origin"] = "http://localhost:5173";
    context.Response.Headers["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS";
    context.Response.Headers["Access-Control-Allow-Headers"] = "*";
    return next();
});
*/

app.MapControllers();

app.Run();
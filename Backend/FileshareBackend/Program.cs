using DAL.Context;
using FileshareBackend.Services;
using FileshareBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IFileSystemService, FileSystemService>();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddDbContext<MyDbContext>(o=>
    o.UseSqlite(builder.Configuration.GetValue<string>("Database_ConnectionString"))
);


var app = builder.Build();


app.Use((context, next) =>
{
    context.Response.Headers["Access-Control-Allow-Origin"] = "http://localhost:5173";
    context.Response.Headers["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE";
    context.Response.Headers["Access-Control-Allow-Headers"] = "*";
    return next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
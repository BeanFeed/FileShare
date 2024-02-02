using DAL.Context;
using FileshareBackend.Services;
using FileshareBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        x =>
        {
            x.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
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

app.UseCors();


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
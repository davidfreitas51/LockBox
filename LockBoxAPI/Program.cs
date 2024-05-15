using LockBoxAPI.Application.Services;
using LockBoxAPI.Repository;
using LockBoxAPI.Repository.Contracts;
using LockBoxAPI.Repository.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRegisteredAccountRepository, RegisteredAccountRepository>();
builder.Services.AddScoped<VerificationEmailService>();

builder.Services.AddDbContext<LockBoxContext>(opt =>
opt.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LockBoxDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));

var app = builder.Build();

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

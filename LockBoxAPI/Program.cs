using LockBox.Commons.Services;
using LockBox.Models;
using LockBoxAPI.Repository;
using LockBoxAPI.Repository.Contracts;
using LockBoxAPI.Repository.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRegisteredAccountRepository, RegisteredAccountRepository>();
builder.Services.AddScoped<VerificationEmailService>();
builder.Services.AddScoped<SecurityHandler>();

builder.Services.AddDbContext<LockBoxContext>(opt =>
    opt.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LockBoxDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));

// Adiciona Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<LockBoxContext>()
    .AddDefaultTokenProviders();

// Configura opções de senha (você pode ajustar conforme necessário)
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredUniqueChars = 1;
    options.Password.RequiredLength = 6;
});

// Configura autenticação com cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Adicione esta linha para garantir que a autenticação seja usada
app.UseAuthorization();

app.MapControllers();

app.Run();

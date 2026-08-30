using Api.Models;
using Application.Commands.Authentication.Login;
using Application.Commands.Authentication.Register;
using Application.Commands.Tasks.CancelTask;
using Application.Commands.Tasks.CompleteTask;
using Application.Commands.Tasks.CreateTask;
using Application.Commands.Tasks.DeleteTask;
using Application.Commands.Tasks.StartTask;
using Application.Commands.Tasks.UpdateTask;
using Application.Interfaces.Auth;
using Application.Interfaces.Persistance;
using Infrastructure.Authentication;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "DbConnection")));
builder.Services.AddScoped<IApplicationDbContext>(
    provider =>
        provider.GetRequiredService<ApplicationDbContext>());

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager();

builder.Services
    .AddAuthentication(
        options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!))
            };
    });

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            Assembly.GetExecutingAssembly(),
            typeof(LoginCommand).Assembly,
            typeof(RegisterUserCommand).Assembly,
            typeof(CreateTaskCommand).Assembly,
            typeof(CancelTaskCommand).Assembly,
            typeof(CompleteTaskCommand).Assembly,
            typeof(DeleteTaskCommand).Assembly,
            typeof(StartTaskCommand).Assembly,
            typeof(UpdateTaskCommand).Assembly

        ));

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<
    ITaskRepository,
    TaskRepository>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ICurrentUser,
    CurrentUser>();

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("localhost", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

var app = builder.Build();
app.UseCors("localhost");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

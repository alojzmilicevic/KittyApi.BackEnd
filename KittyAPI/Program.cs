global using KittyAPI.Data;
global using Microsoft.EntityFrameworkCore;
using KittyApi.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Net;
using KittyAPI.Services;
using KittyAPI.Hubs;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using KittyAPI.Errors;
using KittyAPI.Settings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
AddServices(builder);
builder.Services.ConfigureAuth(builder.Configuration);

var app = builder.Build();
ConfigureMiddleware(app);
SeedData(app);

app.Run();

static void AddServices(WebApplicationBuilder builder)
{
    var services = builder.Services;

    services.AddSignalR();
    services.AddControllers();
    services.AddDbContext<DataContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(option =>
    {
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer",
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer",
                }
            },
            Array.Empty<string>()
        }
    });
    });

    builder.Services.AddRouting(options => options.LowercaseUrls = true);
}

static void ConfigureMiddleware(WebApplication app)
{
    app.UseStaticFiles();
    app.UseCors("ClientPermission");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "KittyAPI V1");
        c.RoutePrefix = string.Empty;
        DateTime currentTime = DateTime.Now;
        DateTime tenPM = new(currentTime.Year, currentTime.Month, currentTime.Day, 22, 0, 0);

        if(currentTime >= tenPM)
        {
            c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
        }
    });
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHub<ChatHub>("/chatHub");
    app.UseExceptionHandler("/error");
}

static void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using var scope = scopedFactory.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.Migrate();
    
    var service = scope.ServiceProvider.GetService<DataSeeder>();
    service.Seed();
}

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddTransient<DataSeeder>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IStreamService, StreamService>();
        services.AddTransient<IHubService, HubService>();
        services.AddSingleton<ProblemDetailsFactory, CustomProblemDetailsFactory>();

        return services;
    }

    public static IServiceCollection ConfigureAuth(this IServiceCollection services, ConfigurationManager configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        services.AddSingleton(Options.Create(jwtSettings));

        services.AddCors(options =>
        {
            List<string> allowedOrigins = GetAllowedOrigins();

            options.AddPolicy("ClientPermission", policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["token"];

                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/chathub")))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    static List<string> GetAllowedOrigins()
    {
        List<string> allowedClients = new() { "localhost", "192.168.50.115", "192.168.50.71" };
        List<string> allowedOrigins = new() { };

        allowedClients.ForEach(client =>
        {
            allowedOrigins.Add($"http://{client}:3000");
            allowedOrigins.Add($"https://{client}:3000");
        });

        return allowedOrigins;
    }
}

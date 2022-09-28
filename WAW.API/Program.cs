using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WAW.API.Shared.Extensions;
using WAW.API.Shared.Injection;
using WAW.API.Shared.Mapping;
using WAW.API.Shared.Persistence.Contexts;
using Serilog;
using WAW.API.Auth.Authorization.Settings;
using WAW.API.Auth.Extensions;

Log.Logger = new LoggerConfiguration().Enrich.FromLogContext()
  .WriteTo.Console(outputTemplate: "[{Timestamp:u} {Level}]: {Message:l}{NewLine}{Exception}")
  .CreateLogger();

Log.Information("Starting up...");

var environment = string.Empty;

try {
  var builder = WebApplication.CreateBuilder(args);

  environment = builder.Environment.EnvironmentName;
  builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

  // Add services to the container with path prefix
  builder.Services.AddControllers(options => options.UseGeneralRoutePrefix("api/v1"));

  // Security configuration 
  builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("Authentication"));

  // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen(
    options => {
      options.EnableAnnotations();
      options.SwaggerDoc(
        "v1",
        new OpenApiInfo {
          Title = "WAW (We Are Working) API",
          Version = "v1",
          Description = "An ASP.NET Core Web API for managing job offers and job applications",
          TermsOfService = new Uri("https://example.com/terms"),
          Contact = new OpenApiContact {Name = "Example Contact", Url = new Uri("https://example.com/contact"),},
          License = new OpenApiLicense {Name = "MIT", Url = new Uri("https://choosealicense.com/licenses/mit/"),},
        }
      );
      options.AddSecurityDefinition(
        "Bearer Auth",
        new OpenApiSecurityScheme {
          Type = SecuritySchemeType.Http,
          Scheme = "Bearer",
          BearerFormat = "JWT",
          Description = "JWT Authorization header using the Bearer scheme.",
        }
      );
      options.AddSecurityRequirement(
        new OpenApiSecurityRequirement {
          {
            new OpenApiSecurityScheme {
              Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer Auth",},
            },
            Array.Empty<string>()
          },
        }
      );
    }
  );

  // Add database connection
  var connectionString = builder.Configuration["DbConnectionString"];
  var logLevel = builder.Environment.IsDevelopment() ? LogLevel.Information : LogLevel.Warning;
  var enableDebugInfo = builder.Environment.IsDevelopment();

  builder.Services.AddDbContext<AppDbContext>(
    options => {
      var serverVersion = ServerVersion.AutoDetect(connectionString);
      options.UseMySql(connectionString, serverVersion)
        .LogTo(Console.WriteLine, logLevel)
        .EnableSensitiveDataLogging(enableDebugInfo)
        .EnableDetailedErrors(enableDebugInfo);
    }
  );

  // Add lowercase routes
  builder.Services.AddRouting(options => options.LowercaseUrls = true);

  // Add CORS
  var origins = builder.Configuration.GetSection("AllowedOrigins").Get<List<string>>() ?? new List<string>();

  builder.Services.AddCors(
    options => {
      options.AddDefaultPolicy(
        policy => {
          policy.WithOrigins(origins.ToArray())
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetPreflightMaxAge(TimeSpan.FromMinutes(5));
        }
      );
    }
  );

  // Dependency Injection configuration
  AppInjections.Register(builder.Services);

  // AutoMapper configuration
  builder.Services.AddAutoMapper(typeof(ModelToResourceProfile), typeof(ResourceToModelProfile));

  var app = builder.Build();

  app.UseSerilogRequestLogging();

  // Configure the HTTP request pipeline.  
  // Trust the reverse proxy
  app.UseForwardedHeaders();
  app.UseSwagger();
  app.UseSwaggerUI();
  
  if (!app.Environment.IsEnvironment("Testing")) {
    // Apply any migrations needed automatically
    var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetService<AppDbContext>();
    context?.Database.Migrate();
  }
 
  app.UseCors();

  app.UseJwtMiddleware();

  app.UseAuthorization();

  app.MapControllers();

  app.Run();
  return 0;
} catch (Exception ex) {
  if (environment.Equals("Testing")) {
    throw;
  }

  Log.Fatal(ex, "Host terminated unexpectedly.");
  return 1;
} finally {
  Log.CloseAndFlush();
}

// ReSharper disable once ClassNeverInstantiated.Global
[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
public partial class Program {}

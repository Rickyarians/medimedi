using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using System.Reflection;
using System.Text;
using Confluent.Kafka;
using CRUDServices.BusinessObjects.Code;
using CollectionServices.BusinessObject.Kafka;
using CRUDServices.DataAccess.Context;
using CRUDServices.BusinessObjects.User;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    var assembly = Assembly.GetExecutingAssembly();
    FileInfo fileInfo = new(assembly.Location);
    var lastModified = fileInfo.LastWriteTime;

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
    builder.Host.UseNLog();

    // Add services to the container.

    builder.Services.AddCors(options => options.AddDefaultPolicy(
        corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
    ));
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(x =>
    {
        x.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "CRUD Services " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            Description = "Last Update : " + lastModified.ToString("dd-MM-yyyy HH:mm:ss"),
            Contact = new OpenApiContact
            {
                Name = "Copyrights � " + DateTime.Now.Year +
                       ". CRUD TEST. All rights reserved",
                Email = "Ricky Ariansyah",
                Url = new Uri("https://www.test.com")
            }
        });
        x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        x.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    builder.Services.AddDbContext<CRUDContext>(option
        => option.UseSqlServer(builder.Configuration.GetConnectionString("CRUDConnection")), ServiceLifetime.Transient);

    builder.Services.AddScoped<BOGeneral>();
   
    builder.Services.AddSingleton<IApiHelper, ApiHelper>();
    //builder.Services.AddScoped<IKafkaDependentProducer<Null, string>, KafkaDependentProducer<Null, string>>();
    //builder.Services.AddSingleton<KafkaClientHandle>();
    //builder.Services.AddSingleton<KafkaDependentProducer<Null, string>>();
    //builder.Services.AddSingleton<KafkaDependentProducer<string, long>>();
    //builder.Services.AddHostedService<KafkaConsumer>();
    builder.Services.AddScoped<IBOUser, BOUser>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWTSettings:SecretKey"] ?? "")),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

  

    var xmlPath = Path.Combine(AppContext.BaseDirectory, "Logs");
    GlobalDiagnosticsContext.Set("configDir", xmlPath);
    GlobalDiagnosticsContext.Set("connectionString", builder.Configuration.GetConnectionString("LogConnection"));

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsProduction())
    {
       app.UseSwagger();
       app.UseSwaggerUI(x =>
       {
           x.DefaultModelsExpandDepth(-1);
           x.DocumentTitle = "CRUD Services - " + app.Environment.EnvironmentName;
       });
    }

    
    app.UseCors();

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.UseMiddleware<NLogRequestPostedBodyMiddleware>(new NLogRequestPostedBodyMiddlewareOptions());
    //app.UseMiddleware<LoggerRequestResponseWebApi>();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
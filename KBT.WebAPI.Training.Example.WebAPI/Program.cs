using System.Reflection;
using System.Text;
using System.Xml;
using KBT.WebAPI.Training.Example.Entities.Demo;
using KBT.WebAPI.Training.Example.Entities.JWT;
using KBT.WebAPI.Training.Example.WebAPI.Infrastructures;
using KBT.WebAPI.Training.Example.WebAPI.Utils.GlobalErrorHandling;
using KBT.WebAPI.Training.Example.WebAPI.Utils.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add logging
log4net.ILog logger = log4net.LogManager.GetLogger("Startup");
log4net.ThreadContext.Properties["threadid"] = Thread.CurrentThread.ManagedThreadId;

XmlDocument log4netConfig = new XmlDocument();
log4netConfig.Load(File.OpenRead("log4net.config"));
var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
           typeof(log4net.Repository.Hierarchy.Hierarchy));
log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

try
{
    IConfiguration configuration = builder.Configuration;

    // Add services to the container.
    builder.Services.AddDbContext<DemoDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DemoDatabase"))
    );

    builder.Services.AddDbContext<JwtDbContext>(options =>
        options.UseSqlite(configuration.GetConnectionString("JWTDatabase"))
    );

    // Install all service injection
    Installer.InstallService(builder.Services, configuration);

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(SwaggerHelper.ConfigureSwaggerGen);

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAnyOrigin",
            builder => builder.SetIsOriginAllowed(x => _ = true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });

    // Register JWT Token
    //secret is generated string 256 length
    var secret = Encoding.ASCII.GetBytes(configuration.GetSection("JWT:Secret").Value);
    builder.Services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(option =>
    {
        option.RequireHttpsMetadata = false;
        option.SaveToken = true;
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(secret),
            ClockSkew = TimeSpan.Zero
        };
        option.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

    var app = builder.Build();

    app.ConfigureExceptionHandler();

    using (var scope = app.Services.CreateScope())
    {
        var dataContext = scope.ServiceProvider.GetRequiredService<JwtDbContext>();
        dataContext.Database.EnsureCreated();
        dataContext.Database.Migrate();
    }

    // Add CORS
    app.UseCors("AllowAnyOrigin");

    logger.Debug("Environment is : " + app.Environment.EnvironmentName);
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(SwaggerHelper.ConfigureSwagger);
        app.UseSwaggerUI(SwaggerHelper.ConfigureSwaggerUI);
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex.Message, ex);
}
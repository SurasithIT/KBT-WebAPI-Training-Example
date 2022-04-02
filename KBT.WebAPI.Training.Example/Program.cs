using System.Reflection;
using System.Xml;
using KBT.WebAPI.Training.Example.Entities.Demo;
using KBT.WebAPI.Training.Example.Entities.JWT;
using Microsoft.EntityFrameworkCore;

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

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAnyOrigin",
            builder => builder.SetIsOriginAllowed(x => _ = true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });

    var app = builder.Build();

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
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex.Message, ex);
}
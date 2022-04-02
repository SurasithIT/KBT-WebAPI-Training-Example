using System.Reflection;
using System.Xml;

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
    // Add services to the container.

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
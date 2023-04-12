using Avangardum.LifeArena.Server.Helpers;
using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Models;
using Avangardum.LifeArena.Server.Settings;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices();
var app = builder.Build();
ConfigureMiddlewarePipeline();
StartBackgroundServices();
app.Run();

void ConfigureServices()
{
    var services = builder.Services;
    services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    ConfigureSettings();
    ConfigureCustomServices();

    void ConfigureSettings()
    {
        var configuration = builder.Configuration;
        services.Configure<CoreGameModelSettings>(configuration.GetSection("CoreGameModel"));
        services.Configure<GameServiceSettings>(configuration.GetSection("GameService"));
    }

    void ConfigureCustomServices()
    {
        services.AddSingleton<CoreGameModelFactory>();
        services.AddSingleton<ICoreGameModel>(provider => 
            provider.GetRequiredService<CoreGameModelFactory>().CreateCoreGameModel());
        services.AddSingleton<IGameService, GameService>();
        services.AddSingleton<IUserIdProvider, IpAddressUserIdProvider>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }
}

void ConfigureMiddlewarePipeline()
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
}

void StartBackgroundServices()
{
    var services = app.Services;
    services.GetRequiredService<IGameService>();
}

public partial class Program { }
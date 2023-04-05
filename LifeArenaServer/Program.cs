using Avangardum.LifeArena.Server.Settings;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices();
var app = builder.Build();
ConfigureMiddlewarePipeline();
app.Run();

void ConfigureServices()
{
    var services = builder.Services;
    services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    ConfigureSettings();

    void ConfigureSettings()
    {
        var configuration = builder.Configuration;
        services.Configure<CoreGameModelSettings>(configuration.GetSection("CoreGameModel"));
        services.Configure<GameServiceSettings>(configuration.GetSection("GameService"));
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
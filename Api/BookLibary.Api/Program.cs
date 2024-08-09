using BookLibary.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));


// Startup sınıfını kullanarak uygulama yapılandırmasını başlat
builder.Host.ConfigureAppConfiguration((context, config) =>
{
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
})
.ConfigureServices((context, services) =>
{
    var startup = new Startup(context.Configuration);
    startup.ConfigureServices(services);
});

var app = builder.Build();

var startup = new Startup(app.Configuration);
startup.Configure(app, app.Environment);

app.Run();

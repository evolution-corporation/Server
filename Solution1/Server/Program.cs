using System.Text.Json.Serialization;
using Amazon.S3;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Server.Controllers;
using Server.Helpers;
using Server.Services;
using Subscription;
using Newtonsoft.Json;
using Resources = Server.Helpers.Resources;
using TinkoffCredential = Server.Helpers.TinkoffCredential;

var builder = WebApplication.CreateBuilder(args);
//File.Copy("../settings/app_settings.json", "appsettings.json", true);
var googleCredential = File.ReadAllText("../settings/firebase_key.json");
var tinkoffCredential = File.ReadAllText("../settings/tinkoff_key.json");
var resourcesSettings = File.ReadAllText("../settings/resources.json");
var ip = "http://*:8000";
// add services to DI container
{
    var services = builder.Services;
    services.AddDbContext<DataContext>();
    services.AddCors();
    services.AddControllers().AddJsonOptions(x =>
    {
        // serialize enums as strings in api responses (e.g. Role)
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // ignore omitted parameters on models to enable optional params (e.g. User update)
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        x.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    //var resourceSection = builder.Configuration.GetSection("Resources");
    var resources = (Resources)JsonConvert.DeserializeObject(resourcesSettings, typeof(Resources))!;
    //resources.DbConnectionString = connectionString;
    //var resources = JsonConverter.Deserialize<Resources>(resourcesSettings)!;
    // var resources = new Resources
    // {
    //     Storage = resourceSection["Storage"],
    //     ImageBucket = resourceSection["ImageBucket"],
    //     AudioBucket = resourceSection["AudioBucket"],
    //     SubscriptionImages = resourceSection["SubscriptionImages"],
    //     MeditationImages = resourceSection["MeditationImages"],
    //     UserImage = resourceSection["UserImage"]
    // };
    services.AddSingleton(_ => resources);
    //services.Configure<Resources>(builder.Configuration.GetSection("Resources"));
    var credential = (TinkoffCredential)JsonConvert.DeserializeObject(tinkoffCredential, typeof(TinkoffCredential))!;
    if (credential == null)
        throw new ArgumentException("You forgot about Tinkoff credentials!");
    services.AddSingleton(_ => credential);
    // configure DI for application services
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<INicknameService, NicknameService>();
    services.AddScoped<IUserImageService, UserImageService>();
    services.AddScoped<IAuthenticationService, AuthenticationService>();
    services.AddScoped<IMeditationService, MeditationService>();
    services.AddScoped<ISubscribeService, SubscribeService>();
    services.AddScoped<IPaymentService, PaymentService>();
    services.AddScoped<IMeditationImageService, MeditationImageService>();
    services.AddScoped<INotificationService, NotificationService>();
    services.AddScoped<ITinkoffNotificationService, TinkoffNotificationService>();
    services.AddScoped<IMeditationAudioService, MeditationAudioService>();
    services.AddScoped(typeof(WebServicesController));
    services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
    //services.AddAWSService<IAmazonS3>();
    new AmazonS3Client(new AmazonS3Config { ServiceURL = "https://s3.yandexcloud.net" }).GetBucketVersioningAsync(
        resources.ImageBucket);
    services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(new AmazonS3Config { ServiceURL = "https://s3.yandexcloud.net" }));
    services.AddSingleton<AmazonS3Client>(_ => new AmazonS3Client(new AmazonS3Config { ServiceURL = "https://s3.yandexcloud.net" }));
    services.AddSignalR();
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromJson(googleCredential),
        ProjectId = "plants-336217",
    });
}
var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    app.MapControllers();
}


new Task(() => new CheckSubscription().Run()).Start();
new Task(NicknameService.Run).Start();

new Task(() =>
{
    Thread.Sleep(1000 * 60 * 60);
    ((WebServicesController)app.Services.GetService(typeof(WebServicesController))!).ShutdownSite();
}).Start();
app.Run(ip);
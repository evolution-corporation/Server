﻿using System.Text.Json.Serialization;
using Amazon.S3;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Server.Controllers;
using Server.Helpers;
using Server.Services;
using Subscription;
using JsonSerializer = System.Text.Json.JsonSerializer;
using TinkoffCredential = Server.Helpers.TinkoffCredential;

var builder = WebApplication.CreateBuilder(args);
//builder.Environment.WebRootPath = Directory.GetCurrentDirectory() + "/..";
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
    // var resources = new Resources
    // {
    //     Storage = resourceSection["Storage"],
    //     ImageBucket = resourceSection["ImageBucket"],
    //     AudioBucket = resourceSection["AudioBucket"],
    //     SubscriptionImages = resourceSection["SubscriptionImages"],
    //     MeditationImages = resourceSection["MeditationImages"],
    //     UserImage = resourceSection["UserImage"]
    // };

    var resources = JsonSerializer.Deserialize<Resources>(Environment.GetEnvironmentVariable("Resources"));

    services.AddSingleton(_ => resources);
    //services.Configure<Resources>(builder.Configuration.GetSection("Resources"));
    var credential =
        JsonSerializer.Deserialize<TinkoffCredential>(Environment.GetEnvironmentVariable("TinkoffCredential"))!;
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
    new AmazonS3Client(new AmazonS3Config { ServiceURL = "https://s3.yandexcloud.net" }).GetBucketVersioningAsync(
        resources.ImageBucket);
    services.AddSingleton(_ => new AmazonS3Client(new AmazonS3Config { ServiceURL = "https://s3.yandexcloud.net" }));
    services.AddSignalR();
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromJson(Environment.GetEnvironmentVariable("GoogleCredential")),
        //Credential =
          //  GoogleCredential.FromJson(Environment.GetEnvironmentVariable("GoogleCredential")),
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
app.Run(ip);
﻿using System.Text.Json.Serialization;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Server.Controllers;
using Server.Helpers;
using Server.Services;
using JsonSerializer = System.Text.Json.JsonSerializer;

var builder = WebApplication.CreateBuilder(args);
var ip = "http://*:8000";
var client = new HttpClient();
var message = new HttpRequestMessage(HttpMethod.Get, "62.84.125.238:8000/api/204");
client.Send(message);
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
    });
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    var resourceSection = builder.Configuration.GetSection("Resources");
    var resources = new Resources
    {
        MeditationAudio = resourceSection["MeditationAudio"], MeditationImages = resourceSection["MeditationImages"],
        UserImage = resourceSection["UserImage"]
    };
    services.AddSingleton(_ => resources);
    //services.Configure<Resources>(builder.Configuration.GetSection("Resources"));
    //TODO: Перделать Tinkoff Credential на прямую загрузку из appsettings.json
    var str = builder.Configuration["TinkoffCredential"];
    var credential = (TinkoffCredential)JsonSerializer.Deserialize(str, typeof(TinkoffCredential))!;
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
    services.AddScoped<IDmdService, DmdService>();
    services.AddScoped<IMeditationImageService, MeditationImageService>();
    services.AddScoped<INotificationService, NotificationService>();
    services.AddScoped<ITinkoffNotificationService, TinkoffNotificationService>();
    services.AddScoped<Notificator>();
    services.AddSignalR();
    FirebaseApp.Create(new AppOptions
    {
        Credential =
            GoogleCredential.FromJson(builder.Configuration["GoogleCredential"]),
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
    app.MapHub<AsyncEnumerableHub>("/meditation.audio");
}

app.Run(ip);
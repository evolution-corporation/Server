using System.Text.Json.Serialization;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Server.Controllers;
using Server.Entities;
using Server.Helpers;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);
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
    
    services.Configure<Resources>(builder.Configuration.GetSection("Resources"));
    // configure DI for application services
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<INicknameService, NicknameService>();
    services.AddScoped<IUserImageService, UserImageService>();
    services.AddScoped<IAuthenticationService, AuthenticationService>();
    services.AddScoped<IMeditationService, MeditationService>();
    services.AddScoped<ISubscribeService, SubscribeService>();
    services.AddScoped<IPaymentService, PaymentService>();
    services.AddScoped<IDmdService, DmdService>();
    services.AddScoped<IMeditationAudioService, MeditationAudioService>();
    services.AddScoped<IMeditationImageService, MeditationImageService>();
    services.AddScoped<Resources>();
    services.AddSignalR();
    FirebaseApp.Create(new AppOptions
    {
        Credential =
            GoogleCredential.FromFile(builder.Configuration["GoogleCredential"]),
        ProjectId = "<plants-336217>",
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



app.Run("http://localhost:4000");
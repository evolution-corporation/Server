// See https://aka.ms/new-console-template for more information

using Bot;
using Ninject; 
using Microsoft.Extensions.Configuration;
using Server.Helpers;

static StandardKernel ConfigureContainer()
{
    var container = new StandardKernel();
    
    var config =  new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();
    container.Bind<IConfiguration>().ToConstant(config);
    container.Bind<TelegramBot>().To<TelegramBot>();
    var resourceSection = config.GetSection("Resources");
    var resources = new Resources{MeditationAudio = resourceSection["MeditationAudio"],MeditationImages = resourceSection["MeditationImages"],UserImage = resourceSection["UserImage"]};
    container.Bind<Resources>().ToConstant(resources);
    return container;
}

var container = ConfigureContainer();

var config = container.Get<IConfiguration>();
container.Get<TelegramBot>();

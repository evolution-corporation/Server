using Microsoft.Extensions.Configuration;
using Server.Entities;
using Server.Entities.Mediatation;
using Server.Helpers;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace Bot;

public class TelegramBot
{
    private TelegramBotClient bot;
    private DataContext context;
    private readonly Resources resources;
    private Dictionary<string?, List<Meditation>> dictionary;

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        // Некоторые действия
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            var message = update.Message;
            if (message.Text.ToLower() == "/start")
            {
                var markup = new ReplyKeyboardMarkup(new KeyboardButton("Список медитаций"));
                await botClient.SendTextMessageAsync(message.Chat,
                    "Здравствуйте, по нажатию кнопки вы получите список всех доступных для прослушивания медитаций",
                    replyMarkup: markup, cancellationToken: cancellationToken);
                Thread.Sleep(6000);
                await botClient.SendTextMessageAsync(message.Chat, "Hello");
            }

            if (message.Text.ToLower().Equals("Список медитаций"))
            {
                var meditations = context.Meditations
                    .Where(x => !x.IsSubscribed)
                    .ToList();
                dictionary.Add(message.Chat.Username, meditations);
                await botClient.SendTextMessageAsync(message.Chat,
                    string.Join("\n", meditations.Select((x, i) => $"{i + 1}. {x.Name}")),
                    cancellationToken: cancellationToken);
            }

            // if (message.Text.ToLower().StartsWith("/get_meditation"))
            // {
            //     var id = Convert.ToInt32(message.Text.Split()[1]);
            //     if (!dictionary.ContainsKey(message.Chat.Username))
            //         return;
            //     await botClient.SendTextMessageAsync(message.Chat,
            //         dictionary[message.Chat.Username][id - 1].Description!, cancellationToken: cancellationToken);
            //     var meditation = dictionary[message.Chat.Username][id - 1];
            //     var file = File.OpenRead($"{resources.MeditationAudio}/{meditation.id}");
            //     await botClient.SendAudioAsync(message.Chat, new InputOnlineFile(file, meditation.Name),
            //         cancellationToken: cancellationToken);
            // }
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
    }

    public TelegramBot(IConfiguration configuration, DataContext context, Resources resources)
    {
        bot = new TelegramBotClient(configuration.GetSection("BotToken").Value);
        this.context = context;
        this.resources = resources;
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }, // receive all update types
        };
        dictionary = new Dictionary<string?, List<Meditation>>();
        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
        Console.WriteLine("Бот запущен");
        while (true)
        {
        }
    }
}
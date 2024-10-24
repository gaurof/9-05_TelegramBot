using OpenQA.Selenium.DevTools.V128.PWA;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MainConsoleApp;

public class TelegramBot
{
    public static TelegramBotClient Client = new("7684584581:AAH3GSHb5Vray3dv6pPl6Qtp4CyCsw3VDvI");
    //Please don't take my API key, I'm just testing this bot anyway

    public static async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        //Dealing with a new user
        User? currentUser = Storage.GetUserByID(update.Message!.From!.Id);

        if (currentUser is null)
        {
            currentUser = new User(update.Message.From.Id, update.Message.From.Username!, update.Message.From.LastName!);
            Storage.Users?.Add(currentUser);
        }
        Console.WriteLine($"{currentUser.LastName} ({currentUser.Username}): {update.Message.Text}");



        if (update.Message.Text is not null)
        {
            currentUser.Messages?.Add(update.Message.Text);

            if (update.Message.Text.ToLower() == "/start")
            {
                await currentUser.CurrentGame.StartAsync();


                var messageToSend = await currentUser.CurrentGame.GetTextAsync(false);
                messageToSend = await Translator.TranslateAsync(messageToSend, currentUser.PreferredLanguage[..2]);
                await currentUser.SendGameMessageAsync(messageToSend);
                return;
            }

            if (update.Message.Text.ToLower().StartsWith("/language ")) 
            {
                
            }

            if (currentUser.CurrentGame is not null)
            {
                await currentUser.CurrentGame.EnterCommandAsync(update.Message.Text!);
                await currentUser.SendGameMessageAsync(await currentUser.CurrentGame!.GetTextAsync(true));
            }
        }
        return;
    }
    public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        throw new Exception(exception.ToString());
    }
    public static void Start()
    {
        Client?.StartReceiving(
            updateHandler: HandleUpdateAsync, 
            pollingErrorHandler: HandlePollingErrorAsync
            );
    }
}

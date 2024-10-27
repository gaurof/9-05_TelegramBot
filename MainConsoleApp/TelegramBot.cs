using OpenQA.Selenium.DevTools.V128.PWA;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MainConsoleApp;

public class TelegramBot
{
    public static readonly TelegramBotClient Client = new("7684584581:AAH3GSHb5Vray3dv6pPl6Qtp4CyCsw3VDvI");

    public static async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        switch (update)
        {
            case { Message: { Text: { } } text }:
                await HandleTextMessageAsync(update.Message);
                break;

            case { CallbackQuery: { } cbQuery }:
                await HandleCallbackQueryAsync(cbQuery);
                break;

            default:
                Log.Warning($"Unhandled update");
                break;
        }
    }



    private static async Task HandleCallbackQueryAsync(CallbackQuery cbQuery)
    {
        var currentUser = Storage.GetOrAddUser(cbQuery.From);
        var callbackData = cbQuery.Data;
        Log.Information($"callBackQuery sent from {currentUser.FirstName} (@{currentUser.Username}): {cbQuery.Data}");

        switch (callbackData)
        {
            case "start_game":


                if (currentUser.LastButtonMessageId != 0)
                    await Client.DeleteMessageAsync(currentUser.Id, currentUser.LastButtonMessageId);
                var waitMessageID = (await currentUser.SendMessageAsync(await Translator.TranslateAsync("Wait while the game is starting\\.\\.\\.", currentUser.Language)))!.MessageId;
                await currentUser.CurrentGame.RestartAsync();
                await currentUser.SendGameMessageAsync(await currentUser.CurrentGame.GetTextAsync(false, currentUser.Language));
                await Client.DeleteMessageAsync(currentUser.Id, waitMessageID);
                break;

            case "choose_language":

                break;

            default:
                break;
        }
    }

    private static async Task HandleTextMessageAsync(Message message)
    {
        var currentUser = Storage.GetOrAddUser(message.From);

        Log.Information($"{currentUser.FirstName} (@{currentUser.Username}): {message.Text}");


        currentUser.Messages?.Add(message.Text!);

        switch (message.Text)
        {
            case var text when string.IsNullOrEmpty(text):
                return;

            case var text when text!.StartsWith("/start")
                            || text!.StartsWith("/restart"):
                currentUser.CurrentGame.Stop();
                currentUser.LastButtonMessageId = (await currentUser.ShowMainMenuAsync()).MessageId;
                return;

            case var text when text!.StartsWith("/changelanguage "):
                currentUser.Language = text.Split(" ")[1];
                return;

            default:
                break;
        }
        if (currentUser.CurrentGame.IsActive)
        {
            await currentUser.CurrentGame.EnterCommandAsync(message.Text!);
            await currentUser.SendGameMessageAsync(await currentUser.CurrentGame!.GetTextAsync(true, currentUser.Language));
        }
    }



    public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        throw new Exception(exception.ToString());
    }
    public static void Start()
    {
        Client.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: null
            );

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        Log.Information("Logging started");
    }
}

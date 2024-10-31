using OpenQA.Selenium.DevTools.V127.Debugger;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MainConsoleApp;

public class TelegramBot
{
    public static readonly TelegramBotClient Client = new("7684584581:AAH3GSHb5Vray3dv6pPl6Qtp4CyCsw3VDvI");

    public static async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                switch (update)
                {
                    case { Message.Text: { } }:
                        await HandleTextMessageAsync(update.Message);
                        break;
                    case { CallbackQuery: { } cbQuery }:
                        await HandleCallbackQueryAsync(cbQuery);
                        break;
                    default:
                        Log.Warning("Unhandled update");
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error handling update: {ex.Message}");
            }
        }, cancellationToken);
    }

    private static async Task HandleCallbackQueryAsync(CallbackQuery cbQuery)
    {
        var currentUser = Storage.GetOrAddUser(cbQuery.From);
        var callbackData = cbQuery.Data;
        try
        {
            var buttonMessageId = cbQuery.Message!.MessageId;
            await Client.DeleteMessageAsync(currentUser.Id, buttonMessageId);
        }
        catch (Exception)
        {
            Log.Error("Couldn't delete a message");
        }

        Log.Information($"callBackQuery sent from {currentUser.FirstName} (@{currentUser.Username}): {cbQuery.Data}");

        switch (callbackData)
        {
            case "start_game":

                var waitMessageID = (await currentUser.SendMessageAsync(await Translator.TranslateAsync("Wait while the game is starting\\.\\.\\.", currentUser.Language)))!.MessageId;
                await currentUser.CurrentGame.RestartAsync();
                await currentUser.SendGameMessageAsync(await currentUser.CurrentGame.GetTextAsync(false, currentUser.Language));
                await Client.DeleteMessageAsync(currentUser.Id, waitMessageID);
                break;

            case "change_language":

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

            case var text when text!.StartsWith("/language "):
                currentUser.Language = text.Length > 2 ? text.Split(" ")[1]: currentUser.Language;
                await currentUser.SendMessageAsync("Язык успешно изменён");
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
        Client!.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: null
            );
        Client.SetMyCommandsAsync(
            new List<BotCommand>() 
            { 
                new() { 
                    Command = "restart", 
                    Description = "Restarts your game.",
                },

                new() {
                    Command = "language",
                    Description = "Changes the language.",
                }
            });

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        Log.Information("Logging started");
    }
}

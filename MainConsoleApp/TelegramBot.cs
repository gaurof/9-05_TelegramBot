using Telegram.Bot;
using Telegram.Bot.Types;

namespace MainConsoleApp;

public class TelegramBot
{
    public static TelegramBotClient Client = new("7684584581:AAH3GSHb5Vray3dv6pPl6Qtp4CyCsw3VDvI");

    public static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message!.Text == null)
            return Task.CompletedTask;

        User currentUser = Storage.GetUserByID(update.Message.From.Id);
        if (currentUser == null)
        {
            currentUser = new User(update.Message.From.Id, update.Message.From.Username, update.Message.From.LastName);
            Storage.Users.Add(currentUser);
        }

        Console.WriteLine($"{currentUser.LastName} ({currentUser.Username}): {update.Message.Text}");

        if (update.Message.Text == "/start")
        {
            currentUser.CurrentGame.Restart();
            return Task.CompletedTask;
        }
        if (currentUser.CurrentGame == null)
        {
            currentUser.CurrentGame.Restart();
            currentUser.SendGameMessage(currentUser.CurrentGame.GetText(false));
        }
        currentUser.CurrentGame.EnterCommand(update.Message.Text);
        currentUser.SendGameMessage(currentUser.CurrentGame.GetText(true));

        return Task.CompletedTask;
    }
    public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        throw new Exception(exception.ToString());
    }
    public static void Start()
    {
        Client?.StartReceiving(updateHandler: HandleUpdateAsync, pollingErrorHandler: HandlePollingErrorAsync);
    }
}

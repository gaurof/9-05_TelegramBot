using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MainConsoleApp;

public class TelegramBot
{
    public static TelegramBotClient Client = new("7684584581:AAH3GSHb5Vray3dv6pPl6Qtp4CyCsw3VDvI");

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message.Text == null)
            return;

        var currentUser = new User(update.Message.From.Id, update.Message.From.Username, update.Message.From.LastName);
        if (Storage.IsUserNew(update.Message.From.Id))
            Storage.Users.Add(currentUser);
        else
            currentUser = Storage.GetUserByID(currentUser.Id);

        Console.WriteLine($"{currentUser.LastName} ({currentUser.Username}): {update.Message.Text}");

        if (update.Message.Text == "/start")
        {
            currentUser.RestartGame();
            currentUser.SendMessage(currentUser.CurrentGame.GetText());
            return;
        }

        if (currentUser.CurrentGame == null)
        {
            currentUser.RestartGame();
        }
        currentUser.CurrentGame.EnterCommand(update.Message.Text);
        currentUser.SendMessage(currentUser.CurrentGame.GetText());
    }
    public static async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.ToString);
        Console.WriteLine("Произошёл PollingError");
    }
    public static void Start()
    {
        Client?.StartReceiving(updateHandler: HandleUpdateAsync, pollingErrorHandler: HandlePollingErrorAsync);
    }
}

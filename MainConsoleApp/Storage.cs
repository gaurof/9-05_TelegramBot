using Newtonsoft.Json;
using Telegram.Bot.Types;
using System.IO;

namespace MainConsoleApp;

static class Storage
{
    public static List<User>? Users = new();
    public static string MainMenuString = "Welcome to the text game *9:05*🕘 made by *Adam Cadre*\nPlease chose one of the buttons below to proceed";



    public static User? GetUserByID(long userID)
    {
        if (Users is null || Users.Count == 0)
            return null;

        return Users.FirstOrDefault(u => u.Id == userID);
    }

    internal static User GetOrAddUser(Telegram.Bot.Types.User? user)
    {
        User? currentUser = GetUserByID(user!.Id);

        if (currentUser is null)
        {
            currentUser = new User(user.Id, user.Username ?? "NoUsername", user.FirstName ?? "NoFirstName");
            Storage.Users?.Add(currentUser);
        }

        return currentUser;
    }
}

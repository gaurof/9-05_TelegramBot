using Newtonsoft.Json;

namespace MainConsoleApp;

static internal class Storage
{
    public static List<User>? Users = new();

    public static void SaveData()
    {
        string usersJsonString = JsonConvert.SerializeObject(Users);
        File.WriteAllText("users.json", usersJsonString);
    }
    public static void LoadData()
    {
        if (File.Exists("users.json"))
        {
            Users = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText("users.json"));
        }
    }
    public static bool IsUserNew(long userID)
    {
        if (Users == null || Users.Count == 0)
            return true;
        if (Users.Count != 0)
        {
            foreach (var user in Users)
            {
                if (user.Id == userID)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public static User? GetUserByID(long userID)
    {
        foreach (var user in Users)
        {
            if (user.Id == userID)
            {
                return user;
            }
        }
        return null;
    }
}

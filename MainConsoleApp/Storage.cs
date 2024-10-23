using Newtonsoft.Json;

namespace MainConsoleApp;

static class Storage
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
    public static User? GetUserByID(long userID)
    {
        if(Users?.Count == 0)
            return null;

        foreach (var user in Users!)
        {
            if (user.Id == userID)
            {
                return user;
            }
        }
        return null;
    }
}

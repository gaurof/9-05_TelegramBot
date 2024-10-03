using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;


namespace MainConsoleApp;

public class User
{
    public long Id { get; private set; }
    public string Username { get; private set; }
    public string LastName { get; private set; }
    public List<string> Messages { get; set; }
    public Game CurrentGame { get; private set ; }
    public User(long ID, string username, string lastName)
    {
        Id = ID;
        Username = username;
        LastName = lastName;
        Messages = new List<string>();
        RestartGame();
    }
    public void SendMessage(string message)
    {
        var charactersToChange = new[] { '_', '[', ']', '(', ')', '~', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };
        foreach (var character in charactersToChange)
        {
            message = message.Replace(character.ToString(), $@"\{character}");
        }
        message = message.Replace("  ", " ");
        TelegramBot.Client.SendTextMessageAsync(Id, message, parseMode: ParseMode.MarkdownV2);
    }
    public void RestartGame()
    {
        if (CurrentGame != null)
            CurrentGame.Stop();
        CurrentGame = new Game();
    }


}

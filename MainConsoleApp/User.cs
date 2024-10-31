using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V128.PWA;
using OpenQA.Selenium.Firefox;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Serilog;

namespace MainConsoleApp;


public class User
{
    public long Id { get; private set; }
    public string Username { get; private set; }
    public string FirstName { get; private set; }
    public List<string> Messages { get; set; }
    public Game CurrentGame { get; private set; }
    public string Language { get; set; }
    public int LastButtonMessageId { get; set; }

    public User(long ID, string username, string firstName)
    {
        Id = ID;
        Username = username;
        FirstName = firstName;
        Messages = new List<string>();
        CurrentGame = new();
        Language = "en";
    }

    public async Task SendGameMessageAsync(string message)
    {
        var charactersToChange = new[] { '_', '[', ']', '(', ')', '~', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };
        foreach (var character in charactersToChange)
        {
            message = message.Replace(character.ToString(), $@"\{character}");
        }
        message = message.Replace("  ", " ");
        message = message.Length > 2 ? message.Remove(message.Length - 2) : message;


        await TryToSendMarkupMessageAsync(message);
    }
    private async Task<Telegram.Bot.Types.Message?> TryToSendMarkupMessageAsync(string message) 
    {
        try
        {
            return await TelegramBot.Client.SendTextMessageAsync(Id, message, parseMode: ParseMode.MarkdownV2);
        }
        catch (Exception)
        {
            Serilog.Log.Error($"Couldn't send a message to @{Username}");
            return null;
        }
    }
    public async Task<Telegram.Bot.Types.Message?> SendMessageAsync(string message) => await TryToSendMarkupMessageAsync(message);

    public async Task<Telegram.Bot.Types.Message> ShowMainMenuAsync()
    {
        
        var replyMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(await Translator.TranslateAsync("Start Game 🎮", Language), "start_game"),
                InlineKeyboardButton.WithCallbackData(await Translator.TranslateAsync("Choose Language 🔤", Language), "choose_language")
            }
        });

        return await TelegramBot.Client.SendTextMessageAsync(
            chatId: Id,
            text: await Translator.TranslateAsync(Storage.MainMenuString, Language),
            replyMarkup: replyMarkup,
            parseMode: ParseMode.MarkdownV2
        );
    }
}
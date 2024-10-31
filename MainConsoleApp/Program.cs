using Google.Cloud.Translate.V3;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Serilog;
using System.Xml;

namespace MainConsoleApp;
public class Program
{
    public static void Main()
    {
        TelegramBot.Start();
        Console.ReadKey();
            Log.CloseAndFlush();

    }
}

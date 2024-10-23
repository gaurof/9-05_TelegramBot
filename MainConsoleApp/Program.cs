using Google.Cloud.Translate.V3;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Xml;

namespace MainConsoleApp;
public class Program
{
    public static void Main(string[] args)
    {
        TelegramBot.Start();
        Console.ReadKey();
    }
}

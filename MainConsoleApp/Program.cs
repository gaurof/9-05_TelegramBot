using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Xml;

namespace MainConsoleApp;
public class Program
{
    public static void Main()
    {
        TelegramBot.Start();
        Console.ReadKey();
    }
}

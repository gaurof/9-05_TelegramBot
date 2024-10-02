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
//https://devindeep.com/how-to-automate-web-browser-in-c/#What_is_Web_Browser_Automation
//https://www.selenium.dev/documentation/webdriver/getting_started/first_script/
//https://www.selenium.dev/documentation/#introducing-webdriver
//https://github.com/DevInDeep/Web-Automation

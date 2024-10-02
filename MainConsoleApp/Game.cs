using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V127.CSS;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainConsoleApp
{
    public class Game
    {
        private FirefoxDriver firefoxDriver;
        private int lastMessageLength = 0;

        public Game()
        {
            firefoxDriver = new FirefoxDriver();
            firefoxDriver.Navigate().GoToUrl("https://adamcadre.ac/if/905.html");
            firefoxDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1000);
        }
        public string GetText()
        {
            string str = "";
            var textBox = firefoxDriver.FindElements(By.CssSelector("span, br"));
            foreach (var item in textBox)
            {
                switch (item.TagName)
                {
                    case "span":
                        if (item.GetAttribute("class").Contains("z-breaking-whitespace") || item.GetAttribute("class").Contains("finished-input"))
                            break;
                        if (item.GetAttribute("class").Contains("bold"))
                            break;
                        str += item.Text;
                            break;
                    case "br":
                        str += "\n";
                        break;
                }
            }
            var tempLastMessageLength = str.Length;
            str = str[lastMessageLength..];
            lastMessageLength = tempLastMessageLength;
            Console.WriteLine(str);
            return str;
        }
        public void EnterCommand(string command)
        {
            var input = firefoxDriver.FindElement(By.CssSelector("input"));

            input.Click();
            input.SendKeys(command);
            input.Submit();
        }
        public void Stop()
        {
            if (firefoxDriver != null)
                firefoxDriver.Quit();
        }
        public void Restart()
        {
            Stop();
            firefoxDriver = new FirefoxDriver();
        }
    }
}

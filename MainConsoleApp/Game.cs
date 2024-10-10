using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V127.CSS;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
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
            Start();
        }
        public string GetText(bool wasCommandEnteredBefore)
        {
            string pageString = "";
            string timeLocationString = ""; 
            var textBox = firefoxDriver.FindElements(By.CssSelector("span, br"));
            foreach (var item in textBox)
            {
                switch (item.TagName)
                {
                    case "span":
                        var itemClassName = item.GetAttribute("class");
                        if (itemClassName.Contains("z-breaking-whitespace") || itemClassName.Contains("finished-input"))
                            break;
                        if (itemClassName.Contains("reversed"))
                        {
                            timeLocationString = "`" + item.Text + "`\n";
                            break;
                        }
                        if (itemClassName.Contains("bold"))
                        {
                            pageString += "*" + item.Text + "*";
                            break;
                        }
                        pageString += item.Text;
                            break;
                    case "br":
                        pageString += "\n";
                        break;
                }
            }
            var tempLastMessageLength = pageString.Length;
            pageString = pageString[lastMessageLength..];
            if (wasCommandEnteredBefore)
            {
                pageString = RemoveUserMessage(pageString);
            }
            pageString = timeLocationString + pageString;
            lastMessageLength = tempLastMessageLength;
            Console.WriteLine(pageString);
            return pageString;
        }
        public void EnterCommand(string command)
        {
            Wait();

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
            if (firefoxDriver == null )
                Start();
            firefoxDriver!.Navigate().RefreshAsync();
        }
        public void Start()
        {
            firefoxDriver = new FirefoxDriver();
            firefoxDriver.Navigate().GoToUrl("https://adamcadre.ac/if/905.html");

            Wait();
        }

        private string RemoveUserMessage(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\n')
                {
                    Console.WriteLine(input);
                    Console.WriteLine(input.Remove(0, i));
                    return input.Remove(0, i);
                }
            }

            return input;
        }
        private void Wait()
        {
            WebDriverWait wait = new WebDriverWait(firefoxDriver, TimeSpan.FromSeconds(5))
            {
                PollingInterval = TimeSpan.FromMilliseconds(300),
            };
            wait.Until(a =>
            {
                try
                {
                    firefoxDriver.FindElement(By.CssSelector("input"));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }
    }
}

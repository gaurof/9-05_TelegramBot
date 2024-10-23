using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace MainConsoleApp;

public class Game
{
    private FirefoxDriver? firefoxDriver;
    private int lastMessageLength = 0;

    public Game()
    {
        firefoxDriver = new();
        firefoxDriver.Navigate().GoToUrl("https://adamcadre.ac/if/905.html");
    }
    public async Task EnterCommandAsync(string command)
    {
        WaitForPageLoad();
        command = await Translator.TranslateAsync(command, "en");
        var input = firefoxDriver!.FindElement(By.CssSelector("input"));

        input.Click();
        input.SendKeys(command);
        input.Submit();
    }
    public string GetText(bool wasCommandEnteredBefore)
    {
        string pageString = "";
        string timeLocationString = "";
        var textBox = firefoxDriver!.FindElements(By.CssSelector("span, br"));
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
            pageString = RemoveUserMessageOnPage(pageString);
        pageString = timeLocationString + pageString;
        lastMessageLength = tempLastMessageLength;

        pageString = Translator.TranslateAsync(pageString, "os").Result;
        Console.WriteLine(pageString);
        return pageString;
    }
    public async Task RestartAsync()
    {
        if (firefoxDriver == null)
            await StartAsync();
        await firefoxDriver!.Navigate().RefreshAsync();
    }
    public async Task StartAsync()
    {
        if (firefoxDriver != null)
        {
            await firefoxDriver!.Navigate().RefreshAsync();
            return;
        }
        firefoxDriver = new FirefoxDriver();
        firefoxDriver.Navigate().GoToUrl("https://adamcadre.ac/if/905.html");

        WaitForPageLoad();
    }
    private void WaitForPageLoad()
    {
        WebDriverWait wait = new(firefoxDriver, TimeSpan.FromSeconds(5))
        {
            PollingInterval = TimeSpan.FromMilliseconds(300),
        };
        wait.Until(a =>
        {
            try
            {
                firefoxDriver!.FindElement(By.CssSelector("input"));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        });
    }
    public void Stop()
    {
        if (firefoxDriver != null)
            firefoxDriver.Quit();
    }

    private static string RemoveUserMessageOnPage(string input)
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
}

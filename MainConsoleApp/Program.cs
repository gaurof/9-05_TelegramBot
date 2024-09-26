using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Xml;

public class Game
{
    public static void Main()
    {
        var firefoxDriver = new FirefoxDriver();
        firefoxDriver.Navigate().GoToUrl("https://adamcadre.ac/if/905.html");
        firefoxDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1000);
        string str = "";
        var textBox = firefoxDriver.FindElements(By.CssSelector("span"));
        foreach (var item in textBox)
        {
            str += item.Text;
        }
        Console.WriteLine(str);
        var input = firefoxDriver.FindElement(By.CssSelector("input"));


        input.Click();
        input.SendKeys("get out of bed");
        input.Submit();



    }
}



//https://devindeep.com/how-to-automate-web-browser-in-c/#What_is_Web_Browser_Automation
//https://www.selenium.dev/documentation/webdriver/getting_started/first_script/
//https://www.selenium.dev/documentation/#introducing-webdriver
//https://github.com/DevInDeep/Web-Automation

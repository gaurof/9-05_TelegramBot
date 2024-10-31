using Newtonsoft.Json.Linq;
using Serilog;


namespace MainConsoleApp;
public static class Translator
{
    private static async Task<string> TranslateTextAsync(string input, string targetLanguageCode)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={targetLanguageCode}&dt=t&q={Uri.EscapeDataString(input)}";

        using HttpClient httpClient = new();
        HttpResponseMessage response = await httpClient.GetAsync(url);
        string result = await response.Content.ReadAsStringAsync();

        // The result is a nested array, we need to parse it.
        JArray jsonResponse = JArray.Parse(result);
        string translatedText = jsonResponse[0]![0]![0]!.ToString();

        return translatedText;
    }


    public static async Task<string> TranslateAsync(string input, string targetLanguage)
    {
        var inputStrings = input.Split('\n');
        var output = "";
        
        foreach (var inputString in inputStrings)
        {
            output += await TranslateTextAsync(inputString, targetLanguage[..2].ToLower()) + "\n";
        }
        Log.Information($"Translated to {targetLanguage}");

        return output;
    }public static async Task<string> TranslateGameAsync(string input, string targetLanguage)
    {
        if (targetLanguage[..2] == "en")
            return input;
        var inputStrings = input.Split('\n');
        var output = "";
        
        foreach (var inputString in inputStrings)
        {
            output += await TranslateTextAsync(inputString, targetLanguage[..2].ToLower()) + "\n";
        }
        Log.Information($"Translated to {targetLanguage}");

        return output;
    }
}
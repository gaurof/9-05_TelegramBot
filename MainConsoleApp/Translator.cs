using Newtonsoft.Json.Linq;


namespace MainConsoleApp;
public static class Translator
{
    private static SemaphoreSlim semaphore = new(1, 1);
    private static async Task<string> TranslateTextAsync(string input, string targetLanguage)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";
        string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl={targetLanguage}&dt=t&q={Uri.EscapeDataString(input)}";

        using (HttpClient httpClient = new HttpClient())
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            string result = await response.Content.ReadAsStringAsync();

            // The result is a nested array, we need to parse it.
            JArray jsonResponse = JArray.Parse(result);
            string translatedText = jsonResponse[0]![0]![0]!.ToString();

            return translatedText;
        }
    }
    public static async Task<string> TranslateAsync(string input, string targetLanguage)
    {
        var inputStrings = input.Split('\n');
        var output = "";

        foreach (var inputString in inputStrings)
        {
            output += await TranslateTextAsync(inputString, targetLanguage) + "\n";
        }

        return output;
    }
}
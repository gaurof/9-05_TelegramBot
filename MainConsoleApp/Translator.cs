using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MainConsoleApp;
internal static class Translator
{
    const string baseUrl = "http://api.mymemory.translated.net";
    private static HttpClient httpClient = new();



    public static string Translate(string text, string sourceLang, string targetLang)
    {
        var stringsToTranslate = text.Split('\n');
        var translatedText = "";

        foreach (var stringPart in stringsToTranslate)
        {
            if (stringPart == "")
            { 
                translatedText += "\n";
                continue;
            }
            translatedText += TranslateAsync(stringPart, sourceLang, targetLang).Result + "\n";
        }

        return translatedText;
    }
    private static async Task<string> TranslateAsync(string text, string sourceLang, string targetLang)
    {
        string url = $"{baseUrl}/get?q={Uri.EscapeDataString(text)}&langpair={sourceLang}|{targetLang}";

        HttpResponseMessage response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string responseJson = await response.Content.ReadAsStringAsync();
        var translationResult = JsonConvert.DeserializeObject<TranslationResponse>(responseJson);

        if (translationResult.ResponseStatus == 200)
            return translationResult.TranslatedText;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Something went wrong in the translation " + translationResult.ResponseStatus);
        Console.ForegroundColor = ConsoleColor.White;

        return string.Empty;

    }

}

public class TranslationResponse
{
    [JsonProperty("responseStatus")]
    public int ResponseStatus { get; set; }

    [JsonProperty("responseData")]
    public TranslationData ResponseData { get; set; }

    public string TranslatedText => ResponseData?.TranslatedText;
}

public class TranslationData
{
    [JsonProperty("translatedText")]
    public string TranslatedText { get; set; }
    
}


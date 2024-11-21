using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Meine_Ki
{
    public class GeminiSearchService
    {
        private readonly HttpClient _httpClient;
        private const string GeminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";
        private const string ApiKey = "AIzaSyAfgkpKZYh18ZfIuBKvA0CX47Ir4DeKuzc"; // Hier deinen API-Key einfügen

        public GeminiSearchService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<SearchResult> GetResponseAsync(string query)
        {
            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new
                                {
                                    text = $"Beantworte folgende Frage auf Deutsch und ausführlich: {query}"
                                }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 800,
                    }
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    $"{GeminiApiUrl}?key={ApiKey}",
                    content
                );

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(responseJson);

                if (geminiResponse?.Candidates != null && geminiResponse.Candidates.Count > 0)
                {
                    var answer = geminiResponse.Candidates[0].Content.Parts[0].Text;
                    return new SearchResult
                    {
                        Content = FormatResponse(answer),
                        Sources = new List<string> { "" }
                    };
                }

                return new SearchResult
                {
                    Content = "Entschuldigung, ich konnte keine passende Antwort generieren.",
                    Sources = new List<string>()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler: {ex.Message}");
                return new SearchResult
                {
                    Content = "Es ist ein Fehler aufgetreten. Bitte versuchen Sie es erneut.",
                    Sources = new List<string>()
                };
            }
        }

        private string FormatResponse(string response)
        {
            var builder = new StringBuilder();
            builder.AppendLine("📝 Antwort:");
            builder.AppendLine();
            builder.AppendLine(response);
            return builder.ToString();
        }

        private class GeminiResponse
        {
            [JsonProperty("candidates")]
            public List<Candidate> Candidates { get; set; }
        }

        private class Candidate
        {
            [JsonProperty("content")]
            public Content Content { get; set; }
        }

        private class Content
        {
            [JsonProperty("parts")]
            public List<Part> Parts { get; set; }
        }

        private class Part
        {
            [JsonProperty("text")]
            public string Text { get; set; }
        }
    }
}
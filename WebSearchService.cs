using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace Meine_Ki
{
    public class WebSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly HashSet<string> _processedUrls;
        private const string DuckDuckGoUrl = "https://html.duckduckgo.com/html/";

        public WebSearchService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            _processedUrls = new HashSet<string>();
        }

        public async Task<List<SearchResult>> SearchAndSummarize(string query)
        {
            var results = new List<SearchResult>();
            _processedUrls.Clear();

            try
            {
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("q", query),
                    new KeyValuePair<string, string>("b", ""),
                    new KeyValuePair<string, string>("kl", "de-de")
                });

                var response = await _httpClient.PostAsync(DuckDuckGoUrl, formContent);
                var html = await response.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var searchResults = doc.DocumentNode.SelectNodes("//div[contains(@class, 'result')]");

                if (searchResults != null)
                {
                    foreach (var result in searchResults)
                    {
                        if (results.Count >= 4) break; // Maximale Anzahl der Ergebnisse

                        var linkNode = result.SelectSingleNode(".//a[@class='result__a']");
                        if (linkNode?.Attributes["href"]?.Value == null) continue;

                        var url = HttpUtility.HtmlDecode(linkNode.Attributes["href"].Value);

                        // Überspringe bereits verarbeitete URLs
                        if (_processedUrls.Contains(url)) continue;

                        try
                        {
                            var content = await GetPageContent(url);
                            if (string.IsNullOrWhiteSpace(content)) continue;

                            var summary = await GenerateIntelligentSummary(content, query);

                            // Überprüfe auf ähnliche Inhalte
                            if (!results.Any(r => IsSimilarContent(r.Summary, summary)))
                            {
                                results.Add(new SearchResult
                                {
                                    Title = HttpUtility.HtmlDecode(linkNode.InnerText.Trim()),
                                    Url = url,
                                    Summary = summary
                                });
                                _processedUrls.Add(url);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Fehler bei URL {url}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler bei der Suche: {ex.Message}");
            }

            return results;
        }

        private bool IsSimilarContent(string text1, string text2)
        {
            if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
                return false;

            text1 = text1.ToLower();
            text2 = text2.ToLower();

            var words1 = text1.Split(' ').ToHashSet();
            var words2 = text2.Split(' ').ToHashSet();

            var commonWords = words1.Intersect(words2).Count();
            var totalWords = Math.Max(words1.Count, words2.Count);

            return (double)commonWords / totalWords > 0.6; // 60% Ähnlichkeit = Duplikat
        }

        private async Task<string> GetPageContent(string url)
        {
            try
            {
                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var nodesToRemove = doc.DocumentNode.SelectNodes(
                    "//script|//style|//header|//footer|//nav|//aside|//iframe|//form");

                if (nodesToRemove != null)
                {
                    foreach (var node in nodesToRemove)
                    {
                        node.Remove();
                    }
                }

                var mainContent = doc.DocumentNode.SelectNodes("//article|//main|//div[@class='content']|//p[string-length(text()) > 100]");
                if (mainContent != null)
                {
                    return string.Join(" ", mainContent.Select(n => CleanText(n.InnerText)));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Abrufen der Seite {url}: {ex.Message}");
            }
            return string.Empty;
        }

        private async Task<string> GenerateIntelligentSummary(string text, string query)
        {
            text = CleanText(text);
            var sentences = Regex.Split(text, @"(?<=[.!?])\s+")
                               .Where(s => s.Length > 20 && s.Length < 300)
                               .Distinct()
                               .ToList();

            var relevantSentences = sentences
                .Where(s => s.ToLower().Contains(query.ToLower()))
                .Take(2)
                .ToList();

            if (!relevantSentences.Any())
            {
                relevantSentences = sentences.Take(2).ToList();
            }

            return string.Join(" ", relevantSentences);
        }

        private string CleanText(string text)
        {
            text = Regex.Replace(text, @"\s+", " ");
            text = Regex.Replace(text, @"[\u0000-\u001F\u007F-\u009F]", "");
            text = Regex.Replace(text, @"[.!?]+(?=[.!?])", "");
            return text.Trim();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Meine_Ki
{
    public class SearchResult
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Summary { get; set; }
        public SearchResultDetails Details { get; set; }
    }

    public class SearchResultDetails
    {
        public string Statistics { get; set; }
        public string Dates { get; set; }
        public string References { get; set; }

        public bool HasStatistics => !string.IsNullOrEmpty(Statistics);
        public bool HasDates => !string.IsNullOrEmpty(Dates);
        public bool HasReferences => !string.IsNullOrEmpty(References);

        public SearchResultDetails() { }

        public SearchResultDetails(string content)
        {
            ExtractStatistics(content);
            ExtractDates(content);
            ExtractReferences(content);
        }

        private void ExtractStatistics(string content)
        {
            var stats = new List<string>();
            var matches = Regex.Matches(content, @"(?<!\w)(?:\d+(?:,\d+)?%|\d+(?:\.\d+)?(?:\s*(?:Millionen|Milliarden|Euro|Dollar|€|\$)))");

            if (matches.Count > 0)
            {
                stats.AddRange(matches.Cast<Match>().Select(m => m.Value));
                Statistics = string.Join(" | ", stats.Distinct().Take(5));
            }
        }

        private void ExtractDates(string content)
        {
            var dates = new List<string>();
            var matches = Regex.Matches(content, @"\b(?:19|20)\d{2}(?:-(?:19|20)\d{2})?\b|\b(?:Januar|Februar|März|April|Mai|Juni|Juli|August|September|Oktober|November|Dezember)\s+(?:19|20)\d{2}\b");

            if (matches.Count > 0)
            {
                dates.AddRange(matches.Cast<Match>().Select(m => m.Value));
                Dates = string.Join(" | ", dates.Distinct().Take(3));
            }
        }

        private void ExtractReferences(string content)
        {
            var refs = new List<string>();
            var matches = Regex.Matches(content, @"(?:laut|nach|gemäß)\s+[^,.]+");

            if (matches.Count > 0)
            {
                refs.AddRange(matches.Cast<Match>().Select(m => m.Value.Trim()));
                References = string.Join(" • ", refs.Distinct().Take(3));
            }
        }
    }
}
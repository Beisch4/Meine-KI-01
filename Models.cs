using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Meine_Ki.Models
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; }
    }

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
    }
}
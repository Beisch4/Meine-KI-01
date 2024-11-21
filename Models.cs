using System;
using System.Collections.Generic;

namespace Meine_Ki
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class SearchResult
    {
        public string Content { get; set; }
        public List<string> Sources { get; set; } = new List<string>();
    }
}
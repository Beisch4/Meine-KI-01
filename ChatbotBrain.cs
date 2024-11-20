using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfChatbot
{
    public class ChatbotBrain
    {
        private readonly Dictionary<string, List<string>> responses;
        private readonly Random random;

        public ChatbotBrain()
        {
            random = new Random();
            responses = new Dictionary<string, List<string>>
            {
                {"grüße", new List<string> {
                    "Hallo! Wie kann ich Ihnen helfen?",
                    "Guten Tag! Wie geht es Ihnen?",
                    "Hi! Schön Sie zu sehen!"
                }},
                {"danke", new List<string> {
                    "Gerne!",
                    "Kein Problem!",
                    "Immer wieder gerne!"
                }},
                {"hilfe", new List<string> {
                    "Ich kann Ihnen bei verschiedenen Themen helfen. Fragen Sie einfach!",
                    "Wie kann ich Ihnen weiterhelfen?",
                    "Ich stehe Ihnen gerne zur Verfügung!"
                }},
                {"default", new List<string> {
                    "Interessant, erzählen Sie mir mehr.",
                    "Ich verstehe. Können Sie das genauer erklären?",
                    "Das ist ein interessanter Punkt. Was denken Sie darüber?"
                }}
            };
        }

        public string GenerateResponse(string input)
        {
            string lowercaseInput = input.ToLower();

            // Suche nach Schlüsselwörtern im Eingabetext
            foreach (var pattern in responses.Keys)
            {
                if (lowercaseInput.Contains(pattern))
                {
                    var possibleResponses = responses[pattern];
                    return possibleResponses[random.Next(possibleResponses.Count)];
                }
            }

            // Wenn kein Schlüsselwort gefunden wurde, verwende Standard-Antworten
            var defaultResponses = responses["default"];
            return defaultResponses[random.Next(defaultResponses.Count)];
        }
    }
}
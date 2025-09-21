using AIDBDataFeatch.DataAccessLayer.GetSchemaDetails;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace AIDBDataFeatch.AIOperations
{
    public class AIOpeartions(IConfiguration configuration) : IAIOperations
    {
        public async Task<string> GetDataFromLLM(IList<SchemaColumn> schema, string querytext)
        {
            string rsquery = string.Empty;

            var tableschmea = JsonConvert.SerializeObject(schema);

            string apiKey = configuration["AIAIPDetails:OpenAIAPIKey"];

            string endpoint = configuration["AIAIPDetails:AIAPIEndPoint"]; ;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string prompt = $"query to {querytext}, please share only quey";

            var payload = new
            {
                model = "sonar", // or "sonar-medium-online", "sonar-deep-research" etc.
                messages = new[]
                {
                    new { role = "system", content = "SQL table Schemas are shared as per schema please provide effective query" },
                    new { role = "user", content = $"{tableschmea} /n{prompt}" //input } //"How many stars are there in our galaxy?" }
                    }
                    // Optional: temperature, max_tokens, etc.


                }
            };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();

                dynamic result = JsonConvert.DeserializeObject(responseJson);
                string reply = result.choices[0].message.content;
                // Try to match code block: ```sql ... ```
                Match match = Regex.Match(reply, @"```(?:sql)?\s*(.*?)\s*```", RegexOptions.Singleline);

                string query;

                if (match.Success)
                {
                    // Extracted from code block
                    query = match.Groups[1].Value.Trim();
                }
                else
                {
                    // Fallback: try to extract SQL-looking line from plain text
                    Match fallbackMatch = Regex.Match(reply, @"(SELECT\s+.*?;)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    query = fallbackMatch.Success ? fallbackMatch.Groups[1].Value.Trim() : "";
                }
                rsquery = query;
                // Use the cleaned query
                Console.WriteLine(query); 
                Console.WriteLine("--------------------------");

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request failed: {e.Message}");
                if (e.Data["response"] != null)
                    Console.WriteLine(e.Data["response"]);
            }

            return rsquery;

        }
    }
}

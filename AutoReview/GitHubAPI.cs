﻿using System.Diagnostics;
using System.Net.Http;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.Json.Serialization;
using System.Threading;
using System.Web;

public static class GitHubAPI
{
    private const string BaseUrl = "https://api.github.com/";

    private static string accessToken 
    {
        get {
            return GitHubAuther.AccessToken;
        }
    }

    private static string username
    {
        get
        {
            return GitHubAuther.Username;
        }
    }

    public static async Task<List<string>> GetRepoBanches(string repoOwner, string repoName)
    {

        List<string> branches = new List<string>();

        using (var client = new HttpClient())
        {
            HttpClient client = new HttpClient();
            string owner = "AutoReviewer-BBD";
            string repo = "AutoReviewer";
            string url = $"https://api.github.com/repos/{owner}/{repo}/branches";

            // Make request to get repositories for the user
            HttpResponseMessage response = await client.GetAsync($"{BaseUrl}repos/{repoOwner}/{repoName}/branches");

            HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                // Read response content
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON response
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = JsonSerializer.Deserialize<List<Repo>>(responseBody, options);

                foreach (var branch in data)
                {
                    branches.Add(branch.Name);
                }
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            JsonDocument document = JsonDocument.Parse(responseContent);

            branchesComboBox.Dispatcher.Invoke(() =>
            {
                branchesComboBox.Items.Clear();
            });

            
            List<string> branchList = new List<string>();
            JsonElement root = document.RootElement;
            foreach (JsonElement repoElement in root.EnumerateArray())
            {
                string name = repoElement.GetProperty("name").GetString();
                branchList.Add(name);
            }

            branchesComboBox.Dispatcher.Invoke(() =>
            {
                branchesComboBox.ItemsSource = branchList;
                branchesComboBox.SelectedIndex = 0;
            });

        }
        catch (HttpRequestException ex)
        {
            Trace.WriteLine($"HTTP request exception: {ex.Message}");
            throw; // Rethrow the exception to propagate it upwards
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"An error occurred: {ex.Message}");
            throw;
        }

        return branches;
    }

    public static async Task<string> CreatePR(string repoOwner, string repoName, string prTitle, string prHead, string prType)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "GitHubAPI"); // GitHub API requires User-Agent header
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            // Create pull request payload
            var prPayload = new PullRequestBody
            {
                title = $"[Autogenerated] {prTitle}",
                body = $"**This PR was autogenerated to include reviews for _{prType}_**",
                head = prHead,
                Base = "main",
            };

            var jsonPayload = JsonSerializer.Serialize(prPayload, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new PullRequestBodyConverter() }
            });

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Make request to create pull request
            HttpResponseMessage response = await client.PostAsync($"{BaseUrl}repos/{repoOwner}/{repoName}/pulls", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                JsonDocument doc = JsonDocument.Parse(responseContent);


                string pullNumber = "";
                // You can then navigate through the JSON structure to get the required data
                // Here's an example if the responseContent is a JSON object containing a "number" property
                if (doc.RootElement.TryGetProperty("number", out JsonElement numberElement))
                {
                    pullNumber = numberElement.GetInt32().ToString();
                }
                else
                {
                    Console.WriteLine("Number not dound");
                }

                await AddReviewersToPR(
                    new List<string> { "KatlegoKungoane" },
                    repoOwner,
                    repoName,
                pullNumber
                );

                Process.Start(new ProcessStartInfo(doc.RootElement.GetProperty("html_url").ToString()) { UseShellExecute = true });
                return "Success dawg 😎";
            }
            else
            {
                return "Somethig got tripped up dawg. Github dont tell us much, so a PR for this branch could be open or youre not authed";
            }
        }
    }

    public static async Task AddReviewersToPR(List<string> usernamesToAdd, string repoOwner, string repoName, string pullNumber)
    {
        List<string> pullRequestLinks = new List<string>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "GitHubAPI"); // GitHub API requires User-Agent header
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            var prPayload = new
            {    
                reviewers=usernamesToAdd
            };

            var jsonPayload = JsonSerializer.Serialize(prPayload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Make request to create pull request
            HttpResponseMessage response = await client.PostAsync($"{BaseUrl}repos/{repoOwner}/{repoName}/pulls/{pullNumber}/requested_reviewers", content);

            if (response.IsSuccessStatusCode)
            {
            }
            else
            {
                Console.WriteLine($"Failed to create pull request for {username}: {response.ReasonPhrase}");
            }
        }
    }



    // Class to represent repository JSON response
    private class Repo
    {
        prTypeComboBox.Dispatcher.Invoke(() =>
        {
            prTypeComboBox.ItemsSource = new List<string> { "Frontend", "Backend", "Database", "Infrastructure", "Maintenance" };
            prTypeComboBox.SelectedIndex = 0;
        });
    }

    private class PullRequestBody
    {
        public string title { get; set; }
        public string head { get; set; }
        public string Base { get; set; }
        public string body { get; set; }
    }

    private class PullRequestBodyConverter : JsonConverter<PullRequestBody>
    {
        public override PullRequestBody Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonDocument = JsonDocument.ParseValue(ref reader);
            var root = jsonDocument.RootElement;

            return new PullRequestBody
            {
                title = root.GetProperty("title").GetString(),
                head = root.GetProperty("head").GetString(),
                Base = root.GetProperty("base").GetString(),
                body = root.GetProperty("body").GetString()
            };
        }

        public override void Write(Utf8JsonWriter writer, PullRequestBody value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("title", value.title);
            writer.WriteString("head", value.head);
            writer.WriteString("base", value.Base);
            writer.WriteString("body", value.body);

            writer.WriteEndObject();
        }
    }
}

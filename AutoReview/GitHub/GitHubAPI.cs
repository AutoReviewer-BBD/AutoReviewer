using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text;
using RealConnection.Data;
using Api.Repositories;
using RealConnection.Models;

namespace Api.GitHub;
public static class GitHubAPI
{
    private const string BaseUrl = "https://api.github.com/";
    private const string RepositoryOwner = "AutoReviewer-BBD";
    private const string Repository = "AutoReviewer";

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

    public static async Task<List<Dictionary<string, string>>> GetUserRepositories()
    {
        try
        {
            HttpClient client = new HttpClient();
            string url = "https://api.github.com/user/repos";

            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            client.DefaultRequestHeaders.Add("User-Agent", "request");

            HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception();
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            JsonDocument document = JsonDocument.Parse(responseContent);

            List<Dictionary<string, string>> repositories = new List<Dictionary<string, string>>();
            JsonElement root = document.RootElement;
            foreach (JsonElement repoElement in root.EnumerateArray())
            {

                string repositoryName = repoElement.GetProperty("name").GetString();
                string repositoryOwnerName = repoElement.GetProperty("owner").GetProperty("login").GetString();
                Console.WriteLine("Add");
                Console.WriteLine(repoElement);
                Console.WriteLine(repositoryName);
                Console.WriteLine(repositoryOwnerName);
                repositories.Add(new Dictionary<string, string>{
                    {"RepositoryName", repositoryName},
                    {"RepositoryOwnerUsername", repositoryOwnerName}
                });
            }

            return repositories;
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
    }

    public static async Task<List<string>> GetUserBranches(string owner, string repo)
    {
        try
        {
            HttpClient client = new HttpClient();
            string url = $"https://api.github.com/repos/{owner}/{repo}/branches";

            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            client.DefaultRequestHeaders.Add("User-Agent", "request");

            HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception();
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            JsonDocument document = JsonDocument.Parse(responseContent);
            
            List<string> branches = new List<string>();
            JsonElement root = document.RootElement;
            foreach (JsonElement repoElement in root.EnumerateArray())
            {
                string name = repoElement.GetProperty("name").GetString();
                branches.Add(name);
                Console.WriteLine("Added branch");
                Console.WriteLine(name);
            }

            return branches;
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
    }

    public static async Task CreatePR(
        string repoOwner, 
        string repoName, 
        string prTitle, 
        string prHead, 
        string prType,
        int skillID,
        int repositoryID
    ) {
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

                // First see if they are adding anyone
                AutoReviewerDbContext dataContext = new AutoReviewerDbContext();
                UserSkillsRepository userSkillsRepository = new UserSkillsRepository(dataContext);
                ICollection<ProcedureUsersWithSkill> usersWithSkill = userSkillsRepository.GetUsersWithSkillInRepository(skillID, repositoryID);
                List<string> reviewers = new List<string>();

                foreach (var entry in usersWithSkill){
                    reviewers.Add(entry.GitHubUsername);
                }

                if (reviewers.Count != 0){
                    string pullNumber = "";
                    // You can then navigate through the JSON structure to get the required data
                    // Here's an example if the responseContent is a JSON object containing a "number" property
                    if (doc.RootElement.TryGetProperty("number", out JsonElement numberElement))
                    {
                        pullNumber = numberElement.GetInt32().ToString();
                    }
                    else
                    {
                        Console.WriteLine("Number not found");
                    }

                    await AddReviewersToPR(
                        reviewers,
                        repoOwner,
                        repoName,
                    pullNumber
                    );

                    Process.Start(new ProcessStartInfo(doc.RootElement.GetProperty("html_url").ToString()) { UseShellExecute = true });   
                }
                else {
                    Console.WriteLine($"There were no users with the skill {prType} in the repo {repoName} to add to this review");
                }
            }
            else
            {
                Console.WriteLine($"Failed to create pull request for {username}: {response.ReasonPhrase}");
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

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to create pull request for {username}: {response.ReasonPhrase}");
            }
        }
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

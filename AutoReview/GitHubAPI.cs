using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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

    public static async Task<List<string>> GetRepositoriesForUser(string username)
    {

        List<string> repositories = new List<string>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "GitHubAPI"); // GitHub API requires User-Agent header
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            // Make request to get repositories for the user
            HttpResponseMessage response = await client.GetAsync($"{BaseUrl}users/{username}/repos");

            if (response.IsSuccessStatusCode)
            {
                // Read response content
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON response
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var repos = JsonSerializer.Deserialize<List<Repo>>(responseBody, options);

                foreach (var repo in repos)
                {
                    repositories.Add(repo.Name);
                }
            }
            else
            {
                Console.WriteLine($"Failed to retrieve repositories: {response.ReasonPhrase}");
            }
        }

        return repositories;
    }

    // Class to represent repository JSON response
    private class Repo
    {
        public string Name { get; set; }
    }
}

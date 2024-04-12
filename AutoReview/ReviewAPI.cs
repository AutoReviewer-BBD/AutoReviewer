using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using static ReviewAPI;

public static class ReviewAPI
{

    static string baseUrl = "http://autoreviewer.eu-west-1.elasticbeanstalk.com";

    public static async Task<List<string>> GetUserReposAsync(string token)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "GitHubAPI"); // GitHub API requires User-Agent header
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // Make request to get repositories for the user
            HttpResponseMessage response = await client.GetAsync($"{ baseUrl }/api/Repository");

            if (response.IsSuccessStatusCode)
            {
                // Read response content
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON response
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = JsonSerializer.Deserialize<List<Repo>>(responseBody, options);
                
                return data.Select(repo => repo.singleString).ToList();
            }
            else
            {
                Console.WriteLine($"Failed to retrieve repositories: {response.ReasonPhrase}");
                return new List<string> { "AutoReviewer-BBD/AutoReviewer" };

            }
        }
    }
    public static async Task LoginUser(string token)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "GitHubAPI"); // GitHub API requires User-Agent header
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            // Make request to get repositories for the user
            HttpResponseMessage response = await client.GetAsync($"http://autoreviewer.eu-west-1.elasticbeanstalk.com/api/Login?githubToken={token}");

            if (response.IsSuccessStatusCode)
            {
                // Read response content
                string responseBody = await response.Content.ReadAsStringAsync();

       
            }
            else
            {
                Console.WriteLine($"Failed to Login");
            }
        }
    }
    public class Repo
    {
        public string repositoryOwnerUsername { get; set; }
        public string repositoryName { get; set; }

        public string singleString
        {
            get
            {
                return repositoryOwnerUsername + "/" + repositoryName;
            }
        }
    }
}

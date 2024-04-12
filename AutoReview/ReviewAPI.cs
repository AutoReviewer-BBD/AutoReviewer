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
        public static async Task<string> CreatePR(string accessToken, string repositoryName, string prTitle, string prHead, string prType)
    {
        using (var client = new HttpClient())
        {
            // Create the request body as JSON
            var requestBody = new
            {
                accessToken,
                repositoryName,
                skillName = prType,
                branchName = prHead,
                prTitle
            };

            // Serialize the request body to JSON
            var requestBodyJson = JsonSerializer.Serialize(requestBody);

            try
            {
                // Send POST request to the API endpoint
                var response = await client.PostAsync($"{baseUrl}/api/CreatePR",
                    new StringContent(requestBodyJson, System.Text.Encoding.UTF8, "application/json"));

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // Handle the failure scenario
                    return $"Failed to create PR. Status code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                return $"An error occurred: {ex.Message}";
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

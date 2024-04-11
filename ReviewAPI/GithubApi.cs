using System.Diagnostics;
using System.Text.Json;

public class GitHubAPI
{

    public static async Task<List<string>> GetUserRepositoriesEndPoint(string token)
    {

        List<string> branches = new List<string>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            client.DefaultRequestHeaders.Add("User-Agent", "request");
            // Make request to get repositories for the user
            HttpResponseMessage response = await client.GetAsync($"https://api.github.com/user/repos");

            if (response.IsSuccessStatusCode)
            {

                var responseContent = await response.Content.ReadAsStringAsync();
                JsonDocument document = JsonDocument.Parse(responseContent);

                JsonElement root = document.RootElement;
                foreach (JsonElement repoElement in root.EnumerateArray())
                {
                    string name = repoElement.GetProperty("full_name").GetString();
                    branches.Add(name);
                }
            }

        }

        return branches;
    }

    public static async Task<string> GetUsernameAsync(string AccessToken)
    {
        try
        {
            HttpClient client = new HttpClient();
            string url = "https://api.github.com/user";

            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            client.DefaultRequestHeaders.Add("Authorization", AccessToken);
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            client.DefaultRequestHeaders.Add("User-Agent", "request");

            HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Response was " + response);
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            using (JsonDocument document = JsonDocument.Parse(responseContent))
            {
                JsonElement root = document.RootElement;
                return root.GetProperty("login").GetString();
            }
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

}
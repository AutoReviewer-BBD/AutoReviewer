using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

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

    public static async Task GetUserRepos(ComboBox branchesComboBox)
    {
        try
        {
            HttpClient client = new HttpClient();
            string owner = "AutoReviewer-BBD";
            string repo = "AutoReviewer";
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
    }

    public static void PopulatePRTypeSelection(ComboBox prTypeComboBox)
    {
        prTypeComboBox.Dispatcher.Invoke(() =>
        {
            prTypeComboBox.ItemsSource = new List<string> { "Frontend", "Backend", "Database", "Infrastructure", "Maintenance" };
            prTypeComboBox.SelectedIndex = 0;
        });
    }
}

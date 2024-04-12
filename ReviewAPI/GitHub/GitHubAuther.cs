using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Api.GitHub;
public static class GitHubAuther
{
    static string ClientId = "42e94b7bf6e02549006b";
    static string ClientSecret = "c57b0282454a88f47ac0eae46f48ffcb1eaa984f"; // Replace with your actual client secret
    static string RedirectUri = "http://localhost:3322/login/oauth2/code/github";

    static string AuthorizationCode { get; set; } = null!;
    public static string Username { get; set; } = null!; 
    public static string AccessToken { get; set; } = null!;

    public static HttpClient Client { get; set; } = null!;

    static async Task<string> GetAccessTokenAsync()
    {
        // Exchange authorization code for access token
        var tokenUrl = "https://github.com/login/oauth/access_token";
        var requestParams = new Dictionary<string, string>
        {
            { "client_id", ClientId },
            { "client_secret", ClientSecret },
            { "code", AuthorizationCode },
            { "redirect_uri", RedirectUri }
        };

        using var client = new HttpClient();
        var response = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(requestParams));
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var queryParams = HttpUtility.ParseQueryString(responseContent);
        Console.WriteLine("queryParams");
        Console.WriteLine(queryParams);
        return queryParams["access_token"];
    }

    //Might remove this function
    public static async Task<string> GetAuthorizedUsername(string token)
    {
        HttpClient client = new HttpClient();
        string url = "https://api.github.com/user";

        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        client.DefaultRequestHeaders.Add("User-Agent", "request");

        HttpResponseMessage response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception();
        }
        var responseContent = await response.Content.ReadAsStringAsync();
        using (JsonDocument document = JsonDocument.Parse(responseContent))
        {
            JsonElement root = document.RootElement;
            return root.GetProperty("login").GetString();
        }
    }

    static void StartLocalHttpServer(Action<string> callback)
    {
        Console.WriteLine("First Server started for auth code, Before lsitener");

        var listener = new HttpListener();
        listener.Prefixes.Add(RedirectUri + "/");
        listener.Start();

        Task.Run(async () =>
        {
            Console.WriteLine("Task started");
            var context = await listener.GetContextAsync();
            var code = context.Request.QueryString["code"];
            callback(code);
            var response = context.Response;
            var responseBytes = System.Text.Encoding.UTF8.GetBytes("<html><body>Authentication successful! You can close this window.</body></html>");
            response.ContentType = "text/html";
            response.ContentLength64 = responseBytes.Length;
            await response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            response.Close();
            listener.Stop();
            Console.WriteLine("Task ended");
        });
    }

    static public async void SetuthLink(/*Button button, TextBlock textBlock, Image image, ComboBox branchesComboBox*/)
    {
        // Construct the authorization URL
        string authorizationUrl = $"https://github.com/login/oauth/authorize?client_id={ClientId}&scope=user,repo,pull_requests:write,pull_requests:read";

        // Set button content
        // button.Content = "Authorize";

        var deletCode = 1;

        // Waiting for user basically
        Console.WriteLine("Enter anything to start:");
        Console.ReadLine();

        // Open the authorization URL in the default web browser
        Process.Start(new ProcessStartInfo(authorizationUrl) { UseShellExecute = true });

        // Start local HTTP server to receive callback
        StartLocalHttpServer(async code =>
        {
            AuthorizationCode = code;

            Console.WriteLine("Authorization code received. Retrieving access token...");

            // Get access token using the received authorization code
            AccessToken = await GetAccessTokenAsync();
            Console.WriteLine(AccessToken);
            // Get username using the access token
            Username = await GetUsernameAsync();

            Console.WriteLine("Access token retrieved.");
            Console.WriteLine($"Logged in as {Username}");

            await GitHubAPI.GetUserRepositories();

            deletCode = 2;
        });

        while (deletCode != 2){}
    }

    public class UserResponse
    {
        public string Login { get; set; }
        // other properties if needed
    }

    static async Task<string> GetUsernameAsync()
    {
        try
        {
            HttpClient client = new HttpClient();
            string url = "https://api.github.com/user";

            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            client.DefaultRequestHeaders.Add("User-Agent", "request");

            HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception();
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

    // static async Task SetUserProfileImage(Image img)
    // {
    //     try
    //     {
    //         HttpClient client = new HttpClient();
    //         string url = "https://api.github.com/user";

    //         client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
    //         client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
    //         client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
    //         client.DefaultRequestHeaders.Add("User-Agent", "request");

    //         HttpResponseMessage response = await client.GetAsync(url);
    //         if (!response.IsSuccessStatusCode)
    //         {
    //             throw new Exception();
    //         }
    //         var responseContent = await response.Content.ReadAsStringAsync();
    //         JsonDocument document = JsonDocument.Parse(responseContent);
            
    //         JsonElement root = document.RootElement;
    //         string imgUrl = root.GetProperty("avatar_url").GetString();

    //         img.Dispatcher.Invoke(() =>
    //         {
    //             BitmapImage bitmap = new BitmapImage();
    //             bitmap.BeginInit();
    //             bitmap.UriSource = new Uri(imgUrl, UriKind.Absolute);
    //             bitmap.EndInit();

    //             img.Source = bitmap;
    //         });
    //     }
    //     catch (HttpRequestException ex)
    //     {
    //         Trace.WriteLine($"HTTP request exception: {ex.Message}");
    //         throw; // Rethrow the exception to propagate it upwards
    //     }
    //     catch (Exception ex)
    //     {
    //         Trace.WriteLine($"An error occurred: {ex.Message}");
    //         throw;
    //     }
    // }
}

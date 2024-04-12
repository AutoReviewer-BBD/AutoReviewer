using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

public static class GitHubAuther
{
    static string ClientId = "42e94b7bf6e02549006b";
    static string ClientSecret = "c57b0282454a88f47ac0eae46f48ffcb1eaa984f"; // Replace with your actual client secret
    static string RedirectUri = "http://localhost:3322/login/oauth2/code/github";

    static string AuthorizationCode { get; set; }
    public static string Username { get; set; } // Add Username property
    public static string AccessToken { get; set; }

    public static HttpClient Client { get; set; }

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
        return queryParams["access_token"];

    }

    static void StartLocalHttpServer(Action<string> callback)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add(RedirectUri + "/");
        listener.Start();

        Task.Run(async () =>
        {
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
        });
    }

    static public void SetAuthLink(Button button, TextBlock textBlock, ComboBox RepoComboBox)
    {
        // Construct the authorization URL
        string authorizationUrl = $"https://github.com/login/oauth/authorize?client_id={ClientId}&scope=user,repo,pull_requests:write,pull_requests:read";

        // Set button content
        button.Content = "Authorize";

        // Handle button click event
        button.Click += async (sender, e) =>
        {
            // Open the authorization URL in the default web browser
            Process.Start(new ProcessStartInfo(authorizationUrl) { UseShellExecute = true });

            // Start local HTTP server to receive callback
            
            StartLocalHttpServer(async code =>
            {
                AuthorizationCode = code;
                
                textBlock.Dispatcher.Invoke(() =>
                {
                    textBlock.Text = "Authorization code received. Retrieving access token...";
                });

                // Get access token using the received authorization code
                AccessToken = await GetAccessTokenAsync();
                Trace.WriteLine(AccessToken);
                // Get username using the access token
                Username = await GetUsernameAsync();

                await ReviewAPI.LoginUser(AccessToken);
                textBlock.Dispatcher.Invoke(() =>
                {
                    textBlock.Text = "Access token retrieved.";
                });

                _ = RepoComboBox.Dispatcher.Invoke(async () =>
                {
                    RepoComboBox.ItemsSource = await ReviewAPI.GetUserReposAsync(AccessToken);
                });

            });

        };
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

    static async Task SetUserProfileImage(Image img)
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
            JsonDocument document = JsonDocument.Parse(responseContent);
            
            JsonElement root = document.RootElement;
            string imgUrl = root.GetProperty("avatar_url").GetString();

            img.Dispatcher.Invoke(() =>
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imgUrl, UriKind.Absolute);
                bitmap.EndInit();

                img.Source = bitmap;
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
}

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Controls;

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

        Client = new HttpClient();
        Client.BaseAddress = new Uri("https://github.com");

        var response = await Client.PostAsync(tokenUrl, new FormUrlEncodedContent(requestParams));
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

    static public void SetAuthLink(Button button, TextBlock textBlock)
    {
        // Construct the authorization URL
        string authorizationUrl = $"https://github.com/login/oauth/authorize?client_id={ClientId}&scope=user,repo,pull_requests:write,pull_requests:read";

        // Set button content
        button.Content = "Authorize";

        // Handle button click event
        button.Click += (sender, e) =>
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

                // Get username using the access token
                Username = await GetUsernameAsync();

                textBlock.Dispatcher.Invoke(() =>
                {
                    textBlock.Text = "Access token retrieved.";
                });

            });
        };
    }

    static async Task<string> GetUsernameAsync()
    {

        using var request = new HttpRequestMessage(HttpMethod.Get, "/users/");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

        using var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        Trace.WriteLine(content);
        return content;
    }
}

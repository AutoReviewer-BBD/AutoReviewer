using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Octokit;
namespace GithubAuthentication.Controllers

{
    public class HomeController : Controller
    {
        public static string AccessToken { get; set; }

        public Task<ActionResult> Index()
        {
            ViewBag.ClientId = ConfigurationManager.AppSettings["GithubClientId"].ToString();
            ViewBag.RedirectUrl = ConfigurationManager.AppSettings["RedirectUri"].ToString();

            return Task.FromResult<ActionResult>(View());
        }
        public async Task<ActionResult> GithubLogin(string code)
        {
            var client = new HttpClient();
            var parameters = new Dictionary<string, string>
            {
            { "client_id", ConfigurationManager.AppSettings["GithubClientId"].ToString() },
            { "client_secret", ConfigurationManager.AppSettings["GithubClientSecret"].ToString()},
            { "code", code },
            { "redirect_uri", ConfigurationManager.AppSettings["RedirectUri"].ToString()}
            };
            var content = new FormUrlEncodedContent(parameters);
            var response = await client.PostAsync("https://github.com/login/oauth/access_token", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var values = HttpUtility.ParseQueryString(responseContent);
            var access_token = values["access_token"];
            var client1 = new GitHubClient(new ProductHeaderValue("reviewerauto126"));
            if(access_token!= null) 
                AccessToken = access_token;

            var tokenAuth = new Credentials(AccessToken);

            client1.Credentials = tokenAuth;
            var user = await client1.User.Current();
            var email = user.Email;
            var repos = await client1.Repository.GetAllForCurrent();

            var repositories = await client1.Repository.GetAllForCurrent();
            ViewBag.RepositoriesJson = JsonConvert.SerializeObject(repositories);

            // Create a list to hold repositories with branches
            var repositoriesWithBranches = new List<RepositoryWithBranches>();

            // Fetch branches for each repository
            foreach (var repo in repositories)
            {
                var branches = await client1.Repository.Branch.GetAll(repo.Owner.Login,repo.Name);
                var repoWithBranches = new RepositoryWithBranches
                {
                    Repository = repo,
                    Branches = branches.Select(b => b.Name).ToList()
                };
                repositoriesWithBranches.Add(repoWithBranches);
            }
            

            return View(repositoriesWithBranches);
        }


        public async Task<ActionResult> Repositories()
{
    // Get the access token from session or wherever you store it
    var accessToken = ""; // Retrieve the access token from the session or wherever you store it

    if (string.IsNullOrEmpty(accessToken))
    {
        // Redirect to login page if access token is not available
        return RedirectToAction("Index");
    }

    try
    {
        var client = new GitHubClient(new ProductHeaderValue("reviewerauto126"));
        var tokenAuth = new Credentials(accessToken);
        client.Credentials = tokenAuth;

        // Get the current user's repositories
        var repositories = await client.Repository.GetAllForCurrent();

        // Pass the repositories to the view
        return View(repositories);
    }
    catch (Exception ex)
    {
        // Handle any errors, maybe redirect to an error page
        return View("Error");
    }
}

    }
}

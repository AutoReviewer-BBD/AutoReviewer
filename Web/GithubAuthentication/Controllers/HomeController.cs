using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Octokit;
namespace GithubAuthentication.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            ViewBag.ClientId = ConfigurationManager.AppSettings["GithubClientId"].ToString();
            ViewBag.RedirectUrl = ConfigurationManager.AppSettings["RedirectUri"].ToString();
            return View();
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
            var tokenAuth = new Credentials(access_token);
            client1.Credentials = tokenAuth;
            var user = await client1.User.Current();
            var email = user.Email;
            return View(user);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
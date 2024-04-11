using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Octokit;

namespace GithubAuthentication.Controllers
{
    public class RepositoryWithBranches : Controller
    {
    public Repository Repository { get; set; }
        public List<string> Branches { get; set; }
    }
}
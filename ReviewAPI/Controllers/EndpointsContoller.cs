using Microsoft.AspNetCore.Mvc;

namespace ReviewAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class userController : ControllerBase
{
    private readonly ILogger<userController> _logger;

    public userController(ILogger<userController> logger)
    {
        _logger = logger;

        DatabaseAPI.Initialize();

    }

    /// <summary>
    /// Retrieves repositories for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires an authorization header with a valid access token.
    /// </remarks>
    [HttpGet("repos", Name = "GetUserRepos")]
    [ProducesResponseType(typeof(IEnumerable<Repository>), 200)]
    [ProducesResponseType(401)]
    public async Task<IEnumerable<Repository>> GetUserRepositories()
    {
        string accessToken = HttpContext.Request.Headers["Authorization"];

        string username = await GitHubAPI.GetUsernameAsync(accessToken).ConfigureAwait(false);

        // Do DB call here

        // For demonstration purposes, returning some data
        var repositories = new List<Repository>
        {
            new Repository { 
                    Owner= "AutoReviewer-BBD",
                    Name = "AutoReviewer"
                }
        };
        return repositories;
    }

}

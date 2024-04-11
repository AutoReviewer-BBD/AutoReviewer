using System.Text;
using Api.Interfaces;
using System.Security.Claims;
﻿using RealConnection.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controller
{
    [Route("api/[controller]")]
    [ApiController] //, Authorize]
    public class RepositoryController : ControllerBase
    {
        private readonly IRepositoryRepository repositoryRepository;

        public RepositoryController(IRepositoryRepository repositoryRepository, IConfiguration secrets){
            this.repositoryRepository = repositoryRepository;
        }

        private string GetCurrentUser()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var githubUserID = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (githubUserID == null)
            {
                throw new Exception("Cannot find userId within token");
            }

            return int.Parse(githubUserID);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Repository))]
        public IActionResult GetUserRepositories()
        {
            string gitHubUserName = "";
            try {
                gitHubUserName = await GitHubAuther.GetAuthorizedUsername(githubToken);
            }
            catch (Exception e)
            {
                return BadRequest("Cannot find userName with token from github");
            }
            var repositories = repositoryRepository.GetRepositoriesForUser(gitHubUserName);

            return Ok(repositories);
        }      
    }
}
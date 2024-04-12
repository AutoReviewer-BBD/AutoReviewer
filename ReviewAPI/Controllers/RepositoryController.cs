using System.Text;
using Api.Interfaces;
using System.Security.Claims;
﻿using RealConnection.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Api.GitHub;
using Microsoft.Extensions.Primitives;

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

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Repository))]
        public async Task<IActionResult> GetUserRepositories()
        {
        if (!Request.Headers.TryGetValue("Authorization", out StringValues authorizationHeader))
            {
                return BadRequest("Authorization header is missing");
            }

            // Extract the token from the authorization header
            AuthenticationHeaderValue authHeaderValue = AuthenticationHeaderValue.Parse(authorizationHeader);
            if (authHeaderValue.Scheme != "Bearer" || string.IsNullOrEmpty(authHeaderValue.Parameter))
            {
                return BadRequest("Invalid authorization header");
            }

            string gitHubToken = authHeaderValue.Parameter;
            string gitHubUserName = "";
            try {
                gitHubUserName = await GitHubAPI.GetUsernameAsync(gitHubToken);
            }
            catch {
                return BadRequest("Cannot find userName with token from github");
            }
            ICollection<ProcedureViewUsersRepositories> repositories = 
                repositoryRepository.GetRepositoriesForUser(gitHubUserName);

            return Ok(repositories);
        }      
    }
}
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

        // private string GetCurrentUser()
        // {
        //     var claimsIdentity = User.Identity as ClaimsIdentity;
        //     var githubUserID = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //     if (githubUserID == null)
        //     {
        //         throw new Exception("Cannot find userId within token");
        //     }

        //     return int.Parse(githubUserID);
        // }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Repository))]
        public async Task<IActionResult> GetUserRepositories()
        {
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                return BadRequest("Authorization header is missing");
            }

            // Extract the token from the authorization header
            var authHeaderValue = AuthenticationHeaderValue.Parse(authorizationHeader);
            if (authHeaderValue.Scheme != "Bearer" || string.IsNullOrEmpty(authHeaderValue.Parameter))
            {
                return BadRequest("Invalid authorization header");
            }

            string gitHubToken = authHeaderValue.Parameter;
            string gitHubUserName = "";
            try {
                gitHubUserName = await GetUsernameAsync(gitHubToken);
            }
            catch (Exception e)
            {
                return BadRequest("Cannot find userName with token from github");
            }
            var repositories = repositoryRepository.GetRepositoriesForUser(gitHubUserName);

            return Ok(repositories);
        }      

        private static async Task<string> GetUsernameAsync(string token)
    {
        try
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
}
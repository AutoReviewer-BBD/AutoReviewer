using System.Text;
using Api.Interfaces;
using System.Security.Claims;
﻿using RealConnection.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Api.GitHub;
using Api.Populating;

namespace Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration secrets;
        private readonly IGitHubUserRepositoy gitHubUserRepositoy;

        public LoginController(IGitHubUserRepositoy gitHubUserRepositoy, IConfiguration secrets){
            this.gitHubUserRepositoy = gitHubUserRepositoy;
            this.secrets = secrets;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string githubToken)
        {
            IActionResult response = Unauthorized();
            string gitHubUsername = await GitHubAuther.GetAuthorizedUsername(githubToken);

            if (gitHubUsername != null)
            {
                GitHubUser gitHubUser = gitHubUserRepositoy.LoginUser(gitHubUsername);
                await PerformPopulating(gitHubUser, githubToken);

                string tokenString = GenerateJSONWebToken(gitHubUser);

                response = Ok(new
                {
                    githubUsername = gitHubUsername,
                    gitHubUserToken = tokenString
                });
            }

            return response;
        }

        private string GenerateJSONWebToken(GitHubUser gitHubUser)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secrets["Jwt:Key"]));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, gitHubUser.GitHubUserId.ToString()),
                new Claim("username", gitHubUser.GitHubUsername),
            };

            SecurityToken token = new JwtSecurityToken(
                secrets["Jwt:Issuer"],
                secrets["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task PerformPopulating(GitHubUser gitHubUser, string githubToken){
            List<Dictionary<string, string>> repositoryDictionary = await GitHubAPI.GetUserRepositoriesEndPoint(githubToken);
            List<Repository> repositories = repositoryDictionary.Select(
                repository => new Repository(){
                   RepositoryName = repository["RepositoryName"] ,
                   RepositoryOwnerUsername = repository["RepositoryOwnerUsername"]
                }
            ).ToList();
 
            Populator.AddRegistrationsForUser(gitHubUser.GitHubUsername, repositories);
        }
    }
}
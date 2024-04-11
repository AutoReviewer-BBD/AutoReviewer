using System.Text;
using Api.Interfaces;
using System.Security.Claims;
﻿using RealConnection.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Api.GitHub;
using RealConnection.Data;
using Api.Repositories;

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
                AutoReviewerDbContext databaseContext = new AutoReviewerDbContext();
                GitHubUser gitHubUser = gitHubUserRepositoy.LoginUser(gitHubUsername);

                var tokenString = GenerateJSONWebToken(gitHubUser);

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
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secrets["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, gitHubUser.GitHubUserId.ToString()),
                new Claim("username", gitHubUser.GitHubUsername),
            };

            var token = new JwtSecurityToken(
                secrets["Jwt:Issuer"],
                secrets["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
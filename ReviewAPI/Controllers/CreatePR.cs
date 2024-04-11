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
using Api.Populating;

namespace Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreatePRController : ControllerBase
    {
        private readonly IConfiguration secrets;
        private readonly IGitHubUserRepositoy gitHubUserRepositoy;
        private readonly IRepositoryRepository repositoryRepository;
        private readonly ISkillRepository skillRepository;

        public CreatePRController(
            IRepositoryRepository repositoryRepository, 
            IGitHubUserRepositoy gitHubUserRepositoy,
            ISkillRepository skillRepository,
            IConfiguration secrets)
        {
            this.repositoryRepository = repositoryRepository;
            this.gitHubUserRepositoy = gitHubUserRepositoy;
            this.skillRepository = skillRepository;
            this.secrets = secrets;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreatePR(
            string accessToken,
            string repositoryName, 
            string skillName, 
            string branchName,
            string prTitle
            )
        {
            IActionResult response = Unauthorized();
            string gitHubUsername = await GitHubAuther.GetAuthorizedUsername(accessToken);
            GitHubUser gitHubUser = gitHubUserRepositoy.LoginUser(gitHubUsername);
            Repository repository = repositoryRepository.GetRepositoryWithName(repositoryName);
            Skill skill = skillRepository.GetSkillWithName(skillName);

            bool result = await GitHubAPI.CreatePR(
                repository.RepositoryOwnerUsername,
                repository.RepositoryName,
                prTitle,
                branchName,
                skill.SkillName,
                accessToken,
                skill.SkillId,
                repository.RepositoryId,
                gitHubUser.GitHubUserId
            );

            string strResult = result ? "success" : "fail";
            return response = Ok(new {
                strResult
            });
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

        // private async Task PerformPopulating(GitHubUser gitHubUser, string githubToken){
        //     List<> repositoryDictionary = await GitHubAPI.GetUserRepositoriesEndPoint(githubToken);
        //     List<Repository> repositories = repositoryDictionary.Select(
        //         repository => new Repository(){
        //            RepositoryName = repository.Split("/")[1] ,
        //            RepositoryOwnerUsername = repository.Split("/")[0]
        //         }
        //     ).ToList();

        //     Populator.AddRegistrationsForUser(gitHubUser.GitHubUsername, repositories);
        // }

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
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
            Repository? repository = repositoryRepository.GetRepositoryWithName(repositoryName);
            Skill? skill = skillRepository.GetSkillWithName(skillName);

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
    }
}
using System.Text;
using Api.Interfaces;
using System.Security.Claims;
﻿using RealConnection.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Api.GitHub;

namespace Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSkillController : ControllerBase
    {
        private readonly IUserSkillsRepository userSkillsRepository;
        private readonly ISkillRepository skillRepository;
        private readonly IGitHubUserRepositoy gitHubUserRepositoy;

        public UserSkillController(
            IUserSkillsRepository userSkillsRepository,
            IGitHubUserRepositoy gitHubUserRepositoy,
            ISkillRepository skillRepository,
            IConfiguration secrets)
        {
            this.userSkillsRepository = userSkillsRepository;
            this.gitHubUserRepositoy = gitHubUserRepositoy;
            this.skillRepository = skillRepository;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddUserWithSkill(string token, string skillName)
        {
            IActionResult response = Unauthorized();

            Skill? skill = skillRepository.GetSkillWithName(skillName);
            string gitHubUsername = await GitHubAuther.GetAuthorizedUsername(token);

            if (gitHubUsername != null && skill != null)
            {
                GitHubUser gitHubUser = gitHubUserRepositoy.LoginUser(gitHubUsername);
                UserSkill userSkill = new UserSkill(){
                    GitHubUserId = gitHubUser.GitHubUserId,
                    SkillId = skill.SkillId
                };

                userSkillsRepository.AddUserWithSkill(userSkill);

                response = Ok(
                    new {
                        userSkill
                    });
            }

            return response;
        }
    }
}
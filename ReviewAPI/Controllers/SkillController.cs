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
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly ISkillRepository skillsRepository;

        public SkillController(ISkillRepository skillsRepository, IConfiguration secrets){
            this.skillsRepository = skillsRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<Skill>))]
        public IActionResult AddUserWithSkill()
        {
            var skills = skillsRepository.GetAllSkills();

            return Ok(skills);
        }      
    }
}
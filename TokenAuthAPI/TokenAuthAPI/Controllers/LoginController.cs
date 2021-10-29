using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TokenAuthAPI.Models;
using TokenAuthAPI.Repositories;
using TokenAuthAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenAuthAPI.Controllers
{
    [ApiController]
    //[Route("v1/[controller]")]
    [Route("v1")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> AuthenticateAsync([FromBody] User model)
        {
            var user = UserRepository.Get(model.Username, model.Password);

            if (user == null)
                return NotFound(new { message = "Usuário ou Senha inválidos." });

            var token = TokenService.GenerateToken(user);

            user.Password = "";

            return new { user = user, token = token };
        }
    }
}

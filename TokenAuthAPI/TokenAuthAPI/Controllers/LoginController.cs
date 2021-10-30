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
using Microsoft.IdentityModel.Tokens;

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

            var refreshToken = TokenService.GenerateRefreshToken();
            TokenService.SaveRefreshToken(model.Username, refreshToken);

            user.Password = "";

            return new { user = user, token = token, refreshToken = refreshToken };
        }

        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(string token, string refreshToken)
        {
            var principal = TokenService.GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;
            var savedRefreshToken = TokenService.GetRefreshToken(username);

            if (savedRefreshToken != refreshToken)
                throw new SecurityTokenException("Refresh Token inválido.");

            var newToken = TokenService.GenerateToken(principal.Claims);

            var newRefreshToken = TokenService.GenerateRefreshToken();
            TokenService.DeleteRefreshToken(username, refreshToken);
            TokenService.SaveRefreshToken(username, newRefreshToken);

            return new ObjectResult(new { token = newToken, refreshToken = newRefreshToken });
        }
    }
}

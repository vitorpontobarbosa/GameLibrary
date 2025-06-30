using GameLibrary.Api.DTOs.Auth;
using GameLibrary.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GameLibrary.Api.Controllers
{
    /// <summary>
    /// Endpoints relacionados ao usuario.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Registra Usuario</summary>
        /// <response code="200">Usuário criado com sucesso</response>
        /// <response code="400">Dados inválidos ou usuário já existe</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        ///<summary>Realiza o login do usuário</summary>
        ///<response code="200">Login realizado com sucesso</response>
        ///<response code="401">Usuário ou senha inválidos</response>
        ///<response code="400">Dados inválidos</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Contains("inválidos"))
                    return Unauthorized(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
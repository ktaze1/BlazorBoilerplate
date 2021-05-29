using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Boilerplate.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Boilerplate.AuthServer.Security;
using Boilerplate.AuthServer.Security.Model;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Boilerplate.Shared.Models.Fundamentals;
using System.Security.Claims;

namespace Boilerplate.AuthServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly aspnetBoilerplateContext _context;

        public UsersController(aspnetBoilerplateContext context, UserManager<User> userManager,
            SignInManager<User> signInManager, IJwtAuthManager jwtAuthManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtAuthManager = jwtAuthManager;
        }

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtAuthManager _jwtAuthManager;


        [TempData]
        public string StatusMessage { get; set; }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Surname = model.Surname,
                Name = model.Name,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(user);
            }
            else
            {
                return BadRequest();
            }

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<Response<LoginResultDto>> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != default(User))
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name,model.Email)
                    };

                    var jwtResult = _jwtAuthManager.GenerateTokens(model.Email, claims, DateTime.Now);
                    return await Response<LoginResultDto>.Run(new LoginResultDto() { AccessToken = jwtResult.AccessToken });
                }
            }
            else
            {
                return await Response<LoginResultDto>.Catch(new ResponseError() { Message = "Invalid Credentials" });
            }

            return await Response<LoginResultDto>.Catch(new ResponseError() { Message = "Bad Request" });
        }


        [HttpPost("LogOff")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        //[HttpPost("SendVerificationEmail")]
        //public async Task<IActionResult> SendVerificationEmail([FromBody] ConfirmMailDto model)
        //{
        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        return NotFound(_userManager.GetUserId(User));
        //    }

        //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        //    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);

        //    //await _emailSender.SendEmailAsync(model.Email, "ConfirmEmailTitle", string.Concat("ConfirmEmailBody"));
        //    StatusMessage = "VerificationSent";
        //    return Ok();
        //}
    }
}

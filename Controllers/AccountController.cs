using Authorization_Refreshtoken.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Authorization_Refreshtoken.Models;
using Authorization_Refreshtoken.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Authorization_Refreshtoken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJWTService _jwtService;
        public AccountController(UserManager<AppUser> userManager, IJWTService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegiterDTO request)
        {
            if (!ModelState.IsValid) 
            {
                var errors = ModelState.Values
                             .SelectMany(x => x.Errors)
                             .Select(x => new {
                                 ErrorMessage = x.ErrorMessage,
                                 PropertyName = x.Exception?.TargetSite?.Name
                             })
                             .ToList();

                return BadRequest(errors);
            }
                AppUser user=RegiterDTO.ConvertToAppUser(request);
               var result= await _userManager.CreateAsync(user,request.Password);
            if(result.Succeeded)
            {
               
                var res=_jwtService.CreateToken(user);
                return Ok(res);
            }
            else
            {
                return BadRequest(result.Errors.Select(x => x.Description));
            }
        }

       [HttpPost("login")]
       public async Task<IActionResult> Login(LoginDTO request)
       {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                             .SelectMany(x => x.Errors)
                             .Select(x => new
                             {
                                 ErrorMessage = x.ErrorMessage,
                                 PropertyName = x.Exception?.TargetSite?.Name
                             })
                             .ToList();

                return BadRequest(errors);
            }

           

            AppUser? user=await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
                return NotFound("no such user");
            bool result=await _userManager.CheckPasswordAsync(user,request.Password);
            if(result)
            {
                var res = _jwtService.CreateToken(user);
                return Ok(res);
            }
            return BadRequest("pas is wrong");
        }


       
    }
}

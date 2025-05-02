using Authorization_Refreshtoken.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Authorization_Refreshtoken.Models;
using Authorization_Refreshtoken.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Claims;
using Authorization_Refreshtoken.Data;

namespace Authorization_Refreshtoken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IJWTService _jwtService;
        public AccountController(UserManager<AppUser> userManager, IJWTService jwtService, AppDbContext context)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _context = context;
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
                user.RefreshToken= res.RefreshToken;
                user.ExpirationDateRefreshToken= res.ExpirationDateRefreshToken;
                _context.Users.Attach(user);

                _context.Entry(user).Property(u => u.RefreshToken).IsModified = true;
                _context.Entry(user).Property(u => u.ExpirationDateRefreshToken).IsModified = true;

                await _context.SaveChangesAsync();
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
                user.RefreshToken= res.RefreshToken;
                user.ExpirationDateRefreshToken=res.ExpirationDateRefreshToken;
                _context.Users.Attach(user);

                _context.Entry(user).Property(u => u.RefreshToken).IsModified = true;
                _context.Entry(user).Property(u => u.ExpirationDateRefreshToken).IsModified = true;

                await _context.SaveChangesAsync();

                return Ok(res);
            }
            return BadRequest("pas is wrong");
        }


        [HttpPost("generateNewToken")]
        public async Task<IActionResult> GenerateNewAccessToken(TokenModel model)
        {
            if (model == null)
                return BadRequest("invalid request");

            ClaimsPrincipal?principle=_jwtService.GetPrinciplesFromJWTToken(model?.Token);

            if (principle == null)
                return BadRequest();

            var id = principle.FindFirstValue(ClaimTypes.NameIdentifier);

            AppUser?user=await _userManager.FindByIdAsync(id);

            if(user is null || user.RefreshToken!=model.RefreshToken || user.ExpirationDateRefreshToken <= DateTime.Now)
            {
                return BadRequest("bad credential");
            }

            var authResponse = _jwtService.CreateToken(user);
            user.RefreshToken= authResponse.RefreshToken;
            user.ExpirationDateRefreshToken=authResponse.ExpirationDateRefreshToken;

            await _userManager.UpdateAsync(user);
            return Ok(authResponse);
        }

       
    }
}

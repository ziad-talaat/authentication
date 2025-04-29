using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authorization_Refreshtoken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            List<string> someDummyData = new List<string>()
            {
               "1234","124124","1424124124","124124124"
            };
            return Ok(someDummyData);
        }
    }
}

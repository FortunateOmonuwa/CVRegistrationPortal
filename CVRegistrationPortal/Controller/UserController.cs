using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CVRegistrationPortal.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController( IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("User/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await userService.GetUser(id);
                if (user.IsSuccessful == false)
                {
                    return BadRequest(user);
                }
                else
                {
                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }
    }
}

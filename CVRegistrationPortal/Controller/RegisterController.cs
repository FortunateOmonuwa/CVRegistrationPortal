using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CVRegistrationPortal.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegister register;
        public RegisterController(IRegister register)
        {
            this.register = register;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] UserCreateDTO user)
        {
            try
            {
                var res = await register.Register(user);
                return Ok(res);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

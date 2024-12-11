using CVRegistrationPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CVRegistrationPortal.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService register;
      
        public RegisterController(IRegisterService register)
        {
            this.register = register;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] UserCreateDTO user)
        {
            try
            {
                var res = await register.Register(user);
                if (res.IsSuccessful is false)
                {
                    return BadRequest(res);
                }
                else
                {
                    return Ok(res);

                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Verify")]
        public async Task<IActionResult> VerifyAccount(string token)
        {
            try
            {
                var res = await register.VerifyAccount(token);
                if (res.ResultCode == 498)
                {
                    return BadRequest(res);
                }
                else if(res.IsSuccessful is true)
                {

                    return Ok(res);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Resend-Verification-Token")]
        public async Task<IActionResult> ResendSendVerificationMail(string email)
        {
            try
            {
                var res = await register.ResendSendVerificationToken(email);
                if (res.IsSuccessful)
                {
                    return Ok(res);
                }
                else
                {
                    return BadRequest(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

using Calendar.Api.DTO;
using Calendar.Api.Logics;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class AssignmentController : Controller
    {
        private readonly AssignmentLogic _logic;
        public AssignmentController(AssignmentLogic logic)
        {
            _logic = logic;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(AssignmentDTO_LOGIN dto)
        {
            try
            {
                var data = await _logic.LoginUser(dto);
                if(data == null)
                {
                    return BadRequest("Invalid Email or Password");
                }
                return Ok(data);
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromForm] AssignmentDTO_POST dto)
        {
            try
            {
                string response = await _logic.RegisterUser(dto);
                if (response == "success")
                {
                    return Ok();
                }
                else if(response == "exists")
                {
                    return BadRequest("Account already exists");
                }
                else
                {
                    return BadRequest("Failed to register account");
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }
    }
}

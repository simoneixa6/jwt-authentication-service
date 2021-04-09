using jwt_authentication_service.Helpers;
using jwt_authentication_service.Models;
using jwt_authentication_service.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;


namespace jwt_authentication_service.Controllers
{
    [ApiController]
    [Route("authentication")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<List<User>> Get() =>
            _userService.Get();

        [Authorize]
        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<User> Get(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPost("authenticate", Name = "Authenticate")]
        public ActionResult<User> Authenticate(User user)
        {
            var response = _userService.Authenticate(user);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpPost("verifyToken/{token}", Name = "VerifyTokenValidity")]
        public IActionResult VerifyTokenValidity(String token)
        {
            Boolean response = _userService.VerifyTokenValidity(token);

            if (response == false)
                return Unauthorized();

            return Ok("This token is valid");
        }

        [Authorize]
        [HttpPost(Name = "PostUser")]
        public ActionResult<User> Create(User user)
        {
            _userService.Create(user);

            return CreatedAtRoute("PostUser", new { id = user.Id.ToString() }, user);
        }

        [Authorize]
        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, User userIn)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Update(id, userIn);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Remove(user.Id);

            return NoContent();
        }
    }
}

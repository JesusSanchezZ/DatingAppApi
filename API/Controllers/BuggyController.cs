using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    public class BuggyController : ControllerBase
    {
        private readonly DataContext _context;

        public BuggyController(DataContext context){
            _context = context;
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/auth")]
        public ActionResult<string> GetSecret(){
            return "secret text";
        }

        [HttpGet]
        [Route("api/[controller]/not-found")]
        public ActionResult<AppUser> GetNotFound(){
            var thing = _context.Users.Find(-1);

            if(thing == null) return NotFound();

            return Ok(thing);
        }

        [HttpGet]
        [Route("api/[controller]/server-error")]
        public ActionResult<string> GetServerError(){
            var thing = _context.Users.Find(-1);

            var thingToReturn = thing.ToString();

            return thingToReturn;
        }

        [HttpGet]
        [Route("api/[controller]/bad-request")]
        public ActionResult<string> GetBadRequest(){
            return BadRequest("this Was not a good request");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class BugsController : BaseApiController
    {
        private readonly DataContext _context;

        public BugsController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var noUser = _context.Users.Find(-1);
            if (noUser == null) return NotFound();
            return noUser;
        }
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var noUser = _context.Users.Find(-1);
            var nullReturn = noUser.ToString();
            return nullReturn;
        }
        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This is not a good request");
        }
    }
}
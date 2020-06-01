using Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("homes")]
    public class HomesController : Controller
    {
        [HttpGet("{id}")]
        public Home Index(string id)
        {
            return new Home { Name = "Our Home!" };
            //return new { 
            //    Name = $"home {id}",
            //    StartupHeader = Request.Headers["StartupHeader"],
            //    CtorHeader = Request.Headers["Middleware-Ctor"],
            //    MethodHeader = Request.Headers["Middleware-Method"],
            //};
        }
    }
}

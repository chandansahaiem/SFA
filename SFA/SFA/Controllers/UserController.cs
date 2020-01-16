using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.Extensions;
using SFA.Filters;
using SFA.Models;
using SFA.Services;

namespace SFA.Controllers
{
    [Route("user")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService = null;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("")]
        [CheckAccess]
        public IActionResult Index()
        {
            return View();
        }
        [Route("detail/{id?}")]
        [CheckAccess]
        public IActionResult Detail(Guid? id)
        {
            ViewBag.Id = id;
            return View();
        }

        [Route("api/menus")]
        public async Task<IActionResult> GetMenu()
        {
            var sessionUser = HttpContext.Session.Get<User>("SESSIONSFAUSER");
            var userId = sessionUser.Id;
            var menus = await _userService.GetMenuByUser(userId);
            return Json(menus);
        }

        [Route("all")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            return new JsonResult(users);
        }


        [Route("api/search")]
        public async Task<IActionResult> Search([FromBody]UserQuery query)
        {
            var users = await _userService.Search(query);
            return new JsonResult(users);
        }

        [Route("api/get/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _userService.GetById(id);
            return new JsonResult(user);
        }

        [HttpPost]
        [Route("api/save")]
        public async Task<IActionResult> Save([FromBody]User user)
        {
            var result = await _userService.Save(user);
            return new JsonResult(result);
        }


    }
}

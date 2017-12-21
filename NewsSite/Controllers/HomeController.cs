﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsSite.Models;

namespace NewsSite.Controllers
{
    [Route("api/home")]
    public class HomeController : Controller
    {

        UserManager<User> userManager;
        SignInManager<User> signInManager;
        RoleManager<UserRole> roleManager;
        NewsSiteContext context;

        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<UserRole> roleManager, NewsSiteContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.context = context;
        }

        [HttpGet]
        [Route("resetdb")]
        public async Task<IActionResult> ClearDbAndRetrieveDefaultUsers()
        {

            context.RemoveRange(context.Users);

            var users = new List<User>() {
                //new User { FirstName = "Adam", UserName = "adam@gmail.com" },
                new User { FirstName = "Peter", UserName = "peter@gmail.com" },
                new User { FirstName = "Susan", Age = 48, UserName = "susan@gmail.com" },
                new User { FirstName = "Viktor", Age = 15, UserName = "viktor@gmail.com" },
                new User { FirstName = "Xerxes", UserName = "xerxes@gmail.com" },
            };

            foreach (var user in users)
            {
                var result = await userManager.CreateAsync(user);
            }

            return Ok(context.Users);
        }

        [HttpGet]
        [Route("getemaillist")]
        public IActionResult GetEmailList()
        {
            return Ok(context.Users
                .Select(o => new IndexVM
                {
                    Id = o.Id,
                    Email = o.UserName
                })
                .OrderBy(o => o.Email)
                .ToArray());
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn(string viewModel)
        {

            if (!ModelState.IsValid)
                return View(viewModel);

            var user = await userManager.FindByNameAsync(viewModel);

            if (user != null)
            {
                await signInManager.SignInAsync(user, false);
                return Ok($"{user.FirstName}");
            }
            else
            {
                return NotFound("User was not found");
            }

        }
    }
}

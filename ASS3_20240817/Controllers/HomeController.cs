using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ASS2_20240802.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public HomeController() { }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        /*
        public ActionResult Index()
        {
            return View();
        }
        */

public async Task<ActionResult> Index()
{
    var userId = User.Identity.GetUserId();
    if (userId != null)
    {
        var user = await UserManager.FindByIdAsync(userId);
        if (user != null) // 确保用户确实存在
        {
            var roles = await UserManager.GetRolesAsync(userId);
            if (!roles.Any()) // 检查用户是否有任何角色
            {
                return RedirectToAction("ChooseRole");
            }
        }
    }
    return View();
}

        public ActionResult ChooseRole()
        {


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetRole(string userRole)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // 先移除现有的角色
            var currentRoles = await UserManager.GetRolesAsync(userId);
            await UserManager.RemoveFromRolesAsync(userId, currentRoles.ToArray());

            // 添加新选择的角色
            var result = await UserManager.AddToRoleAsync(userId, userRole);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(userId);
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false); // 重新登录用户以更新角色信息
                return RedirectToAction("Index");
            }
            else
            {
                // 处理错误
                return View("ChooseRole");
            }
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Map()
        {
            return View();
        }

        public ActionResult GenAI()
        {
            return View();
        }


    }
}
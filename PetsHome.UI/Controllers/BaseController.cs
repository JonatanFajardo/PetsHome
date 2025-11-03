using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using System;
using System.Security.Claims;

namespace PetsHome.UI.Controllers
{
    public class BaseController : Controller
    {
        protected int? CurrentUserId
        {
            get
            {
                var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    return userId;
                }

                return null;
            }
        }

        /// <summary>
        /// Muestra alertas en el navegador
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        protected void ShowAlert(string text, AlertMessageType type)
        {
            var message = new AlertMessageExtensions
            {
                Text = text,
                Type = type
            };
            TempData.Put("ShowAlert", message);
        }

        protected IActionResult ShowAlert(string text, AlertMessageType type, dynamic model)
        {
            var message = new AlertMessageExtensions
            {
                Text = text,
                Type = type
            };
            TempData.Put("ShowAlert", message);
            return View("Create", model);
        }

        public JsonResult AjaxResult(dynamic model, Boolean success)
        {
            return Json(new { item = model, success = success });
        }

        public JsonResult AjaxResult(Boolean success)
        {
            return Json(new { success = success });
        }
    }
}


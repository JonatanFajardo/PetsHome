using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web;

namespace PetsHome.Business.Extensions
{
    public static class HtmlExtensions
    {
        public static HtmlString ShowAlert(this IHtmlHelper html)
        {
            var message = html.ViewContext.TempData.Get<AlertMessageExtensions>("ShowAlert");

            if (message == null)
            {
                return new HtmlString("");
            }

            string alert = $"<script>appConfig.alert('{HttpUtility.JavaScriptStringEncode(message.CssClass)}', '{HttpUtility.JavaScriptStringEncode(message.Text)}')</script>";
            return new HtmlString(alert);
        }
    }
}
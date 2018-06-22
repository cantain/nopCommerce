using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Security.Captcha
{
    /// <summary>
    /// QQcaptchaControl control
    /// </summary>
    public class QQcaptchaControl
    {
        private const string RECAPTCHA_API_URL = "https://ssl.captcha.qq.com/TCaptcha.js";

        /// <summary>
        /// Identifier
        /// </summary>
        public string Id { get; set; }

        public string Aid { get; set; }
        public string Callback { get; set; }

        /// <summary>
        /// Render control
        /// </summary>
        /// <returns></returns>
        public string RenderControl()
        {
            var scriptCallbackTag = new TagBuilder("script")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            scriptCallbackTag.Attributes.Add("type", MimeTypes.TextJavascript);
            var callback = string.IsNullOrWhiteSpace(Callback) ? "" : Callback + "(res);";
            scriptCallbackTag.InnerHtml.AppendHtml(
                $"var qq_Captcha_Callback =  function(res){{ \r\nif(res.ret === 0){{ \r\n$(\"input[name='qq_captcha_ticket_field']\").val(res.ticket); \r\n$(\"input[name='qq_captcha_rand_str_field']\").val(res.randstr); \r\n$('#{Id}').html('验证成功')}} \r\n{callback}}}");
            var ticketTag = new TagBuilder("input")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            ticketTag.Attributes.Add("type", "hidden");
            ticketTag.Attributes.Add("name", "qq_captcha_ticket_field");
            var randStrTag = new TagBuilder("input")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            randStrTag.Attributes.Add("type", "hidden");
            randStrTag.Attributes.Add("name", "qq_captcha_rand_str_field");

            var captchaTag = new TagBuilder("div")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            captchaTag.Attributes.Add("id", Id);
            captchaTag.Attributes.Add("data-appid", Aid);
            captchaTag.Attributes.Add("data-cbfn", "qq_Captcha_Callback");
            captchaTag.InnerHtml.Append("点击验证");


            var styleTag = new TagBuilder("style")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            styleTag.InnerHtml.AppendHtml($"#{Id}{{ width:200px;height:50px;background:lightblue;}}");

            var scriptLoadApiTag = new TagBuilder("script")
            {
                TagRenderMode = TagRenderMode.Normal
            };
            scriptLoadApiTag.Attributes.Add("src", RECAPTCHA_API_URL);


            return scriptLoadApiTag.RenderHtmlContent() + "\r\n" 
                //+ styleTag.RenderHtmlContent() + "\r\n"
                + ticketTag.RenderHtmlContent() + "\r\n"
                + randStrTag.RenderHtmlContent() + "\r\n"
                + scriptCallbackTag.RenderHtmlContent() + "\r\n"
                + captchaTag.RenderHtmlContent();
        }
    }
}
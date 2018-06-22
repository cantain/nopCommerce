using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Security.Captcha;

namespace Nop.Web.Framework.TagHelpers.Public
{
    /// <summary>
    /// nop-captcha tag helper
    /// </summary>
    [HtmlTargetElement("nop-qq-captcha", TagStructure = TagStructure.WithoutEndTag)]
    public class NopGenerateQQCaptchaTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private const string CallbackAttributeName = "asp-qq-captcha-callback";

        /// <summary>
        /// 
        /// </summary>
        [HtmlAttributeName(CallbackAttributeName)]
        public string Callback { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="htmlHelper">HTML helper</param>
        public NopGenerateQQCaptchaTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="output">Output</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);
            
            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(_htmlHelper.GenerateQQCaptcha(Callback));
        }
    }
}
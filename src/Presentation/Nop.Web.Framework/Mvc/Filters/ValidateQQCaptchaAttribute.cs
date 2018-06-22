using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Security;
using Nop.Web.Framework.Security.Captcha;

namespace Nop.Web.Framework.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute enabling CAPTCHA validation
    /// </summary>
    public class ValidateQQCaptchaAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute 
        /// </summary>
        /// <param name="actionParameterName">The name of the action parameter to which the result will be passed</param>
        public ValidateQQCaptchaAttribute(string actionParameterName = "captchaValid") : base(typeof(ValidateCaptchaFilter))
        {
            this.Arguments = new object[] { actionParameterName };
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter enabling CAPTCHA validation
        /// </summary>
        private class ValidateCaptchaFilter : IActionFilter
        {
            #region Constants

            private const string TICKET_KEY = "qq_captcha_ticket_field";
            private const string RAND_STR_KEY = "qq_captcha_rand_str_field";

            #endregion

            #region Fields

            private readonly string _actionParameterName;
            private readonly QQCaptchaSettings _captchaSettings;
            private readonly IWebHelper _webHelper;
            #endregion

            #region Ctor

            public ValidateCaptchaFilter(string actionParameterName, QQCaptchaSettings captchaSettings, IWebHelper webHelper)
            {
                this._actionParameterName = actionParameterName;
                this._captchaSettings = captchaSettings;
                this._webHelper = webHelper;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Validate CAPTCHA
            /// </summary>
            /// <param name="context">A context for action filters</param>
            /// <returns>True if CAPTCHA is valid; otherwise false</returns>
            protected bool ValidateCaptcha(ActionExecutingContext context)
            {
                var isValid = false;

                //get form values
                var ticketValue = context.HttpContext.Request.Form[TICKET_KEY];
                var randStrValue = context.HttpContext.Request.Form[RAND_STR_KEY];

                if (!StringValues.IsNullOrEmpty(ticketValue) && !StringValues.IsNullOrEmpty(randStrValue))
                {
                    isValid = captchaVerify(ticketValue, randStrValue);
                }

                return isValid;
            }

            private bool captchaVerify(string ticket, string randStr)
            {
                //https://ssl.captcha.qq.com/ticket/verify
                using (var client = new HttpClient())
                {
                    var result = client.GetStringAsync($"https://ssl.captcha.qq.com/ticket/verify?aid={_captchaSettings.Aid}&AppSecretKey={_captchaSettings.AppSecretKey}&Ticket={ticket}&Randstr={randStr}&UserIP={_webHelper.GetCurrentIpAddress()}").Result;
                    var json = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(result, new { response = 0, evil_level = 0, err_msg = "" });
                    return json?.response == 1;
                }
                return false;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called before the action executes, after model binding is complete
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                //whether CAPTCHA is enabled
                if (_captchaSettings.Enabled && context.HttpContext?.Request != null)
                {
                    //push the validation result as an action parameter
                    context.ActionArguments[_actionParameterName] = ValidateCaptcha(context);
                }
                else
                    context.ActionArguments[_actionParameterName] = false;

            }

            /// <summary>
            /// Called after the action executes, before the action result
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuted(ActionExecutedContext context)
            {
                //do nothing
            }

            #endregion
        }

        #endregion
    }
}
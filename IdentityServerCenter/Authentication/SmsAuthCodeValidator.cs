using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerCenter.Authentication
{
    /// <summary>
    /// 自定义grantType
    /// </summary>
    public class SmsAuthCodeValidator : IExtensionGrantValidator
    {
        public string GrantType => "sms-auth-code";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var phone = context.Request.Raw["phone"];
            var code = context.Request.Raw["auth_code"];
            var errorValidationResult = new GrantValidationResult(TokenRequestErrors.InvalidClient);

            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(code))
            {
                context.Result = errorValidationResult;
                return;
            }

            //验证手机号码

            //创建用户

            await Task.Delay(1);

            var claims = new Claim[]
            {
                new Claim("UserName","Adamson"),
                new Claim("Phone","15295758935")
            };

            context.Result = new GrantValidationResult("userid", GrantType, claims);
        }
    }
}

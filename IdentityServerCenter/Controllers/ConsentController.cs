using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerCenter.Controllers
{
    public class ConsentController : Controller
    {
        private readonly IClientStore _iclientStore;
        private readonly IResourceStore _iresouceStore;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;

        public ConsentController(IClientStore iclientStore,
            IResourceStore iresouceStore,
            IIdentityServerInteractionService identityServerInteractionService)
        {
            _iclientStore = iclientStore;
            _iresouceStore = iresouceStore;
            _identityServerInteractionService = identityServerInteractionService;
        }

        // GET: Consent
        [HttpGet]
        public async  Task<ActionResult> Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            var authorizationRequest =await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);

            var client =await _iclientStore.FindEnabledClientByIdAsync(authorizationRequest.ClientId);

            var resources = await _iresouceStore.FindResourcesByScopeAsync(client.AllowedScopes);

            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Agree(string returnUrl)
        {
            var authorizationRequest =await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
            await _identityServerInteractionService.GrantConsentAsync(authorizationRequest, new IdentityServer4.Models.ConsentResponse
            {
                RememberConsent = true,
                //offline_access refrece token
                ScopesConsented = new List<string>{"openid","profile", "offline_access" },
            });
            return Redirect(returnUrl);
        }
    }
}
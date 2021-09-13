using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OnlineBankUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnlineBankUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Payment()
        {
            var claims = User.Claims.ToList();

            var authenticationProps = (await HttpContext.AuthenticateAsync()).Properties.Items;

            #region "Acces Token İle EndPointe Request"

            var accessToken = authenticationProps.FirstOrDefault(x => x.Key == ".Token.access_token").Value;

            string bakiye = string.Empty;

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var message = await httpClient.GetAsync("http://localhost:2000/api/XBank/Bakiye?musteriId=5");
                bakiye = await message.Content.ReadAsStringAsync();
            }

            ViewBag.Bakiye = bakiye;

            #endregion

            return View();
        }

        public async Task<IActionResult> GetAccesTokenByRefreshToken()
        {
            #region " Refresh Token ile Acces Token Alma ve Authentication Propertyleri yenileme "

            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var httpClient = new HttpClient();

            var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:44310");

            if (!disco.IsError)
            {
                var refreshTokenRequest = new RefreshTokenRequest()
                {
                    ClientId = "OnlineBankamatik",
                    ClientSecret = "onlinebankamatik",
                    RefreshToken = refreshToken,
                    Address = disco.TokenEndpoint
                };

                var tokenResponse = await httpClient.RequestRefreshTokenAsync(refreshTokenRequest);
                var properties = (await HttpContext.AuthenticateAsync()).Properties;

                properties.StoreTokens(new List<AuthenticationToken>
                {
                    new AuthenticationToken()
                    {
                        Name = OpenIdConnectParameterNames.IdToken,
                        Value = tokenResponse.IdentityToken
                    },
                    new AuthenticationToken()
                    {
                        Name = OpenIdConnectParameterNames.AccessToken,
                        Value = tokenResponse.AccessToken
                    },
                      new AuthenticationToken()
                    {
                        Name = OpenIdConnectParameterNames.RefreshToken,
                        Value = tokenResponse.RefreshToken
                    },
                      new AuthenticationToken()
                    {
                        Name = OpenIdConnectParameterNames.ExpiresIn,
                        Value = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn).ToString("O")
                    },
                });

                var principal = (await HttpContext.AuthenticateAsync()).Principal;

                await HttpContext.SignInAsync("OnlineBankamatikCookie", principal, properties);

                return RedirectToAction(nameof(HomeController.RefreshTokenView));
            }

            #endregion

            return RedirectToAction(nameof(HomeController.Error));
        }

        public async Task<IActionResult> RefreshTokenView()
        {
            AuthenticateResult authenticateResult = await HttpContext.AuthenticateAsync();
            IOrderedEnumerable<KeyValuePair<string, string>> properties = authenticateResult.Properties.Items.OrderBy(p => p.Key);

            return View(properties);
        }


        [Authorize(Roles ="admin")]
        public IActionResult AllPaymentDocuments()
        {
            return View();
        }


        [Authorize(Roles = "user,admin")]
        public IActionResult LogData()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult AccessDenied()
        {
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("OnlineBankamatikCookie");
            await HttpContext.SignOutAsync("oidc");

            return RedirectToAction(nameof(HomeController.Index));
        }
    }
}

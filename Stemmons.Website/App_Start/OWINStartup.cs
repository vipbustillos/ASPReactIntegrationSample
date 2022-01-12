using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Stemmons.Website.Startup))]

namespace Stemmons.Website
{
    public class Startup
    {
        string clientId = System.Configuration.ConfigurationManager.AppSettings["OpenIDConnect:ClientId"];
        string clientSecret = System.Configuration.ConfigurationManager.AppSettings["OpenIDConnect:ClientSecret"];
        string authority = System.Configuration.ConfigurationManager.AppSettings["OpenIDConnect:Authority"];
        string redirectUri = System.Configuration.ConfigurationManager.AppSettings["OpenIDConnect:RedirectUri"];



        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    RedirectUri = redirectUri,
                    ClientSecret = clientSecret,
                    PostLogoutRedirectUri = redirectUri,
                    Scope = OpenIdConnectScope.OpenIdProfile,
                    ResponseType = OpenIdConnectResponseType.CodeIdToken,
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true
                    },
                    SaveTokens=true,
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = OnAuthenticationFailed,
                        SecurityTokenValidated = async (SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context) =>
                        {
                            var id = context.AuthenticationTicket.Identity;

                            //save the id_token in the claims to be used in logout post redirect
                            var id_token = context.ProtocolMessage.IdToken;
                            context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim("id_token", id_token));

                            var nid = new ClaimsIdentity(
                                id.AuthenticationType,
                                "name",
                                ClaimTypes.Role);

                            nid.AddClaims(id.Claims);


                            context.AuthenticationTicket = new AuthenticationTicket(
                                nid,
                                context.AuthenticationTicket.Properties);
                        },
                        RedirectToIdentityProvider = n =>
                        {
                            // if logout request, add the id_token_hint for identity server to all redirect back to the website
                            if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                            {
                                var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                                if (idTokenHint != null)
                                {
                                    n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                                }

                            }
                            return Task.FromResult(0);
                        },
                    }
                }
            );
        }

        /// <summary>
        /// Handle failed authentication requests by redirecting the user to the home page with an error in the query string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }

    }
}

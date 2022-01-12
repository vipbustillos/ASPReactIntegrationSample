using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Stemmons.TenancyUtilities;

namespace Stemmons.Website.App_Start
{
    public class StemmonsWebsiteTenancyService : TenancyService
    {
        private StemmonsWebsiteTenancyService() { }

        public static ITenancyService Current = new StemmonsWebsiteTenancyService();

        private static TenancyServiceConfiguration _serviceConfiguration = new TenancyServiceConfiguration()
        {
            ClientID = System.Configuration.ConfigurationManager.AppSettings["ConfigurationAPI:ClientId"],
            ClientSecret = System.Configuration.ConfigurationManager.AppSettings["ConfigurationAPI:ClientSecret"],
            Authority = System.Configuration.ConfigurationManager.AppSettings["ConfigurationAPI:Authority"],
            Scope = System.Configuration.ConfigurationManager.AppSettings["ConfigurationAPI:Scope"],
            ServiceUrl = System.Configuration.ConfigurationManager.AppSettings["ConfigurationAPI:ServiceUrl"]
        };

        public static void SetTenantID(Guid tenantID)
        {
            HttpContext.Current.Response.Cookies.Set(new HttpCookie("StemmonstenantID", tenantID.ToString()));
        }


        public override Guid CurrentTenantID
        {
            get
            {
                var tenantIDString = HttpContext.Current.Request.Cookies.Get("StemmonstenantID")?.Value;

                Guid tenantID;
                if (Guid.TryParse(tenantIDString, out tenantID))
                    return tenantID;
                else
                    return Guid.Empty;
            }
        }

        public override Guid CurrentUserID
        {
            get
            {
                if (!HttpContext.Current.Request.IsAuthenticated)
                    return Guid.Empty;

                var userIDstring = (HttpContext.Current.User.Identity as ClaimsIdentity).FindFirst(System.IdentityModel.Claims.ClaimTypes.NameIdentifier)?.Value;
                Guid userID;
                if (!Guid.TryParse(userIDstring, out userID))
                {
                    throw new Exception("User ID claim is not in correct GUID format");
                }

                return userID;
            }
        }

        public override TenancyServiceConfiguration Configurations => _serviceConfiguration;
    }
}
using Stemmons.TenancyUtilities;
using Stemmons.Website.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Stemmons.Website
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            tenantsRepeater.DataSource = GetCurrentUserTenants;
            tenantsRepeater.DataBind();
        }


        List<TenantInfo> GetCurrentUserTenants
        {
            get
            {
                if (Request.IsAuthenticated)
                    return StemmonsWebsiteTenancyService.Current.GetUserTenants();
                else
                    return new List<TenantInfo>();
            }
        }

        protected void SelectTenant_Click(object sender, EventArgs e)
        {
            var lnkBtn = (sender as LinkButton);

            var tenantID = Guid.Parse(lnkBtn.CommandArgument);
            StemmonsWebsiteTenancyService.SetTenantID(tenantID);

            Response.Redirect("~/Requests.aspx");
        }
    }
}
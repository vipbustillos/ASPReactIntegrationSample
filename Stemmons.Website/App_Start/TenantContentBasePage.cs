using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Stemmons.Website.App_Start
{
    public class TenantContentBasePage:Page
    {
        protected override void OnInit(EventArgs e)
        {
            if (!StemmonsWebsiteTenancyService.Current.IsTenantSelected)
                Response.Redirect("~/");

            base.OnInit(e);
        }
    }
}
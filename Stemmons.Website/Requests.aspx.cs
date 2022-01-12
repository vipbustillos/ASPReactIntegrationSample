using Stemmons.TenancyUtilities;
using Stemmons.Website.App_Start;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Stemmons.Website
{
    public partial class Requests : TenantContentBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
               BindRequestsData();
            }
        }

        void BindRequestsData()
        {
            var connectionString = StemmonsWebsiteTenancyService.Current.GetCurrentTenantDBConnection(TenantDatabaseType.Entities);
            DataTable dt = new DataTable();
            
            using (SqlConnection dbConection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("select * from dbo.requests", dbConection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                sqlDataAdapter.Fill(dt);
            }

            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
    }
}
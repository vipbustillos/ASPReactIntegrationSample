using System;
using System.Collections.Generic;
using System.Text;

namespace Stemmons.TenancyUtilities
{
    public class TenancyServiceConfiguration
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string Authority { get; set; }
        public string Scope { get; set; }
        public string ServiceUrl { get; set; }
    }
}

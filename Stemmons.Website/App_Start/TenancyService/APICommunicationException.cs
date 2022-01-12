using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Stemmons.TenancyUtilities
{
    public class StemmonsAPICommunicationException:Exception
    {
        public int ResponseStatus { get; private set; }
        public string ResponseContent { get; private set; }
        public StemmonsAPICommunicationException(int httpStatus,string content)
        {
            ResponseStatus = httpStatus;
            ResponseContent = content;
        }

        public StemmonsAPICommunicationException(Exception exception) : base("", exception) { }

        public override string Message
        {
            get
            {
                if (ResponseStatus > 0)
                    return $"API Communication Failed with status {ResponseStatus}. The response was {ResponseContent ?? ""}";
                else
                    return $"API Communication Failed. Please check inner exception for more details";
            }
        }
    }
}

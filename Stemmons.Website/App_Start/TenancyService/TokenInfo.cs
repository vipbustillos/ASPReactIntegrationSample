using System;

namespace Stemmons.TenancyUtilities
{
    internal class TokenInfo
    {
        public TokenInfo()
        {
            //CreatedAt = DateTime.UtcNow;
        }

        public string access_token { get; set; }
        public int expires_in { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpireAt { get; set; }

        public bool ShouldRenew
        {
            get
            {

                return DateTime.UtcNow < CreatedAt.AddMilliseconds(expires_in / 2);
            }
        }
    }
}
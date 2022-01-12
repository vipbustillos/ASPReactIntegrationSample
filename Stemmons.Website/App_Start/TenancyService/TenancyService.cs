using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;

namespace Stemmons.TenancyUtilities
{
    public abstract class TenancyService: ITenancyService
    {
        #region Public Methods

        public bool IsTenantSelected
        {
            get
            {
                return CurrentTenantID != Guid.Empty;
            }
        }
          
        /// <summary>
        /// Get or Set the Current Tenant ID
        /// If the tenant ID not set the return value will be empty guid
        /// </summary>
        public abstract Guid CurrentTenantID { get;  }

        /// <summary>
        /// Get or Set the Current User ID
        /// If the user ID not set the return value will be empty guid
        /// </summary>
        public abstract Guid CurrentUserID { get; }

        public abstract TenancyServiceConfiguration Configurations { get; }

        /// <summary>
        /// That will return the tenant DB Connection and validate its connection
        /// </summary>
        /// <param name="refreshCacheValue"></param>
        /// <returns></returns>
        public string GetCurrentTenantDBConnection(TenantDatabaseType databaseType)
        {
            var tenantID = CurrentTenantID;
            bool refreshCacheValue = false;

            var connetionString = GetTenantConnectionStringFromCache(tenantID, databaseType);

            #region Validate the cached connection string

            if (connetionString != null)
            {
                using (SqlConnection dbConection = new SqlConnection(connetionString))
                {
                    //make sure the connection string is valid
                    try
                    {
                        dbConection.Open();
                        dbConection.Close();
                    }
                    catch 
                    {
                        refreshCacheValue = true;
                    }
                }
            }

            #endregion

            if (connetionString == null || refreshCacheValue)
            {
                HttpResponseMessage response;
                using (HttpClient client = GetAuthenticatedClient())
                {
                    #region Perform the API Call
                    try
                    {
                       response = client.GetAsync($"/api/tenants/{tenantID}/connections/{databaseType}").GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        throw new StemmonsAPICommunicationException(ex);
                    }
                    #endregion
                }

                string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    throw new StemmonsAPICommunicationException((int)response.StatusCode, content);
                }

                connetionString = content;
                SetTenantConnectionStringCache(tenantID, databaseType, connetionString);
            }

            if (string.IsNullOrEmpty(connetionString))
                throw new Exception($"Database Connection of tenant '{tenantID}' of type '{databaseType}' can't be null or empty value");

            return connetionString;
        }

        public  List<TenantInfo> GetUserTenants()
        {
            HttpResponseMessage response;
            using (HttpClient client = GetAuthenticatedClient())
            {
                #region Perform the API Call
                try
                {
                    response = client.GetAsync($"/api/users/{CurrentUserID}/tenants").GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    throw new StemmonsAPICommunicationException(ex);
                }
                #endregion
            }

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                throw new StemmonsAPICommunicationException((int)response.StatusCode, content);
            }

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            return javaScriptSerializer.Deserialize<List<TenantInfo>>(content);
        }

        #endregion

        #region Helper Private Region

        private TokenInfo ConfigurationAPIAccessToken { get; set; }

        private HttpClient GetAuthenticatedClient()
        {
            string configurationServiceUrl = Configurations.ServiceUrl;

            #region Validate Input Parameters            
            if (string.IsNullOrEmpty(configurationServiceUrl))
                throw new ArgumentException("authority cannot be empty or null");
            #endregion  

            if (ConfigurationAPIAccessToken == null || ConfigurationAPIAccessToken.ShouldRenew)
            {
                ConfigurationAPIAccessToken = GetConfigurationAPIToken();
            }


            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(configurationServiceUrl.TrimEnd('/'));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ConfigurationAPIAccessToken.access_token);

            return httpClient;
        }

        private TokenInfo GetConfigurationAPIToken()
        {
            string clientId = Configurations.ClientID;
            string clientSecret = Configurations.ClientSecret;
            string authority = Configurations.Authority;
            string scope = Configurations.Scope;

            #region Validate Input Parameters            
            if (string.IsNullOrEmpty(authority))
                throw new ArgumentException("authority cannot be empty or null");

            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("clientId cannot be empty or null");

            if (string.IsNullOrEmpty(clientSecret))
                throw new ArgumentException("clientSecret cannot be empty or null");

            if (string.IsNullOrEmpty(scope))
                throw new ArgumentException("scope cannot be empty or null");
            #endregion

            HttpResponseMessage response;
            using (var formContent = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                        new KeyValuePair<string, string>("client_id", clientId),
                        new KeyValuePair<string, string>("client_secret", clientSecret),
                        new KeyValuePair<string, string>("scope", scope)
                }))
            {

                #region Perform the API Call
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        response = client.PostAsync($"{authority.TrimEnd('/')}/connect/token", formContent).GetAwaiter().GetResult();
                    }
                }
                catch (Exception ex)
                {
                    throw new StemmonsAPICommunicationException(ex);
                }
                #endregion
            }


            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                throw new StemmonsAPICommunicationException((int)response.StatusCode, content);
            }



            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            var token = javaScriptSerializer.Deserialize<TokenInfo>(content);

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtSecurityTokenHandler.ReadJwtToken(token.access_token);
            token.CreatedAt = jwtToken.ValidFrom;
            token.ExpireAt = jwtToken.ValidTo;

            return token;
        }

        #endregion

        #region Connection String Cache

        private MemoryCache _dbconnectionsCache = new MemoryCache("StemmonsDBConnectionsCache");

        private string GetTenantConnectionStringFromCache(Guid tenantID,TenantDatabaseType databaseType)
        {
            CacheItem cacheItem = _dbconnectionsCache.GetCacheItem(tenantID.ToString() + databaseType.ToString());
            if (cacheItem == null)
                return null;
            else
                return cacheItem.Value as string;
        }

        private void SetTenantConnectionStringCache(Guid tenantID, TenantDatabaseType databaseType, string dbConnection)
        {
            _dbconnectionsCache.Set(tenantID.ToString() + databaseType.ToString(), dbConnection, DateTime.Now.AddHours(1));
        }

        #endregion
    }
}
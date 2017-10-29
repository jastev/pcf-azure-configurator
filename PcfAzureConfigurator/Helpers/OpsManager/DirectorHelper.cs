using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.OpsManager
{
    public class DirectorHelper : IDirectorHelper
    {
        private IHttpClient _httpClient;

        public DirectorHelper(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DirectorProperties> GetProperties(string opsManagerFqdn, string token)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://" + opsManagerFqdn + "/api/v0/staged/director/properties");
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var directorProperties = JsonConvert.DeserializeObject<DirectorProperties>(responseContent);

                return directorProperties;
            }
            else
            {
                throw new HttpRequestException(response.Content.ToString());
            }
        }
    }

    public class DirectorProperties
    {
        public IaasConfiguration iaas_configuration { get; set; }
        public DirectorConfiguration director_configuration { get; set; }
        public SecurityConfiguration security_configuration { get; set; }
        public SyslogConfiguration syslog_configuration { get; set; }
    }

    public class IaasConfiguration
    {
        public string subscription_id { get; set; }
        public string tenant_id { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string resource_group_name { get; set; }
        public string cloud_storage_type { get; set; }
        public string bosh_storage_account_name { get; set; }
        public string storage_account_type { get; set; }
        public string deployments_storage_account_name { get; set; }
        public string default_security_group { get; set; }
        public string ssh_public_key { get; set; }
        public string environment { get; set; }
    }

    public class DirectorConfiguration
    {
        public string ntp_servers_string { get; set; }
        public string metrics_ip { get; set; }
        public bool resurrector_enabled { get; set; }
        public int? max_threads { get; set; }
        public string database_type { get; set; }
        public string blobstore_type { get; set; }
    }

    public class SecurityConfiguration
    {
        public string trusted_certificates { get; set; }
        public bool generate_vm_passwords { get; set; }
    }

    public class SyslogConfiguration
    {
        public bool enabled { get; set; }
    }
}

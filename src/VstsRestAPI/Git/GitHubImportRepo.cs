using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using VstsRestAPI.Viewmodel.GitHub;

namespace VstsRestAPI.Git
{
    public class GitHubImportRepo : ApiServiceBase
    {
        private ILog logger = LogManager.GetLogger("ErrorLog");
        public GitHubImportRepo(IConfiguration configuration) : base(configuration)
        {
        }

        public HttpResponseMessage GetUserDetail()
        {
            //https://api.github.com/user
            using (var client = GitHubHttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "demogenapi");
                HttpResponseMessage response = client.GetAsync("/user").Result;
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }
            return new HttpResponseMessage();
        }

        public HttpResponseMessage ForkRepo(string repoName)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            try
            {
                using (var client = GitHubHttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("User-Agent", _configuration.userName);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    var jsonContent = new StringContent("", Encoding.UTF8, "application/json");
                    var method = new HttpMethod("POST");
                    //repos/octocat/Hello-World/forks
                    var request = new HttpRequestMessage(method, $"repos/{repoName}/forks") { Content = jsonContent };
                    res = client.SendAsync(request).Result;
                }
            }
            catch (Exception ex)
            {
                logger.Info(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + ex.Message + "\n" + ex.StackTrace + "\n");
            }
            return res;
        }
        public HttpResponseMessage ListForks(string repoName)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            try
            {
                using (var client = GitHubHttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("User-Agent", _configuration.userName);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var method = new HttpMethod("GET");
                    /// repos /:owner /:repo / forks
                    var request = $"repos/{repoName}/forks";
                    res = client.GetAsync(request).Result;
                }
            }
            catch (Exception ex)
            {
                logger.Info(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + ex.Message + "\n" + ex.StackTrace + "\n");
            }
            return res;
        }

        public HttpResponseMessage CreateRepo(string createRepoJson)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            try
            {
                using (var client = GitHubHttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                    client.DefaultRequestHeaders.Add("User-Agent", _configuration.userName);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var newContent = new StringContent(createRepoJson, Encoding.UTF8, "application/vnd.github.v3+json");
                    /// repos /:owner /:repo / forks
                    var method = new HttpMethod("POST");
                    var request = new HttpRequestMessage(method, "user/repos") { Content = newContent };
                    var response = client.SendAsync(request).Result;
                }
            }
            catch(Exception ex)
            {
                logger.Info(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + ex.Message + "\n" + ex.StackTrace + "\n");
            }
            return res;
        }

        public HttpResponseMessage ImportRepo(string repoName, object importRepoObj)
        {
            try
            {
                string importRepoJson = JsonConvert.SerializeObject(importRepoObj);
                if (!string.IsNullOrEmpty(repoName) && !string.IsNullOrEmpty(importRepoJson))
                {
                    using (var client = GitHubHttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                        client.DefaultRequestHeaders.Add("User-Agent", _configuration.userName);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        var newContent = new StringContent(importRepoJson, Encoding.UTF8, "application/vnd.github.v3+json");
                        var method = new HttpMethod("PUT");
                        var request = new HttpRequestMessage(method, $"repos/{_configuration.userName}/{repoName }/import") { Content = newContent };
                        var response = client.SendAsync(request).Result;
                        return response;
                    }
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                logger.Info(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + ex.Message + "\n" + ex.StackTrace + "\n");
            }
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }

        public HttpResponseMessage GetImportStatus(string repoName)
        {
            try
            {
                ///repos/:owner/:repo/import
                if (!string.IsNullOrEmpty(repoName))
                {
                    using (var client = GitHubHttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("User-Agent", _configuration.userName);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        var request = $"{_configuration._gitbaseAddress}/repos/{_configuration.userName}/{repoName }/import";
                        var response = client.GetAsync(request).Result;
                        return response;
                    }
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                logger.Info(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + ex.Message + "\n" + ex.StackTrace + "\n");
            }
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);

        }

        public HttpResponseMessage GetRepositoryPublicKey(string repoName)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            try
            {
                if (!string.IsNullOrEmpty(repoName))
                {
                    using (var client = GitHubHttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("User-Agent", _configuration.userName);
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        var request = $"{_configuration._gitbaseAddress}/repos/{_configuration.userName}/{repoName}/actions/secrets/public-key";
                        res = client.GetAsync(request).Result;
                        return res;
                    }
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
            }
            catch(Exception ex)
            {
                logger.Info(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + ex.Message + "\n" + ex.StackTrace + "\n");
            }
            return res;

        }

        public HttpResponseMessage EncryptAndAddSecret(GitHubPublicKey publicKey, GitHubSecrets.GitHubSecret secret, string repoName)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            try
            {
                var _secretValue = Encoding.UTF8.GetBytes(secret.secrets.secretValue);
                var _publicKey = Convert.FromBase64String(publicKey.key);
                //var sealedPublicKeyBox = Sodium.SealedPublicKeyBox.Create(_secretValue, _publicKey);
                //var encryptedSecret = Convert.ToBase64String(sealedPublicKeyBox);
                using (var client = GitHubHttpClient())
                {
                    var httpMethod = new HttpMethod("PUT");

                    dynamic obj = new JObject();
                    //obj.encrypted_value = encryptedSecret;
                    //obj.key_id = publicKey.key_id;
                    var json = obj.ToString();

                    var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var request = new  HttpRequestMessage(httpMethod, $"{_configuration._gitbaseAddress}/repos/{_configuration.userName}/{repoName}/actions/secrets/{secret.secrets.secretName}");
                    res = client.SendAsync(request).Result;
                }
            }
            catch (Exception ex)
            {
                logger.Info(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + ex.Message + "\n" + ex.StackTrace + "\n");
            }
            return res;
        }
    }
}

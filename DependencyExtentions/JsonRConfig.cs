using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Text;

namespace Pretech.Software.RConfig.Crypto.DependencyExtentions
{
    public static class JsonRConfig
    {

        public static async Task AddRConfigJson(this IConfigurationBuilder configBuilder, string rConfigApiBase, string rConfigSecretKey, string rConfigKeyword)
        {

            RConfigure config = new RConfigure(rconfigBaseUri: rConfigApiBase, secretKey: rConfigSecretKey, decryptKey: rConfigKeyword);

            var decryptedJson = await config.LoadJsonAsync();
            if (decryptedJson.isSuccess)
            {
                var jsonStream = new JsonStreamConfigurationSource();
                jsonStream.Stream = new MemoryStream(Encoding.UTF8.GetBytes(decryptedJson.data.value));
                configBuilder.Add(jsonStream);
            }


        }
        /// <summary>
        /// Within the scope of this overwrite, values are automatically drawn from the environment.(RconfigBaseUrı,Secret,Keyword...) 
        /// </summary>
        /// <param name="configBuilder"></param>
        /// <returns></returns>
        public static async Task AddRConfigJson(this IConfigurationBuilder configBuilder)
        {
            string uri = Environment.GetEnvironmentVariable("RConfigUri");
            string secret = Environment.GetEnvironmentVariable("RConfigSecret") ?? null;
            string keyword = Environment.GetEnvironmentVariable("RConfigKeyword") ?? null;

            RConfigure config = new RConfigure(rconfigBaseUri: uri, secretKey: secret, decryptKey: keyword);

            var decryptedJson = await config.LoadJsonAsync();
            if (decryptedJson.isSuccess)
            {
                var jsonStream = new JsonStreamConfigurationSource();
                jsonStream.Stream = new MemoryStream(Encoding.UTF8.GetBytes(decryptedJson.data.value));
                configBuilder.Add(jsonStream);
            }


        }


    }
}

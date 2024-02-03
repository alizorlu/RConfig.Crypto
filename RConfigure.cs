using JsonFormatterPlus;
using Newtonsoft.Json.Linq;
using Pretech.Software.RConfig.Crypto.Abstract;
using Pretech.Software.RConfig.Crypto.Concrete;
using Pretech.Software.RConfig.Crypto.Response;
using System.Net.Http.Json;

namespace Pretech.Software.RConfig.Crypto
{
    public class RConfigure
    {
        private readonly HttpClient _client;
        private IJsonCrypto _crypto;
        private readonly string _secretKey;
        private readonly string _decryptKey;
        public RConfigure(string rconfigBaseUri, string secretKey, string decryptKey)
        {
            _client = new HttpClient();
            _crypto = new JsonCrypto();
            _client.BaseAddress = new Uri(rconfigBaseUri);
            _secretKey = secretKey;
            _decryptKey = decryptKey;
        }
        //public async Task<T> LoadAsync<T>()
        //{
        //    try
        //    {
        //        var sectionName = typeof(T).Name;

        //        using (var client = _client)
        //        {
        //            var getJson = await _client.GetFromJsonAsync<RestResponse>($"{_client.BaseAddress}api/Config/RawJson/{_secretKey}");
        //            if (getJson.isSuccess)
        //            {
        //                JObject jsonObject = JObject.Parse(getJson.data.value);
        //                var configureTypeValue = (string)jsonObject[sectionName];

        //                var configureDecryptValue = await _crypto.DecryptAsync(configureTypeValue, _decryptKey);
        //                if (configureDecryptValue.IsSuccess)
        //                {

        //                    return JsonConvert.DeserializeObject<T>(configureDecryptValue?.Data);
        //                }
        //                else return default(T);
        //            }
        //            else return default(T);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return default(T);
        //    }
        //}
        public async Task<RestResponse> LoadJsonAsync()
        {
            RestResponse result = new();
            try
            {

                using (var client = _client)
                {
                    var getJson = await _client.GetFromJsonAsync<RestResponse>($"{_client.BaseAddress}api/Config/RawJson/{_secretKey}");
                    if (getJson.isSuccess)
                    {
                        JObject jsonObject = JObject.Parse(getJson.data.value);
                        HashSet<string> hashList = new();
                        foreach (JProperty property in jsonObject.Properties())
                        {
                            // item.Value<string>() kullanılarak JSON öğesinin değeri alınabilir.
                            var value = property.Value.ToString();

                            var decryptedValue = await _crypto.DecryptAsync(value, _decryptKey);
                            if (decryptedValue.IsSuccess)
                            {
                                hashList.Add($"\"{property.Name}\":{decryptedValue.Data}");
                            }
                            else
                            {
                                result.isSuccess = false;
                                result.message = decryptedValue.Message;
                            }
                        }
                        var json = string.Join(",", hashList);
                        json = JsonFormatter.Format("{" + "\n" + json + "\n" + "}");
                        result.isSuccess = true;
                        result.message = "";
                        result.data = new Data();
                        result.data.value = json;
                    }
                    else
                    {
                        result.isSuccess = false;
                        result.message = getJson.message;
                    }
                }
            }
            catch (Exception ex)
            {
                result.isSuccess = false;
                result.message = ex.Message;
            }
            return result;
        }

    }
}

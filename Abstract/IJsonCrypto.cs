namespace Pretech.Software.RConfig.Crypto.Abstract
{
    public interface IJsonCrypto
    {
        Task<ExecuteResponse<string>> EncryptAsync(string value, string key);
        Task<ExecuteResponse<string>> DecryptAsync(string value,string key);
        Task<ExecuteResponse<string>> GenerateKeyAsync();
        Task<string> ToBase64(string value);
        Task<string> FromBase64(string base64);
    }
}

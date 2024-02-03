namespace Pretech.Software.RConfig.Crypto.Response
{
    public class RestResponse
    {
        public Data data { get; set; }
        public bool isSuccess { get; set; } 
        public string message { get; set; }
    }
    public class Data
    {
        public string value { get; set; }
    }
}

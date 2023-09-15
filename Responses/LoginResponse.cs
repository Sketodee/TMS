namespace TMS.Responses
{
    public class LoginResponse
    {
        public string id { get; set; } = string.Empty;
        public string Name { get; set; }
        public IList<string> roles { get; set; }
        public string email { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
    }
}

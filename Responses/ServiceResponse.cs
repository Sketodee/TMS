namespace TMS.Responses
{
    public class ServiceResponse
    {
        public dynamic Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public IEnumerable<string> Errors { get; set; }
    }
}

namespace OutlookAPI.Models
{
    public class APIResponse
    {
        public bool isError { get; set; }
        public string ErrorMessage { get; set; }
        public object Data { get; set; }
    }
}

namespace TarWebApi.Models.Contracts
{
	public class GenericServiceResponse
	{
        public bool IsSuccessful { get; set; }
        public string? ErrorText { get; set; }
    }
}

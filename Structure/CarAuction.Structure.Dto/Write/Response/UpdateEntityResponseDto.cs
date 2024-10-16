namespace CarAuction.Structure.Dto.Write
{
    public class UpdateEntityResponseDto(bool success, string message)
    {
        public bool Success { get; set; } = success;

        public string Message { get; set; } = message;
    }
}
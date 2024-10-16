namespace CarAuction.Structure.Dto.Write
{
    public class CreateEntityResponseDto(bool success, string message)
    {
        public bool Success { get; set; } = success;

        public string Message { get; set; } = message;
    }
}
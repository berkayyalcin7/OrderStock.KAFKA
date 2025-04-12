namespace Order.API.DTOs
{
    public record OrderCreateRequestDto(string OrderCode, string UserId, decimal TotalPrice);
}

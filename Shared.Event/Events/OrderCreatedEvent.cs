namespace Shared.Event.Events
{
    // direkt olarak property'leri tanımlıyoruz.
    // herhangi bir değişiklik olmaz.
    public record OrderCreatedEvent(string OrderCode, string UserId, decimal totalPrice);

}

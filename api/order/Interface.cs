public interface IOrderService
{

    Task<object> OrderRide(CreateOrderDto dto, string idUser);
    Task<object> GetRider(string dto);
}
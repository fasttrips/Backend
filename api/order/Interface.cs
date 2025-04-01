public interface IOrderService
{

    Task<object> OrderRide(CreateOrderDto dto, string idUser);
    Task<object> GetRider(string dto);
    Task<object> GetOrder(string idUser);
    Task<object> GetOrderDetail(string idUser);


}
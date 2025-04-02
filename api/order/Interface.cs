public interface IOrderService
{

    Task<object> OrderRide(CreateOrderDto dto, string idUser);
    Task<object> GetRider(string dto);
    Task<object> GetOrder(string idUser);
    Task<object> CancelOrderByUser(string idUser);
    Task<object> GetOrderDetail(string idUser);


    Task<object> TerimaOrder(string idUser, string idOrder);
    Task<object> LanjutkanOrder(string idUser, string idOrder);
    Task<object> SelesaiOrder(string idUser, string idOrder);
    Task<object> CancelOrder(string idUser, string idOrder);
    Task<object> DriverOrder(string idUser);


}
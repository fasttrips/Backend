public interface ITrasFoodService
{
    Task<Object> GetWarung();
    Task<Object> GetWarungbyId(string id);
    Task<Object> CreateWarung(CreateWarungDto dto);
    Task<Object> CreateFood(CreateFoodDto dto);

    Task<Object> GetWarungDistance( double lat, double lng);
    

    

}
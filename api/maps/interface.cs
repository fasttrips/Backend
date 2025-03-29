public interface IMapsService
{
    Task<Object> Get();
    Task<Object> GetDirections(CreateDirectionsDto createDirectionsDto);
    Task<Object> GetSearchLocation(CreateDirectionsDto createDirectionsDto);
    Task<Object> GetPlaceLocation(CreateDirectionsDto createDirectionsDto);
    Task<Object> GetAddressFromLatLon(CreateDirectionsDto createDirectionsDto);




}
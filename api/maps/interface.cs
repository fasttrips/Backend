public interface IMapsService
{
    Task<Object> Get();
    Task<Object> GetDirections(CreateDirectionsDto createDirectionsDto);

}
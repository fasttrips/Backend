
public interface IAuthService
{
    // Task<Object> RegisterGoogleAsync(string model,RegisterGoogleDto login);
    Task<Object> Aktifasi(string id);
    Task<Object> UpdateProfile(string id, UpdateProfileDto item);


}
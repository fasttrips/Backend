
public interface IAuthService
{
    // Task<Object> RegisterGoogleAsync(string model,RegisterGoogleDto login);
    Task<Object> Aktifasi(string id);
    Task<Object> UpdateProfile(string id, UpdateProfileDto item);
    Task<Object> UpdateUserProfile(string id, UpdateFCMProfileDto item);
    Task<Object> UpdateDriverProfile(string id, DriverAvalibleModelDTO item);
    Task<Object> UpdateDriverLocationProfile(string id, DriverAvalibleModelDTO item);
    Task<Object> UpdateDriverStatusProfile(string id, DriverStatusServe item);
    Task<Object> UpdateServiceDriver(string id, DriverServiceServe item);

    Task<Object> GetDriverStatusProfile(string id);


    Task<Object> SendNotif(PayloadNotifSend payloadNotifSend);
}
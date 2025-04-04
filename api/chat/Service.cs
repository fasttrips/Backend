using MongoDB.Driver;
using Trasgo.Shared.Models;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RepositoryPattern.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IMongoCollection<ChatModel> _ChatCollection;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<DriverAvalibleModel> _driverCollection;

        private readonly IMongoCollection<Setting> _settingCollection;


        public ChatService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            var database = client.GetDatabase("trasgo");
            _ChatCollection = database.GetCollection<ChatModel>("Chat");
            _userCollection = database.GetCollection<User>("User");
            _settingCollection = database.GetCollection<Setting>("Setting");
            _driverCollection = database.GetCollection<DriverAvalibleModel>("DriverListAvailable");
            _userCollection = database.GetCollection<User>("User");
        }

        public async Task<object> SendChatWAAsync(string idUser, CreateChatDto dto)
        {
            try
            {
                if (idUser == dto.IdUser)
                {
                    var items = new ChatModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        IdOrder = dto.IdOrder,
                        IdUser = dto.IdUser,
                        IdDriver = dto.IdDriver,
                        Sender = "User",
                        CreatedAt = DateTime.UtcNow,
                        Message = dto.Message,
                        Image = "",
                    };
                    await _ChatCollection.InsertOneAsync(items);

                    var Driver = await _driverCollection.Find(otp => otp.Id == dto.IdDriver).FirstOrDefaultAsync();
                    var DriverDetail = await _userCollection.Find(otp => otp.Phone == dto.IdDriver).FirstOrDefaultAsync();

                    var notifikasiUser = new PayloadNotifSend
                    {
                        FCM = Driver.FCM,
                        Title = "Customer " + DriverDetail.FullName,
                        Body = dto.Message
                    };
                    SendNotif(notifikasiUser);

                    return new { code = 200, data = "Berhasil" };
                }
                else
                {
                    var items = new ChatModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        IdOrder = dto.IdOrder,
                        IdUser = dto.IdUser,
                        IdDriver = dto.IdDriver,
                        Sender = "Mitra",
                        CreatedAt = DateTime.UtcNow,
                        Message = dto.Message
                    };
                    await _ChatCollection.InsertOneAsync(items);

                    var User = await _userCollection.Find(otp => otp.Phone == dto.IdUser).FirstOrDefaultAsync();
                    var notifikasiUser = new PayloadNotifSend
                    {
                        FCM = User.Fcm,
                        Title = "Customer " + User.FullName,
                        Body = dto.Message
                    };
                    SendNotif(notifikasiUser);
                    return new { code = 200, data = "Berhasil" };
                }
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public async Task<object> GetChatWAAsync(GetChatDto dto)
        {
            try
            {
                var items = await _ChatCollection
                    .Find(x => x.IdOrder == dto.IdOrder && x.IdDriver == dto.IdDriver && x.IdUser == dto.IdUser)
                    .SortBy(x => x.CreatedAt) // Mengurutkan dari yang paling lama ke terbaru
                    .ToListAsync();
                if (items == null)
                {
                    return new { code = 400, data = "Data not found" };
                }
                return new { code = 200, data = items };
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public async Task<object> SendNotif(PayloadNotifSend item)
        {
            try
            {
                string response = await FirebaseService.SendPushNotification(item.FCM, item.Title, item.Body, item.IdOrder);
                return new { code = 200, Message = "Notification sent successfully", Response = response };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

    }
}

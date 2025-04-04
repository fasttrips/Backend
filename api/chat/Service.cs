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
        private readonly IMongoCollection<Setting> _settingCollection;


        public ChatService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            var database = client.GetDatabase("trasgo");
            _ChatCollection = database.GetCollection<ChatModel>("Chat");
            _userCollection = database.GetCollection<User>("User");
            _settingCollection = database.GetCollection<Setting>("Setting");
        }

        public async Task<object> SendChatWAAsync(CreateChatDto dto)
        {
            try
            {
                var items = new ChatModel
                {
                    Id = Guid.NewGuid().ToString(),
                    IdOrder = dto.IdOrder,
                    IdUser = dto.IdUser,
                    IdDriver = dto.IdDriver,
                    Sender = dto.Sender,
                    CreatedAt = DateTime.UtcNow,
                    Message = dto.Message
                };
                await _ChatCollection.InsertOneAsync(items);
                return new { code = 200, data = "Berhasil" };
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

    }
}

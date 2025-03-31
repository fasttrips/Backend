using MongoDB.Driver;
using Trasgo.Shared.Models;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RepositoryPattern.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<OrderModel> _OrderCollection;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Setting> _settingCollection;


        public OrderService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            var database = client.GetDatabase("trasgo");
            _OrderCollection = database.GetCollection<OrderModel>("Order");
            _userCollection = database.GetCollection<User>("User");
            _settingCollection = database.GetCollection<Setting>("Setting");
        }

        public async Task<object> OrderRide(CreateOrderDto dto)
        {
            return new
                    {
                        code = 404,
                        data = (string)null,
                        message = "API Key tidak ditemukan"
                    };
        }

        public class sendForm
        {
            public string? Id { get; set; }
            public string? Phone { get; set; }
            public string? Subject { get; set; }
            public string? Message { get; set; }
            public string? Order { get; set; }
        }


    }
}

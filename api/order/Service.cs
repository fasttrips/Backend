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
        private readonly IMongoCollection<DriverAvalibleModel> _driverAvailableCollection;
        private readonly IMongoCollection<DriverListCancelModel> _driverCancelCollection;




        public OrderService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            var database = client.GetDatabase("trasgo");
            _OrderCollection = database.GetCollection<OrderModel>("Order");
            _driverAvailableCollection = database.GetCollection<DriverAvalibleModel>("DriverListAvailable");
            _driverCancelCollection = database.GetCollection<DriverListCancelModel>("DriverListCancel");

            _userCollection = database.GetCollection<User>("User");
            _settingCollection = database.GetCollection<Setting>("Setting");
        }

        public async Task<object> GetRider(GetOrderDto dto)
        {
            var orderData = await _OrderCollection.Find(otp => otp.Status == 0 && otp.IsActive == true && otp.Id == dto.idOrder).FirstOrDefaultAsync();

            var getCancelNearbyDriver = await _driverCancelCollection.Find(otp => otp.IdOrder == dto.idOrder).ToListAsync();

            var getNearbyDriver = await _driverAvailableCollection.Find(otp => otp.IsStandby == true).ToListAsync();

            var filteredNearbyDriver = getNearbyDriver
            .Where(driver => !getCancelNearbyDriver.Any(cancelled => cancelled.IdDriver == driver.Id))
            .ToList();

            var driverWithDistance = filteredNearbyDriver
            .Select(driver => new
            {
                Driver = driver.Id,
                Distance = GeoHelper.CalculateDistance(
                    orderData.PickupLocation.Latitude,
                    orderData.PickupLocation.Longitude,
                    driver.Latitude ?? 0,
                    driver.Longitude ?? 0
                )
            })
            .OrderBy(d => d.Distance) // Urutkan berdasarkan jarak terdekat
            .ToList();
            var nearbyDrivers = driverWithDistance.Where(d => d.Distance <= 2).ToList();
            string lastDriverId = orderData.LastDriver == "" ? nearbyDrivers[0].Driver : orderData.LastDriver;

            int currentIndex = nearbyDrivers.FindIndex(d => d.Driver == lastDriverId);
            if (currentIndex == -1)
                throw new Exception($"Driver {lastDriverId} tidak ditemukan.");

            if (currentIndex + 1 >= nearbyDrivers.Count)
                throw new Exception("Driver tidak ada.");


            string nextDriverId = nearbyDrivers[orderData.LastDriver == "" ? 0 : currentIndex + 1].Driver;

            orderData.LastDriver = nextDriverId;
            await _OrderCollection.ReplaceOneAsync(x => x.Id == dto.idOrder, orderData);

            // if(orderData.LastDriver == "")
            // {
            //     var driver = nearbyDrivers.FirstOrDefault();
            //     orderData.LastDriver = driver.Driver;
            //     await _OrderCollection.ReplaceOneAsync(x => x.Id == dto.idOrder, orderData);

            //     ////ubah status mjika druver sdng d calling
            //     var driverOn = await _driverAvailableCollection.Find(otp => otp.Id == driver.Driver).FirstOrDefaultAsync();
            //     driverOn.OnCall = true;
            //     await _driverAvailableCollection.ReplaceOneAsync(x => x.Id == driver.Driver, driverOn);
            // }else{
            //     var driver = nearbyDrivers.FirstOrDefault();
            //     orderData.LastDriver = driver.Driver;
            //     await _OrderCollection.ReplaceOneAsync(x => x.Id == dto.idOrder, orderData);
            //     // var roleData = new DriverListCancelModel()
            //     // {
            //     //     Id = Guid.NewGuid().ToString(),
            //     //     IdDriver = orderData.LastDriver,
            //     //     IdOrder = orderData.Id,
            //     //     CreatedAt = DateTime.Now
            //     // };
            //     // await _driverCancelCollection.InsertOneAsync(roleData);
            // }
            return new { code = 200, message = "Berhasil", data = orderData, driverList = nearbyDrivers };
        }

        public async Task<object> OrderRide(CreateOrderDto dto, string idUser)
        {
            var uuid = Guid.NewGuid().ToString();
            var userModel = new OrderModel
            {
                Id = uuid,
                IdUser = idUser,
                IdDriver = "",
                IdMitra = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                IsVerification = false,
                PickupLocation = new PickupLocation
                {
                    Latitude = dto.PickupLocation.Latitude,
                    Longitude = dto.PickupLocation.Longitude,
                    Address = dto.PickupLocation.Address
                },
                DestinationLocation = new DestinationLocation
                {
                    Latitude = dto.DestinationLocation.Latitude,
                    Longitude = dto.DestinationLocation.Longitude,
                    Address = dto.DestinationLocation.Address
                },
                Status = dto.Status,
                Service = dto.Service,
                IsDeclinebyUser = null,
                NotesDecline = "",
                Type = dto.Type,
                HargaLayanan = dto.HargaLayanan,
                HargaPotonganDriver = dto.HargaPotonganDriver,
                HargaPotonganMitra = dto.HargaPotonganMitra,
                HargaKenaikan = dto.HargaKenaikan,
                Diskon = dto.Diskon,
                Jarak = dto.Jarak,
                Payment = dto.Payment,
                LastDriver = ""
            };
            await _OrderCollection.InsertOneAsync(userModel);
            return new { code = 200, message = "Berhasil", idOrder = uuid };
        }

        public class sendForm
        {
            public string? Id { get; set; }
            public string? Phone { get; set; }
            public string? Subject { get; set; }
            public string? Message { get; set; }
            public string? Order { get; set; }
        }

        public class GeoHelper
        {
            private const double EarthRadiusKm = 6371; // Radius Bumi dalam km

            public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
            {
                double dLat = ToRadians(lat2 - lat1);
                double dLon = ToRadians(lon2 - lon1);

                double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                           Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                           Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                return EarthRadiusKm * c; // Hasil dalam kilometer
            }

            private static double ToRadians(double angle)
            {
                return Math.PI * angle / 180.0;
            }
        }


    }
}

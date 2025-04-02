using MongoDB.Driver;
using Trasgo.Shared.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace RepositoryPattern.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<OrderModel> _OrderCollection;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Setting> _settingCollection;
        private readonly IMongoCollection<DriverAvalibleModel> _driverAvailableCollection;
        private readonly IMongoCollection<DriverListCancelModel> _driverCancelCollection;
        private readonly IMongoCollection<User> _driverData;

        private readonly IAuthService _iAuthService;


        public OrderService(IConfiguration configuration, IAuthService iAuthService)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            var database = client.GetDatabase("trasgo");
            _OrderCollection = database.GetCollection<OrderModel>("Order");
            _driverAvailableCollection = database.GetCollection<DriverAvalibleModel>("DriverListAvailable");
            _driverData = database.GetCollection<User>("User");

            _driverCancelCollection = database.GetCollection<DriverListCancelModel>("DriverListCancel");

            _userCollection = database.GetCollection<User>("User");
            _settingCollection = database.GetCollection<Setting>("Setting");

            _iAuthService = iAuthService;
        }

        public async Task<object> GetOrder(string idUser)
        {
            var orderData = await _OrderCollection.Find(otp => otp.IsActive == true && otp.IdUser == idUser).ToListAsync();



            return new { code = 200, message = "Berhasil", data = orderData };
        }

        public async Task<object> GetOrderDetail(string idUser)
        {
            var orderData = await _OrderCollection.Find(otp => otp.Id == idUser).FirstOrDefaultAsync();
            if (orderData.IdDriver == "")
            {
                return new { code = 200, message = "Berhasil", data = orderData, driver = (object)null, locationDriver = (object)null };
            }
            var driverData = await _driverData.Find(otp => otp.Phone == orderData.IdDriver).FirstOrDefaultAsync();
            var locationData = await _driverAvailableCollection.Find(otp => otp.Id == orderData.IdDriver).FirstOrDefaultAsync();

            return new { code = 200, message = "Berhasil", data = orderData, driver = driverData, locationDriver = locationData };
        }

        public async Task<object> CancelOrderByUser(string idOrder)
        {
            var orderData = await _OrderCollection.Find(otp => otp.Id == idOrder).FirstOrDefaultAsync();
            orderData.Status = 4;
            orderData.IsDeclinebyUser = true;
            await _OrderCollection.ReplaceOneAsync(x => x.Id == idOrder, orderData);

            if (orderData.IdDriver != "")
            {
                var Driver = await _driverAvailableCollection.Find(otp => otp.Id == orderData.IdDriver).FirstOrDefaultAsync();
                var notifikasiDriver = new PayloadNotifSend
                {
                    FCM = Driver.FCM,
                    Title = "Pesanan Di batalkan user",
                    Body = $"Tenang kami akan carikan yang baru buat kamu"
                };
                SendNotif(notifikasiDriver);
            }

            var User = await _userCollection.Find(otp => otp.Phone == orderData.IdUser).FirstOrDefaultAsync();
            var notifikasiUser = new PayloadNotifSend
            {
                FCM = User.Fcm,
                Title = "Pesanan Di batalkan",
                Body = $"Trasgo berkomitment selalu meningkatkan layanan kami"
            };
            SendNotif(notifikasiUser);
            return new { code = 200, message = "Order Cancel", data = orderData };
        }

        public async Task<object> GetRider(string idOrder)
        {
            var orderData = await _OrderCollection.Find(otp => otp.Status == 0 && otp.IsActive == true && otp.Id == idOrder).FirstOrDefaultAsync();

            var getCancelNearbyDriver = await _driverCancelCollection.Find(otp => otp.IdOrder == idOrder).ToListAsync();

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
            string lastDriverId = string.IsNullOrEmpty(orderData.LastDriver) ? (nearbyDrivers[0].Driver ?? string.Empty) : orderData.LastDriver;

            int currentIndex = nearbyDrivers.FindIndex(d => d.Driver == lastDriverId);
            if (currentIndex == -1)
            {
                string nextDriverId2 = nearbyDrivers[0].Driver;
                orderData.LastDriver = "";
                await _OrderCollection.ReplaceOneAsync(x => x.Id == idOrder, orderData);

                ////kirim notif
                var toNotif2 = getNearbyDriver.Find(x => x.Id == nextDriverId2).FCM;
                var nDriver1= new PayloadNotifSend
                {
                    FCM = toNotif2,
                    Title = "Ada Pesanan Masuk",
                    Body = $"Terima Pesanan {orderData.Service} Sekarang",
                    IdOrder = orderData.Id
                };
                SendNotif(nDriver1);
                throw new Exception($"Driver tidak adas.");
            }

            if (currentIndex + 1 >= nearbyDrivers.Count)
            {
                string nextDriverId2 = nearbyDrivers[0].Driver;
                orderData.LastDriver = "";
                await _OrderCollection.ReplaceOneAsync(x => x.Id == idOrder, orderData);

                ////kirim notif
                var toNotif2 = getNearbyDriver.Find(x => x.Id == nextDriverId2).FCM;
                var nDriver1= new PayloadNotifSend
                {
                    FCM = toNotif2,
                    Title = "Ada Pesanan Masuk",
                    Body = $"Terima Pesanan {orderData.Service} Sekarang",
                    IdOrder = orderData.Id
                };
                SendNotif(nDriver1);
                throw new Exception("Driver tidak ada.");
            }

            string nextDriverId = nearbyDrivers[orderData.LastDriver == "" ? 0 : currentIndex + 1].Driver;
            orderData.LastDriver = nextDriverId;

            var toNotif = getNearbyDriver.Find(x => x.Id == nextDriverId).FCM;

            var notifikasiDriver = new PayloadNotifSend
            {
                FCM = toNotif,
                Title = "Ada Pesanan Masuk",
                Body = $"Terima Pesanan {orderData.Service} Sekarang",
                IdOrder = orderData.Id
            };
            SendNotif(notifikasiDriver);

            await _OrderCollection.ReplaceOneAsync(x => x.Id == idOrder, orderData);

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
                LastDriver = "",
                Coordinates = dto.Coordinates
            };

            var getUser = await _userCollection.Find(otp => otp.Phone == idUser).FirstOrDefaultAsync();
            var notifikasiUser = new PayloadNotifSend
            {
                FCM = getUser.Fcm,
                Title = "Kami carikan driver terdekat",
                Body = "Silahkan cek di tab aktifitas untuk melihat transaksi"
            };
            SendNotif(notifikasiUser);
            await _OrderCollection.InsertOneAsync(userModel);
            return new { code = 200, message = "Berhasil", idOrder = uuid };
        }

        public async Task<object> TerimaOrder(string idUser, string idOrder)
        {
            var orderData = await _OrderCollection.Find(otp => otp.Id == idOrder).FirstOrDefaultAsync();
            if(orderData.IdDriver != "")
            {
                return new { code = 200, message = "Kamu telat mengambil orderan", data = (object)null };
            }
            var Driver = await _driverAvailableCollection.Find(otp => otp.Id == idUser).FirstOrDefaultAsync();

            orderData.Status = 1;
            orderData.IdDriver = idUser;
            await _OrderCollection.ReplaceOneAsync(x => x.Id == idOrder, orderData);

            Driver.IsStandby = false;
            await _driverAvailableCollection.ReplaceOneAsync(x => x.Id == idUser, Driver);


            var User = await _userCollection.Find(otp => otp.Phone == orderData.IdUser).FirstOrDefaultAsync();
            var notifikasiUser = new PayloadNotifSend
            {
                FCM = User.Fcm,
                Title = "Horay driver di temukan",
                Body = $"Kendaraan {Driver.NamaKendaraan} dengan Plat {Driver.Plat} akan menjemput"
            };
            SendNotif(notifikasiUser);
            return new { code = 200, message = "Order Cancel", data = orderData };
        }

        public async Task<object> LanjutkanOrder(string idUser, string idOrder)
        {
            var orderData = await _OrderCollection.Find(otp => otp.Id == idOrder).FirstOrDefaultAsync();
            var Driver = await _driverAvailableCollection.Find(otp => otp.Id == idUser).FirstOrDefaultAsync();

            orderData.Status = 2;
            orderData.IdDriver = idUser;
            await _OrderCollection.ReplaceOneAsync(x => x.Id == idOrder, orderData);

            var User = await _userCollection.Find(otp => otp.Phone == orderData.IdUser).FirstOrDefaultAsync();
            var notifikasiUser = new PayloadNotifSend
            {
                FCM = User.Fcm,
                Title = "Driver sudah di lokasi",
                Body = $"Kamu naik Kendaraan {Driver.NamaKendaraan} dengan Plat {Driver.Plat}"
            };
            SendNotif(notifikasiUser);
            return new { code = 200, message = "Order Cancel", data = orderData };
        }

        public async Task<object> SelesaiOrder(string idUser, string idOrder)
        {
            var orderData = await _OrderCollection.Find(otp => otp.Id == idOrder).FirstOrDefaultAsync();
            var Driver = await _driverAvailableCollection.Find(otp => otp.Id == idUser).FirstOrDefaultAsync();

            orderData.Status = 3;
            orderData.IdDriver = idUser;
            await _OrderCollection.ReplaceOneAsync(x => x.Id == idOrder, orderData);

            var User = await _userCollection.Find(otp => otp.Phone == orderData.IdUser).FirstOrDefaultAsync();
            var notifikasiUser = new PayloadNotifSend
            {
                FCM = User.Fcm,
                Title = "Terima kasih",
                Body = $"Kamu sudah menggunakan layanan kami"
            };
            SendNotif(notifikasiUser);
            return new { code = 200, message = "Order Cancel", data = orderData };
        }

        public async Task<object> CancelOrder(string idUser, string idOrder)
        {
            var orderData = await _OrderCollection.Find(otp => otp.Id == idOrder).FirstOrDefaultAsync();
            var Driver = await _driverAvailableCollection.Find(otp => otp.Id == idUser).FirstOrDefaultAsync();

            orderData.Status = 0;
            orderData.IdDriver = "";
            await _OrderCollection.ReplaceOneAsync(x => x.Id == idOrder, orderData);

            Driver.IsStandby = true;
            await _driverAvailableCollection.ReplaceOneAsync(x => x.Id == idUser, Driver);

            var User = await _userCollection.Find(otp => otp.Phone == orderData.IdUser).FirstOrDefaultAsync();
            var notifikasiUser = new PayloadNotifSend
            {
                FCM = User.Fcm,
                Title = "Maaf ya driver membatalkan orderan",
                Body = $"kami sedang menghubungi driver terdekat"
            };
            SendNotif(notifikasiUser);
            return new { code = 200, message = "Order", data = orderData };
        }

        public async Task<object> DriverOrder(string idUser)
        {
            var orderData = await _OrderCollection.Find(otp => otp.IdDriver == idUser && otp.Status < 4).FirstOrDefaultAsync();
            if(orderData == null)
            {
                return new { code = 200, message = "Order", data = (object)null };
            }
            var User = await _userCollection.Find(otp => otp.Phone == orderData.IdUser).FirstOrDefaultAsync();
            return new { code = 200, message = "Order", data = orderData, user = User };
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

using MongoDB.Driver;
using Trasgo.Shared.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using SixLabors.ImageSharp;

namespace RepositoryPattern.Services.TrasFoodService
{
    public class TrasFoodService : ITrasFoodService
    {
        private readonly IMongoCollection<Warung> _WarungCollection;
        private readonly IMongoCollection<Food> _FoodCollection;

        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Setting> _settingCollection;


        public TrasFoodService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            var database = client.GetDatabase("trasgo");
            _WarungCollection = database.GetCollection<Warung>("Warung");
            _FoodCollection = database.GetCollection<Food>("Food");
            _userCollection = database.GetCollection<User>("User");
        }
        public async Task<object> GetWarung()
        {
            // Cari TrasFood berdasarkan email
            var TrasFood = await _WarungCollection.Find(o => o.IsActive == true).ToListAsync();
            return new { code = 200, Data = TrasFood };
        }
        public async Task<object> GetWarungbyId(string id)
        {
            // Cari TrasFood berdasarkan email
            var TrasFood = await _WarungCollection.Find(o => o.IdUser == id).FirstOrDefaultAsync();
            if (TrasFood == null)
            {
                return new { code = 400, message = "Warung not found" };
            }
            var Food = await _FoodCollection.Find(o => o.IdUser == id).ToListAsync();
            var result = new
            {
                Id = TrasFood.Id,
                FullName = TrasFood.FullName,
                Address = TrasFood.Address,
                ImageCover = TrasFood.ImageCover,
                Rating = 5, // default value
                Makanan = Food,
            };
            return new { code = 200, Data = result };
        }

        public async Task<object> GetWarungDistance(double lat, double lng)
        {
            // Cari TrasFood berdasarkan email
            var TrasFood = await _WarungCollection.Find(o => o.IsActive == true).ToListAsync();
            var result = TrasFood.Where(o => o.Latitude != null && o.Longitude != null).Select(o => new
            {
                Id = o.Id,
                FullName = o.FullName,
                Address = o.Address,
                ImageCover = o.ImageCover,
                Rating = 5,
                Distance = GeoHelper.CalculateDistance(lat, lng, (double)o.Latitude, (double)o.Longitude)
            }).OrderBy(d => d.Distance).ToList();
            var nearbyWarung = result.Where(d => d.Distance <= 5).ToList();
            return new { code = 200, Data = nearbyWarung };
        }

        public async Task<object> CreateWarung(CreateWarungDto dto)
        {
            // Cari TrasFood berdasarkan email
            var TrasFood = await _WarungCollection.Find(o => o.IdUser == dto.IdUser).FirstOrDefaultAsync();
            if (TrasFood != null)
            {
                return new { code = 400, message = "Warung already exists" };
            }
            TrasFood = new Warung
            {
                Id = Guid.NewGuid().ToString(),
                ImageCover = dto.ImageCover,
                IdUser = dto.IdUser,
                FullName = dto.FullName,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsUMKM = dto.IsUMKM,
                Category = dto.Category,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _WarungCollection.InsertOneAsync(TrasFood);
            return new { code = 200, Data = TrasFood };
        }

        public async Task<object> CreateFood(CreateFoodDto dto)
        {
            // Cari TrasFood berdasarkan email
            var TrasFood = await _WarungCollection.Find(o => o.IdUser == dto.IdUser).FirstOrDefaultAsync();
            if (TrasFood == null)
            {
                return new { code = 400, message = "Warung not exists" };
            }
            var data = new Food
            {
                Id = Guid.NewGuid().ToString(),
                ImageCover = dto.ImageCover,
                IdUser = dto.IdUser,
                FullName = dto.FullName,
                Diskon = dto.Diskon,
                Price = dto.Price,
                Description = dto.Description,
                IsAvailable = dto.IsAvailable,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _FoodCollection.InsertOneAsync(data);
            return new { code = 200, Data = data };
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

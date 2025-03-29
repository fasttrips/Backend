using MongoDB.Driver;
using Trasgo.Shared.Models;

namespace RepositoryPattern.Services.MapsService
{
    public class MapsService : IMapsService
    {
        private readonly string key;
        private readonly IMongoCollection<Setting> _settingCollection;
        private readonly IMongoCollection<HargaDriver> _hargaDriverCollection;


        public MapsService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("trasgo");
            _settingCollection = database.GetCollection<Setting>("Setting");
            _hargaDriverCollection = database.GetCollection<HargaDriver>("HargaDriver");

            this.key = configuration.GetSection("AppSettings")["JwtKey"];
        }
        public async Task<Object> Get()
        {
            try
            {
                string polyline = "a~l~Fjk~uOwHJy@P";
                var points = DecodePolyline(polyline);

                var result = points.Select(p => new LatLngDto
                {
                    Latitude = p.Latitude,
                    Longitude = p.Longitude
                }).ToList();

                return new
                {
                    code = 200,
                    data = result,
                    message = "Success get polyline points"
                };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public int BulatkanKeKelipatan(double angka, int kelipatan)
        {
            return (int)(Math.Ceiling(angka / kelipatan) * kelipatan);
        }

        public async Task<Object> GetDirections(CreateDirectionsDto dto)
        {
            try
            {
                var cekApiKey = await _settingCollection.Find(x => x.Key == "APIKEYmaps").FirstOrDefaultAsync();
                if (cekApiKey == null)
                {
                    return new
                    {
                        code = 404,
                        data = (string)null,
                        message = "API Key tidak ditemukan"
                    };
                }
                var cekLayanan = await _hargaDriverCollection.Find(_ => true).ToListAsync();


                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"https://maps.googleapis.com/maps/api/directions/json?origin={dto.OriginLat},{dto.OriginLon}&destination={dto.DestinationLat},{dto.DestinationLon}&key={cekApiKey.Value}");
                var results = await response.Content.ReadAsStringAsync();
                var jsonResponse = Newtonsoft.Json.Linq.JObject.Parse(results);
                var polylinePoints = jsonResponse["routes"]?[0]?["overview_polyline"]?["points"]?.ToString();

                var leg = jsonResponse["routes"]?[0]?["legs"]?[0];
                var distanceText = leg?["distance"]?["text"]?.ToString();
                var distanceValue = leg?["distance"]?["value"]?.ToObject<int>();
                var durationText = leg?["duration"]?["text"]?.ToString();
                var durationValue = leg?["duration"]?["value"]?.ToObject<int>();
                var startAddress = leg?["start_address"]?.ToString();
                var endAddress = leg?["end_address"]?.ToString();

                var service = new TarifService();
                var hargaMotor = distanceValue.HasValue
                    ? service.HitungTarifMotorDriver(1, distanceValue.Value / 1000)
                    : throw new ArgumentNullException(nameof(distanceValue), "Distance value cannot be null");
                var hargaMobil = distanceValue.HasValue
                    ? service.HitungTarifMobilDriver(1, distanceValue.Value / 1000)
                    : throw new ArgumentNullException(nameof(distanceValue), "Distance value cannot be null");
                var hargaTaxi = service.HitungTarifTaksi("bluebird_reguler", distanceValue.Value / 1000, durationValue.HasValue ? (double)durationValue.Value / 60.0 : 0);

                if (response.IsSuccessStatusCode)
                {
                    var points = DecodePolyline(polylinePoints);
                    var result = points.Select(p => new LatLngDto
                    {
                        Latitude = p.Latitude,
                        Longitude = p.Longitude
                    }).ToList();

                    return new
                    {
                        code = 200,
                        coordinate = result,
                        asal = CleanAddress(startAddress ?? string.Empty),
                        tujuan = CleanAddress(endAddress ?? string.Empty),
                        distanceText = distanceText,
                        durationText = durationText,
                        distanceValue = distanceValue,
                        durationValue = durationValue,
                        hargaMotor = BulatkanKeKelipatan((int)Math.Ceiling(Convert.ToDouble(hargaMotor)), 1000),
                        hargaMobil = BulatkanKeKelipatan((int)Math.Ceiling(Convert.ToDouble(hargaMobil)), 1000),
                        hargaTaxi = BulatkanKeKelipatan((int)Math.Ceiling((double)hargaTaxi), 1000),
                        listLayanan = cekLayanan.Select(x =>
                        {
                            if (x.Type != null && x.Type.ToLower() == "motor")
                            {
                                x.Harga = Convert.ToInt32(hargaMotor) + (x.KenaikanHarga ?? 0);
                                x.Durasi = durationValue?.ToString() ?? string.Empty;
                            }
                            else if (x.Type != null && x.Type.ToLower() == "mobil")
                            {
                                x.Harga = Convert.ToInt32(hargaMobil) + (x.KenaikanHarga ?? 0);
                                x.Durasi = durationValue?.ToString() ?? string.Empty;
                            }
                            else if (x.Type != null && x.Type.ToLower() == "taxi")
                            {
                                x.Harga = Convert.ToInt32(hargaTaxi) + (x.KenaikanHarga ?? 0);
                                x.Durasi = durationValue?.ToString() ?? string.Empty;
                            }
                            return x;
                        }).ToList(),
                        message = "Success get polyline points"
                    };
                }
                else
                {
                    return $"Failed to send OTP. Response: {results}";
                }
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> GetSearchLocation(CreateDirectionsDto dto)
        {
            try
            {
                var cekApiKey = await _settingCollection.Find(x => x.Key == "APIKEYmaps").FirstOrDefaultAsync();
                if (cekApiKey == null)
                {
                    return new
                    {
                        code = 404,
                        data = (string)null,
                        message = "API Key tidak ditemukan"
                    };
                }

                using var httpClient = new HttpClient();
                var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={Uri.EscapeDataString(dto.NameSearch)}&key={cekApiKey.Value}&components=country:id"; // Restrict ke Indonesia (optional)
                var response = await httpClient.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = Newtonsoft.Json.Linq.JObject.Parse(result);
                    var predictions = jsonResponse["predictions"]?.Select(p => new
                    {
                        description = p["description"]?.ToString(),
                        placeId = p["place_id"]?.ToString()
                    }).Take(10).ToList();

                    return new
                    {
                        code = 200,
                        data = predictions,
                        message = "Success get location suggestions"
                    };
                }
                else
                {
                    return new
                    {
                        code = 500,
                        message = "Failed to get suggestions",
                        data = result
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    code = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<object> GetPlaceLocation(CreateDirectionsDto dto)
        {
            try
            {
                var cekApiKey = await _settingCollection.Find(x => x.Key == "APIKEYmaps").FirstOrDefaultAsync();
                if (cekApiKey == null)
                {
                    return new
                    {
                        code = 404,
                        data = (string)null,
                        message = "API Key tidak ditemukan"
                    };
                }

                using var httpClient = new HttpClient();
                var url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={dto.NameSearch}&key={cekApiKey.Value}";
                var response = await httpClient.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = Newtonsoft.Json.Linq.JObject.Parse(result);
                    var location = jsonResponse["result"]?["geometry"]?["location"];
                    var address = jsonResponse["result"]?["formatted_address"]?.ToString();

                    return new
                    {
                        code = 200,
                        data = new
                        {
                            address = address,
                            latitude = location?["lat"]?.ToString(),
                            longitude = location?["lng"]?.ToString()
                        },
                        message = "Success get location detail"
                    };
                }
                else
                {
                    return new
                    {
                        code = 500,
                        message = "Failed to get place details",
                        data = result
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    code = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<Object> GetAddressFromLatLon(CreateDirectionsDto dto)
        {
            try
            {
                var cekApiKey = await _settingCollection.Find(x => x.Key == "APIKEYmaps").FirstOrDefaultAsync();
                if (cekApiKey == null)
                {
                    return "API Key tidak ditemukan";
                }

                using var httpClient = new HttpClient();
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={dto.OriginLat},{dto.OriginLon}&key={cekApiKey.Value}";
                var response = await httpClient.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();

                var json = Newtonsoft.Json.Linq.JObject.Parse(result);
                var status = json["status"]?.ToString();

                if (status == "OK")
                {
                    var formattedAddress = json["results"]?[0]?["formatted_address"]?.ToString();
                    return new
                    {
                        code = 200,
                        data = new
                        {
                            address = CleanAddress(formattedAddress ?? string.Empty) ?? "Alamat tidak ditemukan",
                            latitude = dto.OriginLat,
                            longitude = dto.OriginLon,
                        },
                        message = "Success get location detail"
                    };
                }
                else
                {
                    return $"Gagal mendapatkan alamat. Status: {status}";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }





        public static List<(double Latitude, double Longitude)> DecodePolyline(string encodedPolyline)
        {
            var polylineChars = encodedPolyline.ToCharArray();
            int index = 0;
            int currentLat = 0;
            int currentLng = 0;
            var coordinates = new List<(double Latitude, double Longitude)>();

            while (index < polylineChars.Length)
            {
                // Decode latitude
                int shift = 0;
                int result = 0;
                int b;
                do
                {
                    b = polylineChars[index++] - 63;
                    result |= (b & 0x1F) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int deltaLat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                currentLat += deltaLat;

                // Decode longitude
                shift = 0;
                result = 0;
                do
                {
                    b = polylineChars[index++] - 63;
                    result |= (b & 0x1F) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int deltaLng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                currentLng += deltaLng;

                coordinates.Add((currentLat / 1E5, currentLng / 1E5));
            }

            return coordinates;
        }

        private string CleanAddress(string address)
        {
            if (string.IsNullOrEmpty(address)) return "";

            if (address.Contains(","))
            {
                var parts = address.Split(',', 2);
                if (parts[0].Contains("+"))
                {
                    return parts[1].Trim();
                }
            }
            return address;
        }

    }
}

public class LatLngDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class TarifMotorDriver
{
    public int Bawah { get; set; }
    public int Atas { get; set; }
    public int Minimal { get; set; }
}

public class TarifTaksi
{
    public int BukaPintu { get; set; }
    public int PerKm { get; set; }
    public int PerMenit { get; set; }
}

public class TarifService
{
    private readonly Dictionary<int, TarifMotorDriver> _tarifMotor = new Dictionary<int, TarifMotorDriver>
    {
        { 1, new TarifMotorDriver { Bawah = 2000, Atas = 2500, Minimal = 12000 } },
        { 2, new TarifMotorDriver { Bawah = 2550, Atas = 2800, Minimal = 10200 } },
        { 3, new TarifMotorDriver { Bawah = 2100, Atas = 2600, Minimal = 7000 } }
    };

    private readonly Dictionary<int, TarifMotorDriver> _tarifMobil = new Dictionary<int, TarifMotorDriver>
    {
        { 1, new TarifMotorDriver { Bawah = 3400, Atas = 4250, Minimal = 25000 } },
        { 2, new TarifMotorDriver { Bawah = 3500, Atas = 5000, Minimal = 15000 } },
        { 3, new TarifMotorDriver { Bawah = 3100, Atas = 3900, Minimal = 10500 } }
    };

    private readonly Dictionary<string, TarifTaksi> _tarifTaxi = new Dictionary<string, TarifTaksi>
    {
        { "bluebird_reguler", new TarifTaksi { BukaPintu = 7000, PerKm = 4500, PerMenit = 500 } },
        { "bluebird_eksekutif", new TarifTaksi { BukaPintu = 15000, PerKm = 7000, PerMenit = 750 } },
        { "grabcar_taxi", new TarifTaksi { BukaPintu = 8000, PerKm = 5400, PerMenit = 500 } }
    };

    public object HitungTarifMotorDriver(int zona, double jarakKm)
    {
        if (!_tarifMotor.ContainsKey(zona))
        {
            return "Zona tidak valid";
        }

        var tarif = _tarifMotor[zona];

        var tarifBawah = jarakKm * tarif.Bawah;
        var tarifAtas = jarakKm * tarif.Atas;

        tarifBawah = Math.Max(tarifBawah, tarif.Minimal);
        tarifAtas = Math.Max(tarifAtas, tarif.Minimal);

        return tarifAtas;
    }

    public object HitungTarifMobilDriver(int zona, double jarakKm)
    {
        if (!_tarifMobil.ContainsKey(zona))
        {
            return "Zona tidak valid";
        }

        var tarif = _tarifMobil[zona];

        var tarifBawah = jarakKm * tarif.Bawah;
        var tarifAtas = jarakKm * tarif.Atas;

        tarifBawah = Math.Max(tarifBawah, tarif.Minimal);
        tarifAtas = Math.Max(tarifAtas, tarif.Minimal);

        return tarifAtas;
    }

    public object HitungTarifTaksi(string tipe, double jarakKm, double waktuMenit)
    {
        if (!_tarifTaxi.ContainsKey(tipe))
        {
            return "Tipe taksi tidak valid";
        }

        var tarif = _tarifTaxi[tipe];

        double totalTarif = tarif.BukaPintu + (jarakKm * tarif.PerKm) + (waktuMenit * tarif.PerMenit);

        return totalTarif;
    }
}
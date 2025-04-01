using System;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

public class FirebaseService
{
    private static bool _isInitialized = false;

    public static void InitializeFirebase()
    {
        if (!_isInitialized)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("firebase-service-account.json") // Path ke JSON
            });

            _isInitialized = true;
        }
    }

    public static async Task<string> SendPushNotification(string deviceToken, string title, string body)
    {
        InitializeFirebase(); // Pastikan Firebase sudah diinisialisasi

        var message = new Message()
        {
            Token = deviceToken,
            Notification = new Notification()
            {
                Title = title,
                Body = body
            },
            Data = new Dictionary<string, string>
            {
                { "forceOpen", "true" } // Bisa digunakan untuk membuka aplikasi otomatis
            }
        };

        string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        return response; // Response dari Firebase
    }
}

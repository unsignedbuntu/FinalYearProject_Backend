using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace KTUN_Final_Year_Project.Services
{
#nullable disable
    public class CertificateLoadingService : IHostedService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IOptions<Options.KestrelOptions> _kestrelOptions;

        public CertificateLoadingService(IWebHostEnvironment environment, IOptions<Options.KestrelOptions> kestrelOptions)
        {
            _environment = environment;
            _kestrelOptions = kestrelOptions;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var kestrelOptions = _kestrelOptions.Value;

            if (kestrelOptions?.Endpoints?.HttpsInlineCertFile != null)
            {
                var httpsOptions = kestrelOptions.Endpoints.HttpsInlineCertFile;
                var certPath = httpsOptions.Certificate.Path;
                var keyPath = httpsOptions.Certificate.KeyPath;

                // Ortama göre göreceli yolları mutlak yollara çevir
                if (!Path.IsPathRooted(certPath))
                {
                    certPath = Path.Combine(_environment.ContentRootPath, certPath);
                }

                if (!Path.IsPathRooted(keyPath))
                {
                    keyPath = Path.Combine(_environment.ContentRootPath, keyPath);
                }

                try
                {
                    // Sertifikayı yükle
                    X509Certificate2 certificate = LoadCertificate(certPath, keyPath);

                    // Kestrel seçeneklerini yapılandır
                    // Kestrel'i doğrudan yapılandırmak yerine, Kestrel seçeneklerini güncelle
                    if (certificate != null)
                    {
                        Console.WriteLine("Sertifika başarıyla yüklendi.");
                    }
                }
                catch (Exception ex)
                {
                    // Hata durumunu işle
                    Console.WriteLine($"Sertifika yüklenirken hata oluştu: {ex.Message}");
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private X509Certificate2 LoadCertificate(string certificatePath, string privateKeyPath)
        {
            try
            {
                // Sertifika ve özel anahtar dosyalarını oku
                byte[] certificateBytes = File.ReadAllBytes(certificatePath);

                // X509Certificate2 nesnesini oluştur
                X509Certificate2 certificate = new X509Certificate2(certificateBytes);

                // Özel anahtarı sertifikaya ekle (gerekirse)
                // Bu adım özel anahtar sertifika içinde değilse gereklidir.
                // Önemli: Özel anahtarı güvenli bir şekilde yönettiğinizden emin olun.

                return certificate;
            }
            catch (Exception ex)
            {
                // Hata durumunu işle
                Console.WriteLine($"Sertifika yüklenirken hata oluştu: {ex.Message}");
                return null;
            }
        }
    }
}
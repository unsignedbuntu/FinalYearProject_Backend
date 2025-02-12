using KTUN_Final_Year_Project.Services;
using StackExchange.Redis;

public class RedisService : IRedisService
{
    private readonly ConnectionMultiplexer redis;
    private readonly IDatabase db;

    public RedisService(string connectionString)
    {
        try
        {
            var options = ConfigurationOptions.Parse(connectionString);
            options.AbortOnConnectFail = false; // Bağlantı hatalarında yeniden deneme yapılsın
            redis = ConnectionMultiplexer.Connect(options);
            db = redis.GetDatabase();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Redis bağlantı hatası: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteFromCacheAsync(string promptHash)
    {
        return await db.KeyDeleteAsync(promptHash);
    }

    public async Task<bool> ExistsInCacheAsync(string promptHash)
    {
        return await db.KeyExistsAsync(promptHash);
    }

    public async Task<bool> ExtendExpiryAsync(string promptHash, TimeSpan newDuration)
    {
        return await db.KeyExpireAsync(promptHash, newDuration);
    }


    public async Task<string?> GetImageFromCacheAsync(string promptHash)
    {
        try
        {
            return await db.StringGetAsync(promptHash);
        }
        catch (Exception ex)
        {
            // Loglama yapılabilir
            Console.WriteLine($"Redis get error: {ex.Message}");
            return null;
        }
    }

    public async Task SaveImageToCacheAsync(string promptHash, string imageUrl)
    {
        try
        {
            await db.StringSetAsync(promptHash, imageUrl, TimeSpan.FromDays(120));
        }
        catch (Exception ex)
        {
            // Loglama yapılabilir
            Console.WriteLine($"Redis save error: {ex.Message}");
            throw;
        }
    }
}

namespace KTUN_Final_Year_Project.Services
{
    public interface IRedisService
    {
        // Cache'den görsel alma
        Task<string?> GetImageFromCacheAsync(string promptHash);

        // Cache'e görsel kaydetme
        Task SaveImageToCacheAsync(string promptHash, string imageUrl);

        // Cache'den veri silme
        Task<bool> DeleteFromCacheAsync(string promptHash);

        // Cache'de veri var mı kontrolü
        Task<bool> ExistsInCacheAsync(string promptHash);

        // Cache'deki verinin süresini uzatma
        Task<bool> ExtendExpiryAsync(string promptHash, TimeSpan newDuration);
    }
}


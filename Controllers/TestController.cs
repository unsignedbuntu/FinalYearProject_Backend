using KTUN_Final_Year_Project.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IRedisService _redisService;

    public TestController(IRedisService redisService)
    {
        _redisService = redisService;
    }

    [HttpGet("test-redis")]
    public async Task<IActionResult> TestRedisConnection()
    {
        try
        {
            await _redisService.SaveImageToCacheAsync("test-key", "test-value");
            var cachedValue = await _redisService.GetImageFromCacheAsync("test-key");

            if (cachedValue != null)
            {
                return Ok("Redis bağlantısı başarılı");
            }
            return BadRequest("Redis bağlantısı başarısız");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Redis bağlantı hatası: {ex.Message}");
        }
    }
}
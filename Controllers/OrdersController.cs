using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using KTUN_Final_Year_Project.Entities;
using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace KTUN_Final_Year_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;

        public OrdersController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetOrders()
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.Status != "Cancelled")
                .Select(o => _mapper.Map<OrdersResponseDTO>(o))
                .ToList();
                
            return Ok(orders);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetOrderByID(int id)
        {
            var order = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.OrderID == id && o.Status != "Cancelled");

            if (order == null)
            {
                return NotFound();
            }

            var orderResponse = _mapper.Map<OrdersResponseDTO>(order);
            return Ok(orderResponse);
        }

        [HttpGet("ByUser/{userId}")]
        [Produces("application/json")]
        public IActionResult GetOrdersByUserID(int userId)
        {
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserID == userId && o.Status != "Cancelled")
                .Select(o => _mapper.Map<OrdersResponseDTO>(o))
                .ToList();

            return Ok(orders);
        }

        [HttpGet("ByStatus/{orderStatus}")]
        [Produces("application/json")]
        public IActionResult GetOrdersByStatus(string orderStatus)
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.Status == orderStatus)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return Ok(orders);
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> CreateOrder([FromBody] OrdersDTO orderDTO)
        {
            if (orderDTO == null || !ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto<object> { Success = false, Message = "Invalid order data.", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }

            if (orderDTO.OrderItems == null || !orderDTO.OrderItems.Any())
            {
                return BadRequest(new ApiResponseDto<object> { Success = false, Message = "Order must contain at least one item." });
            }

            // Transaction başlat
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Ana Siparişi Oluştur
                var order = _mapper.Map<Orders>(orderDTO); // UserID ve ShippingAddress gibi alanlar maplenmeli
                order.OrderDate = DateTime.UtcNow; // Use UtcNow for consistency
                order.Status = "Pending";
                order.TotalAmount = 0; // Başlangıçta sıfırla, ürünler eklendikçe hesapla

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // OrderID'nin oluşması için ana siparişi önce kaydet

                decimal calculatedTotalAmount = 0;

                // 2. Sipariş Kalemlerini (OrderItems) İşle ve Kaydet
                foreach (var itemDto in orderDTO.OrderItems) // orderDTO.OrderItems, frontend'den gelen ürün listesi
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == itemDto.ProductID);
                    if (product == null || !product.Status)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(new ApiResponseDto<object> { Success = false, Message = $"Product with ID {itemDto.ProductID} not found or not active." });
                    }

                    if (product.StockQuantity < itemDto.Quantity)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(new ApiResponseDto<object> { Success = false, Message = $"Insufficient stock for product: {product.ProductName}. Requested: {itemDto.Quantity}, In Stock: {product.StockQuantity}" });
                    }

                    var orderItem = new OrderItems
                    {
                        OrderID = order.OrderID, // Yeni oluşturulan Order'ın ID'si
                        ProductID = itemDto.ProductID,
                        Quantity = itemDto.Quantity,
                        PriceAtPurchase = itemDto.PriceAtPurchase // Bu frontend'den gelmeli veya product.Price alınmalı
                    };
                    _context.OrderItems.Add(orderItem);

                    // Ürün stok miktarını güncelle
                    product.StockQuantity -= itemDto.Quantity;
                    _context.Products.Update(product);

                    calculatedTotalAmount += itemDto.PriceAtPurchase * itemDto.Quantity;
                }

                // 3. Siparişin Toplam Tutarını Güncelle
                order.TotalAmount = calculatedTotalAmount;
                _context.Orders.Update(order);

                await _context.SaveChangesAsync(); // OrderItems, Product stok güncellemeleri ve Order TotalAmount güncellemesini kaydet

                await transaction.CommitAsync(); // Her şey başarılıysa transaction'ı onayla

                // 4. Başarılı Yanıtı Döndür
                // Sadece temel sipariş bilgilerini döndürmek genellikle yeterlidir.
                // My Orders sayfasında detaylar zaten ayrıca OrderItems endpoint'inden çekilecek.
                var responsePayload = new 
                {
                    OrderID = order.OrderID,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    ShippingAddress = order.ShippingAddress,
                    UserID = order.UserID
                    // İsterseniz _mapper.Map<OrdersResponseDTO>(order) da kullanabilirsiniz,
                    // ancak OrderItems'ın bu aşamada dolu gelmesi gerekmeyebilir.
                };
                return Ok(new ApiResponseDto<object> { Success = true, Data = responsePayload, Message = "Order created successfully." });
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"DbUpdateException in CreateOrder: {dbEx.InnerException?.Message ?? dbEx.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseDto<object> { Success = false, Message = "An error occurred while saving order data. Please try again.", Errors = new List<string> { dbEx.InnerException?.Message ?? dbEx.Message } });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Exception in CreateOrder: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseDto<object> { Success = false, Message = "An unexpected error occurred. Please try again.", Errors = new List<string> { ex.Message } });
            }
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateOrder(int id, [FromBody] OrdersDTO orderDTO)
        {
            if (orderDTO == null)
            {
                return BadRequest();
            }

            var order = _context.Orders.FirstOrDefault(o => o.OrderID == id && o.Status != "Cancelled");

            if (order == null)
            {
                return NotFound();
            }

            // Sipariş güncelleniyor
            order.UserID = orderDTO.UserID;
            order.TotalAmount = orderDTO.TotalAmount;
            order.Status = orderDTO.Status;
            order.ShippingAddress = orderDTO.ShippingAddress;

            _context.SaveChanges();

            var updatedOrder = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.OrderID == id);

            var orderResponse = _mapper.Map<OrdersResponseDTO>(updatedOrder);
            return Ok(orderResponse);
        }

        [HttpDelete("SoftDelete_Status{id}")]
        [Produces("application/json")]
        public IActionResult SoftDeleteOrderByStatus(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == id && o.Status != "Cancelled");

            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Cancelled";

            _context.Orders.Update(order);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("Cancel/{id}")]
        [Produces("application/json")]
        public IActionResult CancelOrder(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.OrderID == id && o.Status != "Cancelled");
                
            if (order == null)
            {
                return NotFound();
            }

            // Sadece belirli durumdaki siparişler iptal edilebilir
            if (order.Status != "Pending" && order.Status != "Confirmed")
            {
                return BadRequest("Bu durumdaki sipariş iptal edilemez.");
            }

            // Stokları geri ekle
            foreach (var orderItem in order.OrderItems)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductID == orderItem.ProductID);
                if (product != null)
                {
                    product.StockQuantity += orderItem.Quantity;
                    _context.Products.Update(product);
                }
            }

            order.Status = "Cancelled";
            _context.Orders.Update(order);
            _context.SaveChanges();

            var updatedOrder = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.OrderID == id);

            return Ok(updatedOrder);
        }
    }
} 
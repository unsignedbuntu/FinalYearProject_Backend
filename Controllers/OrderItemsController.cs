using KTUN_Final_Year_Project.DTOs;
using KTUN_Final_Year_Project.ResponseDTOs;
using Microsoft.AspNetCore.Mvc;
using KTUN_Final_Year_Project.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KTUN_Final_Year_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemsController : ControllerBase
    {
        private readonly KTUN_DbContext _context;
        private readonly IMapper _mapper;
        
        public OrderItemsController(KTUN_DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetOrderItems()
        {
            var orderItems = _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Where(oi => oi.Order != null && oi.Order.Status != "Cancelled")
                .Select(oi => _mapper.Map<OrderItemsResponseDTO>(oi))
                .ToList();
                
            return Ok(orderItems);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        public IActionResult GetOrderItemByID(int id)
        {
            var orderItem = _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .FirstOrDefault(oi => oi.OrderItemID == id && oi.Order != null && oi.Order.Status != "Cancelled");

            if (orderItem == null)
            {
                return NotFound();
            }

            var orderItemResponse = _mapper.Map<OrderItemsResponseDTO>(orderItem);
            return Ok(orderItemResponse);
        }

        [HttpGet("ByOrder/{orderId}")]
        [Produces("application/json")]
        public IActionResult GetOrderItemsByOrderID(int orderId)
        {
            var orderItems = _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .Where(oi => oi.OrderID == orderId && oi.Order != null && oi.Order.Status != "Cancelled")
                .Select(oi => _mapper.Map<OrderItemsResponseDTO>(oi))
                .ToList();

            if (orderItems.Count == 0)
            {
                return NotFound();
            }

            return Ok(orderItems);
        }

        [HttpGet("ByProduct/{productId}")]
        [Produces("application/json")]
        public IActionResult GetOrderItemsByProductID(int productId)
        {
            var orderItems = _context.OrderItems
                .Include(oi => oi.Order)
                    .ThenInclude(o => o!.User)
                .Where(oi => oi.ProductID == productId)
                .Where(oi => oi.Order != null && oi.Order.Status != "Cancelled")
                .ToList();

            var response = _mapper.Map<List<OrderItemsResponseDTO>>(orderItems);

            return Ok(response);
        }

        [HttpPost]
        [Produces("application/json")]
        public IActionResult CreateOrderItem([FromBody] OrderItemsDTO orderItemDTO)
        {
            if (orderItemDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Sipariş ve ürün varlığını kontrol et
            var order = _context.Orders.FirstOrDefault(o => o.OrderID == orderItemDTO.OrderID && o.Status != "Cancelled");
            if (order == null)
            {
                return BadRequest("Sipariş bulunamadı veya iptal edilmiş.");
            }

            // Ürün stok kontrolü
            var product = _context.Products.FirstOrDefault(p => p.ProductID == orderItemDTO.ProductID);
            if (product == null || !product.Status)
            {
                return BadRequest("Ürün bulunamadı veya aktif değil.");
            }

            if (product.StockQuantity < orderItemDTO.Quantity)
            {
                return BadRequest($"Yetersiz stok: {product.ProductName}, Talep: {orderItemDTO.Quantity}, Stok: {product.StockQuantity}");
            }

            var orderItem = _mapper.Map<OrderItems>(orderItemDTO);

            _context.OrderItems.Add(orderItem);
            
            // Ürün stok miktarını güncelle
            product.StockQuantity -= orderItemDTO.Quantity;
            _context.Products.Update(product);
            
            // Sipariş toplam tutarını güncelle
            order.TotalAmount += orderItemDTO.PriceAtPurchase * orderItemDTO.Quantity;
            _context.Orders.Update(order);
            
            _context.SaveChanges();

            var createdOrderItem = _context.OrderItems
                .Include(oi => oi.Product)
                .FirstOrDefault(oi => oi.OrderItemID == orderItem.OrderItemID);

            var orderItemResponse = _mapper.Map<OrderItemsResponseDTO>(createdOrderItem);
            return Ok(orderItemResponse);
        }

        [HttpPut("{id}")]
        [Produces("application/json")]
        public IActionResult UpdateOrderItem(int id, [FromBody] OrderItemsDTO orderItemDTO)
        {
            if (orderItemDTO == null)
            {
                return BadRequest();
            }

            var orderItem = _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .FirstOrDefault(oi => oi.OrderItemID == id && oi.Order != null && oi.Order.Status != "Cancelled");

            if (orderItem == null)
            {
                return NotFound();
            }

            // Sipariş ve ürün varlığını kontrol et
            var order = orderItem.Order;
            var product = orderItem.Product;

            if (order == null)
            {
                return BadRequest("İlişkili sipariş bulunamadı.");
            }
            if (product == null)
            {
                return BadRequest("İlişkili ürün bulunamadı.");
            }
            
            // Yeni ProductID için ürün kontrolü (eğer ProductID değişiyorsa)
            Products? newProduct = product;
            if(orderItem.ProductID != orderItemDTO.ProductID)
            {
                newProduct = _context.Products.FirstOrDefault(p => p.ProductID == orderItemDTO.ProductID);
                 if (newProduct == null || !newProduct.Status)
                 {
                     return BadRequest("Yeni ürün bulunamadı veya aktif değil.");
                 }
            }
            
            // Miktar değişikliği durumunda stok kontrolü
            int quantityDifference = orderItemDTO.Quantity - orderItem.Quantity;
            
            // Eğer ürün değiştiyse eski ürün stoğunu geri ver, yeni üründen düş
            if (orderItem.ProductID != orderItemDTO.ProductID)
            {
                 if (product.StockQuantity + orderItem.Quantity < 0) { /* Eski stok geri eklenirken hata olmaması için? Gerekli olmayabilir. */ }
                 product.StockQuantity += orderItem.Quantity; // Eski ürüne iade
                 
                 if (newProduct != null && newProduct.StockQuantity < orderItemDTO.Quantity) // Yeni ürün stok kontrolü
                 {
                      return BadRequest($"Yetersiz stok (yeni ürün): {newProduct.ProductName}, Talep: {orderItemDTO.Quantity}, Stok: {newProduct.StockQuantity}");
                 }
                 if (newProduct != null) { newProduct.StockQuantity -= orderItemDTO.Quantity; }
                 
                 _context.Products.Update(product); // Eski ürünü güncelle
                 if (newProduct != null) { _context.Products.Update(newProduct); } // Yeni ürünü güncelle
            }
            else // Ürün aynıysa sadece farkı düş/ekle
            {
                 if (quantityDifference > 0 && product.StockQuantity < quantityDifference)
                 {
                     return BadRequest($"Yetersiz stok: {product.ProductName}, Ek Talep: {quantityDifference}, Stok: {product.StockQuantity}");
                 }
                 product.StockQuantity -= quantityDifference;
                 _context.Products.Update(product);
            }
            
            // Sipariş toplam tutarını güncelle
            order.TotalAmount -= orderItem.PriceAtPurchase * orderItem.Quantity; // Eski tutarı çıkar
            order.TotalAmount += orderItemDTO.PriceAtPurchase * orderItemDTO.Quantity; // Yeni tutarı ekle
            _context.Orders.Update(order);
            
            // Sipariş öğesini güncelle
            orderItem.OrderID = orderItemDTO.OrderID;
            orderItem.ProductID = orderItemDTO.ProductID;
            orderItem.Quantity = orderItemDTO.Quantity;
            orderItem.PriceAtPurchase = orderItemDTO.PriceAtPurchase;

            _context.OrderItems.Update(orderItem);
            _context.SaveChanges();

            var updatedOrderItem = _context.OrderItems
                .Include(oi => oi.Product)
                .FirstOrDefault(oi => oi.OrderItemID == id);

            var orderItemResponse = _mapper.Map<OrderItemsResponseDTO>(updatedOrderItem);
            return Ok(orderItemResponse);
        }

        [HttpDelete("{id}")]
        [Produces("application/json")]
        public IActionResult DeleteOrderItem(int id)
        {
            var orderItem = _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .FirstOrDefault(oi => oi.OrderItemID == id);

            if (orderItem == null)
            {
                return NotFound();
            }

            // Stok miktarını geri ver
            var product = orderItem.Product;
            if (product != null)
            {
                product.StockQuantity += orderItem.Quantity;
                _context.Products.Update(product);
            }

            // Sipariş toplam tutarını güncelle
            var order = orderItem.Order;
            if (order != null)
            {
                order.TotalAmount -= orderItem.PriceAtPurchase * orderItem.Quantity;
                _context.Orders.Update(order);
            }

            _context.OrderItems.Remove(orderItem);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

// OrderItemUpdateDTO sınıfı
namespace KTUN_Final_Year_Project.DTOs
{
    public class OrderItemUpdateDTO
    {
        public int Quantity { get; set; }
    }
} 
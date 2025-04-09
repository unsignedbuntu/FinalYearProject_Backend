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
        public IActionResult CreateOrder([FromBody] OrdersDTO orderDTO)
        {
            if (orderDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = _mapper.Map<Orders>(orderDTO);
            order.OrderDate = DateTime.Now;
            order.Status = "Pending";

            _context.Orders.Add(order);
            _context.SaveChanges();

            var createdOrder = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.OrderID == order.OrderID);

            var orderResponse = _mapper.Map<OrdersResponseDTO>(createdOrder);
            return Ok(orderResponse);
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
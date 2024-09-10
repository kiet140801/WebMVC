using App.Core.Commons;
using App.Core.Entities;
using App.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDemoApplication.Models.Dtos;

namespace WebDemoApplication.Controllers;

public class OrderController : Controller
{
    private readonly IBaseRepository<Order> _orderRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IBaseRepository<CartItem> _cartItemRepository;
    public OrderController(IBaseRepository<Order> orderRepository, ICurrentUser currentUser, IBaseRepository<CartItem> cartItemRepository)
    {
        _orderRepository = orderRepository;
        _currentUser = currentUser;
        _cartItemRepository = cartItemRepository;
    }
    public async Task<IActionResult> Index()
    {
        var userId = _currentUser.GetUserId();
        var orderItems = await _orderRepository.GetAll()
                                               .Where(x => x.UserId == userId)
                                               .SelectMany(x => x.Items.Select(y => new OrderDto
                                               {
                                                   OrderId = y.OrderId,
                                                   ProductName = y.Product.Name,
                                                   Quantity = y.Quantity,
                                                   Price = y.Price,
                                                   CreatedDate = x.CreatedDate
                                               })).ToListAsync();

        return View(orderItems);
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder()
    {
        var userId = _currentUser.GetUserId();
        var cartItems = await _cartItemRepository.GetAll().Where(x => x.UserId == userId)
                                                     .Include(x => x.Product)
                                                     .ToListAsync();
        var order = new Order
        {
            CreatedDate = DateTime.Now,
            UserId = userId,
            Items = cartItems.Select(x => new OrderItem
            {
                ProductId = x.ProductId,
                Price = x.Product.Price,
                Quantity = x.Quantity
            }).ToList()
        };

        _cartItemRepository.RemoveRange(cartItems.ToArray());
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        return RedirectToAction("Index", "Order");
    }
}
